using Microsoft.Extensions.Logging;
using PortEval.Application.Services.Interfaces.BackgroundJobs;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.FinancialDataFetcher.Interfaces;
using PortEval.FinancialDataFetcher.Models;
using PortEval.FinancialDataFetcher.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs.Enums;
using PortEval.Application.Services.Extensions;
using PortEval.Application.Services.Interfaces;

namespace PortEval.BackgroundJobs.MissingPricesFetch
{
    /// <summary>
    /// Retrieves missing instrument prices of all known and tracked instruments to maintain the following intervals:
    /// <list type="bullet">
    ///     <item>1 day for prices older than 5 days.</item>
    ///     <item>1 hour for prices between 2 and 5 days.</item>
    ///     <item>5 minutes for prices in the last 24 hours.</item>
    /// </list>
    /// </summary>
    public class MissingInstrumentPricesFetchJob : IMissingInstrumentPricesFetchJob
    {
        private readonly IInstrumentRepository _instrumentRepository;
        private readonly IInstrumentPriceRepository _instrumentPriceRepository;
        private readonly ICurrencyExchangeRateRepository _exchangeRateRepository;
        private readonly INotificationService _notificationService;
        private readonly IPriceFetcher _fetcher;
        private readonly ILogger _logger;

        public MissingInstrumentPricesFetchJob(IInstrumentRepository instrumentRepository, IInstrumentPriceRepository instrumentPriceRepository,
            ICurrencyExchangeRateRepository exchangeRateRepository, INotificationService notificationService, IPriceFetcher fetcher, ILoggerFactory loggerFactory)
        {
            _instrumentRepository = instrumentRepository;
            _instrumentPriceRepository = instrumentPriceRepository;
            _exchangeRateRepository = exchangeRateRepository;
            _notificationService = notificationService;
            _fetcher = fetcher;            
            _logger = loggerFactory.CreateLogger(typeof(MissingInstrumentPricesFetchJob));
        }

        /// <summary>
        /// Starts the job.
        /// </summary>
        /// <returns>A task representing the asynchronous job processing operation.</returns>
        public async Task Run()
        {
            var currentTime = DateTime.UtcNow;
            _logger.LogInformation($"Starting missing prices fetch at {currentTime}.");

            var instruments = await _instrumentRepository.ListAllAsync();

            foreach (var instrument in instruments)
            {
                if (!instrument.IsTracked) continue;

                var missingRanges = await GetMissingRanges(instrument, currentTime);
                foreach (var range in missingRanges)
                {
                    await ProcessMissingRange(instrument, range, currentTime);
                }

                instrument.TrackingInfo.Update(currentTime);
                _instrumentRepository.Update(instrument);
            }

            await _instrumentRepository.UnitOfWork.CommitAsync();
            _logger.LogInformation($"Missing prices fetch finished at {DateTime.UtcNow}.");
            await _notificationService.SendNotificationAsync(NotificationType.NewDataAvailable);
        }

        private async Task<IEnumerable<TimeRange>> GetMissingRanges(Instrument instrument, DateTime baseTime)
        {
            var prices = await _instrumentPriceRepository.ListInstrumentPricesAsync(instrument.Id);
            var priceTimes = prices
                .Where(p => p.Time >= instrument.TrackingInfo.StartTime)
                .OrderBy(p => p.Time)
                .Select(p => p.Time);

            var missingRanges = PriceUtils.GetMissingPriceRanges(
                priceTimes,
                time => PriceUtils.GetInstrumentPriceInterval(baseTime, time),
                instrument.TrackingInfo.StartTime,
                baseTime.RoundDown(PriceUtils.FiveMinutes)
            );

            return missingRanges;
        }

        /// <summary>
        /// Retrieves and saves prices of a single instrument in the given time range. If the retrieved prices currency and instrument currency differ, then
        /// the prices are converted to instrument's currency beforehand.
        /// </summary>
        /// <param name="instrument">Instrument to retrieve prices for.</param>
        /// <param name="range">Time range of missing prices.</param>
        /// <param name="baseTime">Time used to determine the appropriate price intervals.</param>
        /// <returns>A task representing the asynchronous retrieval, conversion and save operations.</returns>
        private async Task ProcessMissingRange(Instrument instrument, TimeRange range, DateTime baseTime)
        {
            var fetchResult = await FetchPricesInRange(instrument, range);
            if (fetchResult.StatusCode != StatusCode.Ok || fetchResult.Result is null)
            {
                return;
            }

            var filledPrices = await FillMissingPrices(fetchResult.Result, instrument, range, baseTime);
            var pricesToInsert = await ProcessPricePoints(filledPrices, instrument, range);

            await _instrumentPriceRepository.BulkInsertAsync(pricesToInsert);
        }

        private async Task<Response<IEnumerable<PricePoint>>> FetchPricesInRange(Instrument instrument, TimeRange range)
        {
            if (range.Interval < PriceUtils.OneDay)
            {
                var intradayInterval = range.Interval <= PriceUtils.FiveMinutes
                    ? IntradayInterval.FiveMinutes
                    : IntradayInterval.OneHour;
                return await _fetcher.GetIntradayPrices(instrument, range.From, range.To,
                    intradayInterval);
            }

            return await _fetcher.GetHistoricalDailyPrices(instrument, range.From, range.To);
        }

        private async Task<IEnumerable<PricePoint>> FillMissingPrices(IEnumerable<PricePoint> prices,
            Instrument instrument, TimeRange range, DateTime baseTime)
        {
            var priceAtRangeStart = await _instrumentPriceRepository.FindPriceAtAsync(instrument.Id, range.From);

            var rangeStartPricePoint = new PricePoint
            {
                Price = priceAtRangeStart.Price,
                CurrencyCode = instrument.CurrencyCode,
                Symbol = instrument.Symbol,
                Time = range.From
            };

            var newPricePoints = PriceUtils.FillMissingRangePrices(
                prices.Prepend(rangeStartPricePoint),
                time => PriceUtils.GetInstrumentPriceInterval(baseTime, time),
                range.From,
                range.To);

            return newPricePoints;
        }

        private async Task<List<InstrumentPrice>> ProcessPricePoints(IEnumerable<PricePoint> prices,
            Instrument instrument, TimeRange range)
        {
            var pricesToAdd = new List<InstrumentPrice>();
            foreach (var pricePoint in prices)
            {
                if (!(pricePoint.Time > range.From & pricePoint.Time < range.To) || pricePoint.Price <= 0m)
                {
                    continue;
                }

                try
                {
                    var price = await PriceUtils.GetConvertedPricePointPrice(_exchangeRateRepository, instrument, pricePoint);
                    pricesToAdd.Add(new InstrumentPrice(pricePoint.Time, price, instrument.Id));
                }
                catch (OperationNotAllowedException ex)
                {
                    _logger.LogError(ex.Message);
                }
            }

            return pricesToAdd;
        }
    }
}
