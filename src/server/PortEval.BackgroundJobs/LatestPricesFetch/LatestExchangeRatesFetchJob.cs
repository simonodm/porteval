using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Application.Services.Extensions;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.FinancialDataFetcher;
using PortEval.FinancialDataFetcher.Models;
using PortEval.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Services.Interfaces.BackgroundJobs;

namespace PortEval.BackgroundJobs.LatestPricesFetch
{
    /// <summary>
    /// Retrieves the latest available exchange rates of the default currency. Each exchange rate gets rounded down to the nearest hour.
    /// </summary>
    public class LatestExchangeRatesFetchJob : ILatestExchangeRatesFetchJob
    {
        private readonly PortEvalDbContext _context;
        private readonly PriceFetcher _fetcher;
        private readonly ILogger _logger;

        public LatestExchangeRatesFetchJob(PortEvalDbContext context, PriceFetcher fetcher, ILoggerFactory loggerFactory)
        {
            _context = context;
            _fetcher = fetcher;
            _logger = loggerFactory.CreateLogger(typeof(LatestExchangeRatesFetchJob));
        }

        /// <summary>
        /// Starts the job.
        /// </summary>
        /// <returns>A task representing the asynchronous job processing operation.</returns>
        public async Task Run()
        {
            var startTime = DateTime.UtcNow;
            _logger.LogInformation($"Latest exchange rates fetch job started at {startTime}.");

            var currencies = await _context.Currencies.AsNoTracking().ToListAsync();
            var defaultCurrency = currencies.FirstOrDefault(c => c.IsDefault);
            if (defaultCurrency == default)
            {
                throw new ApplicationException("No default currency has been set.");
            }

            if (defaultCurrency.TrackingInfo == null)
            {
                return;
            }

            var exchangeRatesResponse = await _fetcher.GetLatestExchangeRates(defaultCurrency.Code);
            if (exchangeRatesResponse.StatusCode == StatusCode.Ok)
            {
                foreach (var (targetCurrency, exchangeRate) in exchangeRatesResponse.Result.Rates)
                {
                    if (currencies.FirstOrDefault(c => c.Code == targetCurrency) == null || defaultCurrency.Code == targetCurrency) continue;

                    _context.CurrencyExchangeRates.Add(
                        new CurrencyExchangeRate(startTime.RoundDown(TimeSpan.FromHours(1)), exchangeRate, defaultCurrency.Code, targetCurrency));
                }
            }

            defaultCurrency.TrackingInfo.Update(startTime);
            _context.Update(defaultCurrency);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Latest exchange rates fetch job finished at {DateTime.UtcNow}.");
        }
    }
}
