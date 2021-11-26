using PortEval.Domain.Models.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Exceptions;
using PortEval.FinancialDataFetcher;
using PortEval.FinancialDataFetcher.Models;
using System;
using System.Threading.Tasks;
using PortEval.Infrastructure;
using PortEval.Application.Services.Extensions;

namespace PortEval.BackgroundJobs.LatestPricesFetch
{
    /// <summary>
    /// Retrieves the latest available prices of all existing instruments. Each price gets rounded down to the nearest 5 minutes.
    /// </summary>
    public class LatestPricesFetchJob
    {
        private readonly PriceFetcher _fetcher;
        private readonly PortEvalDbContext _context;
        private readonly ILogger _logger;

        public LatestPricesFetchJob(PriceFetcher fetcher, PortEvalDbContext context, ILoggerFactory loggerFactory)
        {
            _fetcher = fetcher;
            _context = context;
            _logger = loggerFactory.CreateLogger(typeof(LatestPricesFetchJob));
        }

        /// <summary>
        /// Starts the job.
        /// </summary>
        /// <returns>A task representing the asynchronous job processing operation.</returns>
        public async Task Run()
        {
            var startTime = DateTime.Now;
            _logger.LogInformation($"Running latest price fetch job at {startTime}.");
            var instruments = await _context.Instruments.AsNoTracking().ToListAsync();

            foreach (var instrument in instruments)
            {
                var fetcherResponse = await _fetcher.GetLatestInstrumentPrice(instrument.Symbol);
                if (fetcherResponse.StatusCode != StatusCode.Ok) continue;

                try
                {
                    var pricePoint = fetcherResponse.Result;
                    var price = await PriceUtils.GetConvertedPricePointPrice(_context, instrument,
                        pricePoint);
                    _context.InstrumentPrices.Add(new InstrumentPrice(startTime.RoundDown(TimeSpan.FromMinutes(5)), price, instrument.Id));
                }
                catch (OperationNotAllowedException ex)
                {
                    _logger.LogError(ex.Message);
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Finished latest price fetch job at {DateTime.Now}.");
        }
    }
}
