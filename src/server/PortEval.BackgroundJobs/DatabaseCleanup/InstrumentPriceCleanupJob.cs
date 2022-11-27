using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PortEval.Domain.Models.Entities;
using PortEval.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using PortEval.Application.Services.Interfaces.BackgroundJobs;
using PortEval.Application.Services.Interfaces.Repositories;

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
        public async Task Run()
        {
            var startTime = DateTime.UtcNow;
            _logger.LogInformation($"Instrument price cleanup started at {startTime}.");

            var instruments = await _instrumentRepository.ListAllAsync();
            foreach(var instrument in instruments)
            {
                var prices = await _instrumentPriceRepository.ListInstrumentPricesAsync(instrument.Id);
                Cleanup(prices, startTime);
            }
            await _instrumentPriceRepository.UnitOfWork.CommitAsync();

            _logger.LogInformation($"Instrument price cleanup finished at {DateTime.UtcNow}.");
        }

        /// <summary>
        /// Removes overabundant prices.
        /// </summary>
        /// <param name="prices">Prices to clean up.</param>
        /// <param name="startTime">Job start time used as a base for price interval evaluation.</param>
        private async void Cleanup(IEnumerable<InstrumentPrice> prices, DateTime startTime)
        {
            InstrumentPrice currentAnchor = null;
            InstrumentPrice previousPrice = null;

            var removeTasks = new List<Task>();

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
                    removeTasks.Add(_instrumentPriceRepository.DeleteAsync(previousPrice.InstrumentId, previousPrice.Id));
                }

                previousPrice = price;
            }

            await Task.WhenAll(removeTasks);
        }
    }
}
