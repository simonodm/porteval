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
        private readonly IPriceFetcher _fetcher;
        private readonly ILogger _logger;

        public MissingInstrumentPricesFetchJob(IInstrumentRepository instrumentRepository, IInstrumentPriceRepository instrumentPriceRepository,
            ICurrencyExchangeRateRepository exchangeRateRepository, IPriceFetcher fetcher, ILoggerFactory loggerFactory)
        {
            _instrumentRepository = instrumentRepository;
            _instrumentPriceRepository = instrumentPriceRepository;
            _exchangeRateRepository = exchangeRateRepository;
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

                var prices = await _instrumentPriceRepository.ListInstrumentPricesAsync(instrument.Id);
                var priceTimes = prices
                    .Where(p => p.Time >= instrument.TrackingInfo.StartTime)
                    .OrderBy(p => p.Time)                    
                    .Select(p => p.Time);

                var missingRanges = PriceUtils.GetMissingPriceRanges(
                    priceTimes, PriceUtils.GetInstrumentPriceInterval,
                    instrument.TrackingInfo.StartTime,
                    currentTime);
                var splitMissingRanges = SplitRangesAtIntervalChanges(missingRanges, currentTime);

                foreach (var range in splitMissingRanges)
                {
                    await ProcessInstrumentRange(instrument, currentTime, range);
                }

                instrument.TrackingInfo.Update(currentTime);
                _instrumentRepository.Update(instrument);
            }

            await _instrumentRepository.UnitOfWork.CommitAsync();
            _logger.LogInformation($"Missing prices fetch finished at {DateTime.UtcNow}.");
        }

        /// <summary>
        /// Retrieves and saves prices of a single instrument in the given time range. If the retrieved prices currency and instrument currency differ, then
        /// the prices are converted to instrument's currency beforehand.
        /// </summary>
        /// <param name="instrument">Instrument to retrieve prices for.</param>
        /// <param name="currentTime">Base time to use for interval requirements.</param>
        /// <param name="range">Time range of missing prices.</param>
        /// <returns>A task representing the asynchronous retrieval, conversion and save operations.</returns>
        private async Task ProcessInstrumentRange(Instrument instrument, DateTime currentTime, TimeRange range)
        {
            Response<IEnumerable<PricePoint>> fetchResult;

            var timeDifference = currentTime - range.To;
            if (timeDifference < PriceUtils.FiveDays)
            {
                var intradayInterval = timeDifference < PriceUtils.OneDay
                    ? IntradayInterval.FiveMinutes
                    : IntradayInterval.OneHour;
                fetchResult = await _fetcher.GetIntradayPrices(instrument, range.From, range.To,
                    intradayInterval);
            }
            else
            {
                fetchResult = await _fetcher.GetHistoricalDailyPrices(instrument, range.From, range.To);
            }

            if (fetchResult.StatusCode != StatusCode.Ok) return;

            var priceAtRangeStart = await _instrumentPriceRepository.FindPriceAtAsync(instrument.Id, range.From);

            var rangeStartPricePoint = new PricePoint
            {
                Price = priceAtRangeStart.Price,
                CurrencyCode = instrument.CurrencyCode,
                Symbol = instrument.Symbol,
                Time = range.From
            };

            var newPricePoints = PriceUtils.FillMissingRangePrices(
                fetchResult.Result.Prepend(rangeStartPricePoint),
                range.From,
                range.To);

            var pricesToAdd = new List<InstrumentPrice>();
            foreach (var pricePoint in newPricePoints)
            {
                if (!(pricePoint.Time > range.From & pricePoint.Time < range.To)) continue;

                if (pricePoint.Price <= 0m)
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

            await _instrumentPriceRepository.BulkInsertAsync(pricesToAdd);
        }

        private IEnumerable<TimeRange> SplitRangesAtIntervalChanges(IEnumerable<TimeRange> ranges, DateTime baseTime)
        {
            var queue = new Queue<TimeRange>();
            foreach (var range in ranges)
            {
                queue.Enqueue(range);
            }

            var result = new List<TimeRange>();
            while(queue.Count > 0)
            {
                var range = queue.Dequeue();

                if (RangeShouldSplitAtInterval(range, PriceUtils.FiveDays, baseTime))
                {
                    queue.Enqueue(new TimeRange
                    {
                        From = range.From,
                        To = baseTime - PriceUtils.FiveDays
                    });
                    queue.Enqueue(new TimeRange
                    {
                        From = baseTime - PriceUtils.FiveDays,
                        To = baseTime
                    });
                }
                else if (RangeShouldSplitAtInterval(range, PriceUtils.OneDay, baseTime))
                {
                    queue.Enqueue(new TimeRange
                    {
                        From = range.From,
                        To = baseTime - PriceUtils.OneDay
                    });
                    queue.Enqueue(new TimeRange
                    {
                        From = baseTime - PriceUtils.OneDay,
                        To = baseTime
                    });
                }
                else
                {
                    result.Add(range);
                }
            }

            return result;
        }

        private bool RangeShouldSplitAtInterval(TimeRange range, TimeSpan interval, DateTime baseTime)
        {
            var timeDifferentFrom = baseTime - range.From;
            var timeDifferenceTo = baseTime - range.To;
            return timeDifferentFrom > interval && timeDifferenceTo < interval;
        }
    }
}
