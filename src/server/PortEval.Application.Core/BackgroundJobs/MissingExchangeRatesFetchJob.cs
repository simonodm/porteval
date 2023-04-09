using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PortEval.Application.Core.BackgroundJobs.Helpers;
using PortEval.Application.Core.Interfaces;
using PortEval.Application.Core.Interfaces.BackgroundJobs;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Domain;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;

namespace PortEval.Application.Core.BackgroundJobs
{
    /// <summary>
    /// Retrieves missing exchange rates of the default currency to maintain the 1 day standard interval between rates.
    /// </summary>
    public class MissingExchangeRatesFetchJob : IMissingExchangeRatesFetchJob
    {
        private readonly ICurrencyRepository _currencyRepository;
        private readonly ICurrencyExchangeRateRepository _exchangeRateRepository;
        private readonly IFinancialDataFetcher _fetcher;
        private readonly ILogger _logger;

        public MissingExchangeRatesFetchJob(ICurrencyRepository currencyRepository, ICurrencyExchangeRateRepository exchangeRateRepository,
            IFinancialDataFetcher fetcher, ILoggerFactory loggerFactory)
        {
            _currencyRepository = currencyRepository;
            _exchangeRateRepository = exchangeRateRepository;
            _fetcher = fetcher;
            _logger = loggerFactory.CreateLogger(typeof(MissingExchangeRatesFetchJob));
        }

        /// <summary>
        /// Starts the job.
        /// </summary>
        /// <returns>A task representing the asynchronous job processing operation.</returns>
        public async Task RunAsync()
        {
            var currentTime = DateTime.UtcNow;
            _logger.LogInformation("Missing exchange rates job started.");

            var currencies = await _currencyRepository.ListAllAsync();
            var defaultCurrency = currencies.FirstOrDefault(c => c.IsDefault);

            if (defaultCurrency == default)
            {
                throw new OperationNotAllowedException("No default currency is set.");
            }

            var exchangeRates = await _exchangeRateRepository.ListExchangeRatesAsync(defaultCurrency.Code);

            var exchangeRateTimes = exchangeRates.Select(r => r.Time);

            var initialTime = PortEvalConstants.FinancialDataStartTime;

            var missingExchangeRates = PriceUtils.GetMissingPriceRanges(
                exchangeRateTimes,
                time => PriceUtils.GetCurrencyExchangeRateInterval(currentTime, time),
                defaultCurrency.TrackingInfo?.StartTime ?? initialTime,
                currentTime);

            foreach (var range in missingExchangeRates)
            {
                await ProcessCurrencyRange(currencies, defaultCurrency, range, currentTime);
            }

            _logger.LogInformation("Missing exchange rates job finished.");
        }

        /// <summary>
        /// Retrieves exchange rates for the given currency and time range and saves them to the database.
        /// </summary>
        /// <param name="currencies">All known currencies.</param>
        /// <param name="currency">Base currency.</param>
        /// <param name="range">Time range to retrieve data for.</param>
        /// <returns>A task representing the asynchronous exchange rate retrieval and save operations.</returns>
        private async Task ProcessCurrencyRange(IEnumerable<Currency> currencies, Currency currency, TimeRange range, DateTime startTime)
        {
            var fetchResult = await _fetcher.GetHistoricalDailyExchangeRatesAsync(currency.Code, range.From, range.To);

            var currenciesList = currencies.ToList();
            int i = 0;
            var newExchangeRates = new List<CurrencyExchangeRate>();
            var minTime = DateTime.UtcNow;
            foreach (var exchangeRateData in fetchResult)
            {
                if (exchangeRateData.Time < range.From || exchangeRateData.Time > range.To) continue;

                foreach (var (targetCurrencyCode, exchangeRate) in exchangeRateData.Rates)
                {
                    var targetCurrency = currenciesList.FirstOrDefault(c => c.Code == targetCurrencyCode);
                    if (targetCurrency == null || currency.Code == targetCurrency.Code) continue;

                    newExchangeRates.Add(CurrencyExchangeRate.Create(exchangeRateData.Time, exchangeRate, currency, targetCurrency));
                    if (exchangeRateData.Time < minTime)
                    {
                        minTime = exchangeRateData.Time;
                    }
                }

                i++;
                // limited to 1000 days per insert to preserve application memory
                if (i < 1000) continue;

                await _exchangeRateRepository.BulkUpsertAsync(newExchangeRates);
                newExchangeRates.Clear();
                i = 0;
            }

            if (currency.TrackingInfo == null && fetchResult.Any())
            {
                currency.SetTrackingFrom(minTime);
                currency.TrackingInfo.Update(startTime);
                currency.IncreaseVersion();
                _currencyRepository.Update(currency);
                await _currencyRepository.UnitOfWork.CommitAsync();
            }

            if (newExchangeRates.Count > 0)
            {
                await _exchangeRateRepository.BulkUpsertAsync(newExchangeRates);
                newExchangeRates.Clear();
            }
        }
    }
}
