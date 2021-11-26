using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PortEval.Domain.Models.Entities;
using PortEval.FinancialDataFetcher;
using PortEval.FinancialDataFetcher.Models;
using PortEval.FinancialDataFetcher.Responses;
using PortEval.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.BackgroundJobs.MissingPricesFetch
{
    /// <summary>
    /// Retrieves missing exchange rates of the default currency to maintain the 1 day standard interval between rates.
    /// </summary>
    public class MissingExchangeRatesFetchJob
    {
        private readonly PortEvalDbContext _context;
        private readonly PriceFetcher _fetcher;
        private readonly ILogger _logger;

        public MissingExchangeRatesFetchJob(PortEvalDbContext context, PriceFetcher fetcher, ILoggerFactory loggerFactory)
        {
            _context = context;
            _fetcher = fetcher;
            _logger = loggerFactory.CreateLogger(typeof(MissingExchangeRatesFetchJob));
        }

        /// <summary>
        /// Starts the job.
        /// </summary>
        /// <returns>A task representing the asynchronous job processing operation.</returns>
        public async Task Run()
        {
            var currentTime = DateTime.Now;
            _logger.LogInformation($"Missing exchange rates job started at {currentTime}.");

            var currencies = await _context.Currencies.AsNoTracking().ToListAsync();
            var defaultCurrency = await _context.Currencies.AsNoTracking().FirstOrDefaultAsync(c => c.IsDefault);

            if (defaultCurrency == default)
            {
                throw new ApplicationException("No default currency is set.");
            }

            var exchangeRateTimes = await _context.CurrencyExchangeRates.AsNoTracking()
                .Where(er => er.CurrencyFromCode == defaultCurrency.Code)
                .Select(er => er.Time)
                .ToListAsync();

            var initialTime = new DateTime(2000, 1, 1);

            var missingExchangeRates = PriceUtils.GetMissingPriceRanges(
                exchangeRateTimes,
                PriceUtils.GetCurrencyExchangeRateInterval,
                defaultCurrency.TrackingInfo?.StartTime ?? initialTime,
                currentTime);

            foreach (var range in missingExchangeRates)
            {
                await ProcessCurrencyRange(currencies, defaultCurrency, range);
            }

            _logger.LogInformation($"Missing exchange rates job finished at {DateTime.Now}.");
        }

        /// <summary>
        /// Retrieves exchange rates for the given currency and time range and saves them to the database.
        /// </summary>
        /// <param name="currencies">All known currencies.</param>
        /// <param name="currency">Base currency.</param>
        /// <param name="range">Time range to retrieve data for.</param>
        /// <returns>A task representing the asynchronous exchange rate retrieval and save operations.</returns>
        private async Task ProcessCurrencyRange(IEnumerable<Currency> currencies, Currency currency, TimeRange range)
        {
            var fetchResult = await _fetcher.GetHistoricalDailyExchangeRates(currency.Code, range.From, range.To);
            if (fetchResult.StatusCode != StatusCode.Ok) return;

            if(currency.TrackingInfo == null)
            {
                await SetTrackingInfo(currency, fetchResult);
            }

            var currenciesList = currencies.ToList();
            int i = 0;
            var newExchangeRates = new List<CurrencyExchangeRate>();
            foreach (var exchangeRateData in fetchResult.Result)
            {
                if (exchangeRateData.Time < range.From || exchangeRateData.Time > range.To) continue;

                foreach (var (targetCurrency, exchangeRate) in exchangeRateData.Rates)
                {
                    if (currenciesList.FirstOrDefault(c => c.Code == targetCurrency) == null || currency.Code == targetCurrency) continue;

                    newExchangeRates.Add(new CurrencyExchangeRate(exchangeRateData.Time, exchangeRate, currency.Code, targetCurrency));
                }

                i++;
                // limited to 1000 days per insert to preserve application memory
                if (i < 1000) continue;
                
                await _context.BulkInsertAsync(newExchangeRates);
                newExchangeRates.Clear();
                i = 0;
            }

            if(newExchangeRates.Count > 0)
            {
                await _context.BulkInsertAsync(newExchangeRates);
                newExchangeRates.Clear();
            }
        }

        private async Task SetTrackingInfo(Currency currency, Response<IEnumerable<ExchangeRates>> fetchResponse)
        {
            if(fetchResponse.Result.Any())
            {
                currency.SetTrackingFrom(fetchResponse.Result.OrderBy(er => er.Time).First().Time);
                _context.Update(currency);
                await _context.SaveChangesAsync();
                _context.Entry(currency).State = EntityState.Detached; // Update() enables tracking on selected currency, we do not desire that for performance reasons
            }
        }
    }
}
