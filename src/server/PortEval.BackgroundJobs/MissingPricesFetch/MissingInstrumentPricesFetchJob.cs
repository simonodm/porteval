using Microsoft.Extensions.Logging;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.FinancialDataFetcher;
using PortEval.FinancialDataFetcher.Models;
using Microsoft.EntityFrameworkCore;
using EFCore.BulkExtensions;
using PortEval.FinancialDataFetcher.Responses;
using PortEval.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Services.Interfaces.BackgroundJobs;

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
        private readonly PortEvalDbContext _context;
        private readonly PriceFetcher _fetcher;
        private readonly ILogger _logger;

        public MissingInstrumentPricesFetchJob(PortEvalDbContext context, PriceFetcher fetcher, ILoggerFactory loggerFactory)
        {
            _fetcher = fetcher;
            _context = context;
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

            var instruments = await _context.Instruments.AsNoTracking().ToListAsync();

            foreach (var instrument in instruments)
            {
                if (!instrument.IsTracked) continue;

                var trackablePrices = await _context.InstrumentPrices
                    .Where(p => p.InstrumentId == instrument.Id)
                    .Where(p => p.Time >= instrument.TrackingInfo.StartTime)
                    .Select(p => p.Time)
                    .ToListAsync();

                var missingRanges = PriceUtils.GetMissingPriceRanges(
                    trackablePrices, PriceUtils.GetInstrumentPriceInterval,
                    instrument.TrackingInfo.StartTime,
                    currentTime);
                foreach (var range in missingRanges)
                {
                    await ProcessInstrumentRange(instrument, currentTime, range);
                }

                instrument.TrackingInfo.Update(currentTime);
                _context.Instruments.Update(instrument);
            }

            await _context.SaveChangesAsync(); // persist tracking info updates
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
            if (timeDifference < TimeSpan.FromDays(5))
            {
                var intradayInterval = timeDifference <= TimeSpan.FromDays(1)
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

            var priceAtRangeStart = await _context.InstrumentPrices
                .AsNoTracking()
                .Where(p => p.InstrumentId == instrument.Id)
                .Where(p => p.Time <= range.From)
                .OrderByDescending(p => p.Time)
                .FirstOrDefaultAsync();

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
                    var price = await PriceUtils.GetConvertedPricePointPrice(_context, instrument, pricePoint);
                    pricesToAdd.Add(new InstrumentPrice(pricePoint.Time, price, instrument.Id));
                }
                catch (OperationNotAllowedException ex)
                {
                    _logger.LogError(ex.Message);
                }
            }

            await _context.BulkInsertAsync(pricesToAdd);
        }
    }
}
