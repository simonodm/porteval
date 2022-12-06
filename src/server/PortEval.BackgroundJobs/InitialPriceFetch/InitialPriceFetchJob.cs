using EFCore.BulkExtensions;
using Microsoft.Extensions.Logging;
using PortEval.Domain.Models.Entities;
using PortEval.FinancialDataFetcher;
using PortEval.FinancialDataFetcher.Models;
using PortEval.FinancialDataFetcher.Responses;
using PortEval.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs.Enums;
using PortEval.Application.Services.Interfaces;
using PortEval.Application.Services.Interfaces.BackgroundJobs;
using PortEval.Domain;
using PortEval.FinancialDataFetcher.Interfaces;
using PortEval.Application.Services.Interfaces.Repositories;

namespace PortEval.BackgroundJobs.InitialPriceFetch
{
    /// <summary>
    /// Retrieves all available prices for the given instrument in the following intervals:
    /// <list type="bullet">
    ///     <item>1 day for prices older than 5 days.</item>
    ///     <item>1 hour for prices between 2 and 5 days.</item>
    ///     <item>5 minutes for prices in the last 24 hours.</item>
    /// </list>
    /// </summary>
    public class InitialPriceFetchJob : IInitialPriceFetchJob
    {
        private readonly IInstrumentRepository _instrumentRepository;
        private readonly IInstrumentPriceRepository _instrumentPriceRepository;
        private readonly ICurrencyExchangeRateRepository _exchangeRateRepository;
        private readonly INotificationService _notificationService;
        private readonly IPriceFetcher _fetcher;
        private readonly ILogger _logger;

        public InitialPriceFetchJob(IInstrumentRepository instrumentRepository, IInstrumentPriceRepository instrumentPriceRepository,
            ICurrencyExchangeRateRepository exchangeRateRepository, INotificationService notificationService, IPriceFetcher fetcher, ILoggerFactory loggerFactory)
        {
            _instrumentRepository = instrumentRepository;
            _instrumentPriceRepository = instrumentPriceRepository;
            _exchangeRateRepository = exchangeRateRepository;
            _notificationService = notificationService;
            _fetcher = fetcher;
            _logger = loggerFactory.CreateLogger(typeof(InitialPriceFetchJob));
        }

        /// <summary>
        /// Starts the job.
        /// </summary>
        /// <param name="instrumentId">ID of the instrument to retrieve prices for.</param>
        /// <returns>A task representing the asynchronous job processing operation.</returns>
        public async Task Run(int instrumentId)
        {
            _logger.LogInformation($"First price fetch for instrument {instrumentId} at {DateTime.UtcNow}.");
            var instrument = await _instrumentRepository.FindAsync(instrumentId);
            if (instrument == null)
            {
                _logger.LogError($"No instrument with id {instrumentId} found.");
                return;
            }
            var fetchStart = DateTime.UtcNow;

            var fiveDaysCutoffDate = fetchStart - PriceUtils.FiveDays;
            var oneDayCutoffDate = fetchStart - PriceUtils.OneDay;

            var dailyPricesResponse = await _fetcher.GetHistoricalDailyPrices(instrument, PortEvalConstants.FinancialDataStartTime,
                fiveDaysCutoffDate);
            var hourlyPricesResponse = await _fetcher.GetIntradayPrices(instrument, fiveDaysCutoffDate, oneDayCutoffDate,
                IntradayInterval.OneHour);
            var latestPricesResponse = await _fetcher.GetIntradayPrices(instrument, oneDayCutoffDate, fetchStart,
                IntradayInterval.FiveMinutes);

            var allFetchedPrices = ConcatFetchedPrices(dailyPricesResponse, hourlyPricesResponse,
                latestPricesResponse);

            if (allFetchedPrices.Count != 0)
            {
                var pricesWithMissingRangesFilled = PriceUtils.FillMissingRangePrices(
                    allFetchedPrices,
                    allFetchedPrices[0].Time,
                    fetchStart);
                await SavePrices(instrument, pricesWithMissingRangesFilled);
            }

            _logger.LogInformation($"First price fetch for instrument {instrumentId} finished at {DateTime.UtcNow}.");
            if (allFetchedPrices.Count > 0)
            {
                await _notificationService.SendNotificationAsync(NotificationType.NewDataAvailable, $"Price download finished for {instrument.Symbol}.");
            }
            else
            {
                await _notificationService.SendNotificationAsync(NotificationType.Info,
                    $"No prices found for {instrument.Symbol}.");
            }
        }

        /// <summary>
        /// Aggregates all fetched price response objects into a single <c>IEnumerable</c> of successfully retrieved price points.
        /// </summary>
        /// <param name="dailyPricesResponse">Daily prices Response object.</param>
        /// <param name="hourlyPricesResponse">Hourly prices Response object.</param>
        /// <param name="latestPricesResponse">Last day prices Response object.</param>
        /// <returns>A list of all retrieved prices.</returns>
        private List<PricePoint> ConcatFetchedPrices(Response<IEnumerable<PricePoint>> dailyPricesResponse,
            Response<IEnumerable<PricePoint>> hourlyPricesResponse,
            Response<IEnumerable<PricePoint>> latestPricesResponse)
        {
            var fetchedPrices = new List<PricePoint>();

            if (dailyPricesResponse.StatusCode == StatusCode.Ok)
            {
                fetchedPrices.AddRange(dailyPricesResponse.Result);
            }

            if (hourlyPricesResponse.StatusCode == StatusCode.Ok)
            {
                fetchedPrices.AddRange(hourlyPricesResponse.Result);
            }

            if (latestPricesResponse.StatusCode == StatusCode.Ok)
            {
                fetchedPrices.AddRange(latestPricesResponse.Result);
            }

            return fetchedPrices;
        }

        /// <summary>
        /// Saves price points in the database. If the price point currency and instrument currency differ, then the prices get converted beforehand.
        /// </summary>
        /// <param name="instrument">Instrument to save prices of.</param>
        /// <param name="prices">Prices to save.</param>
        /// <returns>A Task representing the asynchronous price save operation.</returns>
        private async Task SavePrices(Instrument instrument, IEnumerable<PricePoint> prices)
        {
            if (!prices.Any())
            {
                return;
            }

            var pricesToAdd = new List<InstrumentPrice>();
            var minTime = DateTime.UtcNow;
            foreach (var pricePoint in prices)
            {
                if (pricePoint.Price <= 0m)
                {
                    continue;
                }

                var price = await PriceUtils.GetConvertedPricePointPrice(_exchangeRateRepository, instrument, pricePoint);
                pricesToAdd.Add(new InstrumentPrice(pricePoint.Time, price, instrument.Id));

                if (pricePoint.Time < minTime)
                {
                    minTime = pricePoint.Time;
                }
            }

            instrument.SetTrackingFrom(minTime);
            instrument.TrackingInfo.Update(DateTime.UtcNow);
            _instrumentRepository.Update(instrument);
            await _instrumentRepository.UnitOfWork.CommitAsync();
            await _instrumentPriceRepository.BulkInsertAsync(pricesToAdd);
        }
    }
}
