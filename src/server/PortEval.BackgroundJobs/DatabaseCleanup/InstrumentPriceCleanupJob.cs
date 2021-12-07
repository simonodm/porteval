using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PortEval.Domain.Models.Entities;
using PortEval.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PortEval.BackgroundJobs.DatabaseCleanup
{
    /// <summary>
    /// Cleans up old instrument prices to maintain the following intervals:
    /// <list type="bullet">
    ///     <item>1 day for prices older than 5 days.</item>
    ///     <item>1 hour for prices between 2 and 5 days.</item>
    ///     <item>5 minutes for prices in the last 24 hours.</item>
    /// </list>
    /// </summary>
    public class InstrumentPriceCleanupJob
    {
        private readonly PortEvalDbContext _context;
        private readonly ILogger _logger;

        public InstrumentPriceCleanupJob(PortEvalDbContext context, ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger(typeof(InstrumentPriceCleanupJob));
        }

        /// <summary>
        /// Starts the job.
        /// </summary>
        /// <returns>A task representing the asynchronous job processing operation.</returns>
        public async Task Run()
        {
            var startTime = DateTime.Now;
            _logger.LogInformation($"Instrument price cleanup started at {startTime}.");

            var instruments = await _context.Instruments.AsNoTracking().ToListAsync();
            foreach(var instrument in instruments)
            {
                var prices = await _context.InstrumentPrices.AsNoTracking().Where(p => p.InstrumentId == instrument.Id).ToListAsync();
                Cleanup(prices, startTime);
            }
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Instrument price cleanup finished at {DateTime.Now}.");
        }

        /// <summary>
        /// Removes overabundant prices.
        /// </summary>
        /// <param name="prices">Prices to clean up.</param>
        /// <param name="startTime">Job start time used as a base for price interval evaluation.</param>
        private void Cleanup(IEnumerable<InstrumentPrice> prices, DateTime startTime)
        {
            InstrumentPrice currentAnchor = null;
            InstrumentPrice previousPrice = null;

            foreach (var price in prices.OrderBy(p => p.Time))
            {
                currentAnchor ??= price;
                var interval = PriceUtils.GetInstrumentPriceInterval(startTime, price.Time);

                if (currentAnchor.Time + interval < price.Time)
                {
                    currentAnchor = previousPrice;
                }

                if (previousPrice != null && currentAnchor != previousPrice)
                {
                    _context.Remove(previousPrice);
                }

                previousPrice = price;
            }
        }
    }
}
