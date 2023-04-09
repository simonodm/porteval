using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PortEval.Application.Core.BackgroundJobs.Helpers;
using PortEval.Application.Core.Interfaces.BackgroundJobs;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;

namespace PortEval.Application.Core.BackgroundJobs
{
    /// <summary>
    /// Cleans up old instrument prices to maintain the following intervals:
    /// <list type="bullet">
    ///     <item>1 day for prices older than 5 days.</item>
    ///     <item>1 hour for prices between 2 and 5 days.</item>
    ///     <item>5 minutes for prices in the last 24 hours.</item>
    /// </list>
    /// </summary>
    public class InstrumentPriceCleanupJob : IInstrumentPriceCleanupJob
    {
        private readonly IInstrumentRepository _instrumentRepository;
        private readonly IInstrumentPriceRepository _instrumentPriceRepository;
        private readonly ILogger _logger;

        public InstrumentPriceCleanupJob(IInstrumentRepository instrumentRepository, IInstrumentPriceRepository instrumentPriceRepository, ILoggerFactory loggerFactory)
        {
            _instrumentRepository = instrumentRepository;
            _instrumentPriceRepository = instrumentPriceRepository;
            _logger = loggerFactory.CreateLogger(typeof(InstrumentPriceCleanupJob));
        }

        /// <summary>
        /// Starts the job.
        /// </summary>
        /// <returns>A task representing the asynchronous job processing operation.</returns>
        public async Task RunAsync()
        {
            var startTime = DateTime.UtcNow;
            _logger.LogInformation("Instrument price cleanup started.");

            var instruments = await _instrumentRepository.ListAllAsync();
            foreach (var instrument in instruments)
            {
                var prices = await _instrumentPriceRepository.ListInstrumentPricesAsync(instrument.Id);
                Cleanup(prices, startTime);
            }
            await _instrumentPriceRepository.UnitOfWork.CommitAsync();

            _logger.LogInformation("Instrument price cleanup finished.");
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
                    _instrumentPriceRepository.Delete(previousPrice);
                }

                previousPrice = price;
            }
        }
    }
}
