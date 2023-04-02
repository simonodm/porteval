using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PortEval.Application.Core.Extensions;
using PortEval.Application.Core.Interfaces;
using PortEval.Application.Core.Interfaces.BackgroundJobs;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs.Enums;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Core.BackgroundJobs
{
    /// <summary>
    /// Retrieves the latest available prices of all existing instruments. Each price gets rounded down to the nearest 5 minutes.
    /// </summary>
    public class LatestPricesFetchJob : InstrumentPriceFetchJobBase, ILatestPricesFetchJob
    {
        private readonly IInstrumentRepository _instrumentRepository;
        private readonly IInstrumentPriceRepository _instrumentPriceRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger _logger;

        public LatestPricesFetchJob(IFinancialDataFetcher fetcher, IInstrumentRepository instrumentRepository,
            IInstrumentPriceRepository instrumentPriceRepository,
            INotificationService notificationService, ILoggerFactory loggerFactory) : base(fetcher)
        {
            _instrumentRepository = instrumentRepository;
            _instrumentPriceRepository = instrumentPriceRepository;
            _notificationService = notificationService;
            _logger = loggerFactory.CreateLogger(typeof(LatestPricesFetchJob));
        }

        /// <summary>
        /// Starts the job.
        /// </summary>
        /// <returns>A task representing the asynchronous job processing operation.</returns>
        public async Task Run()
        {
            var startTime = DateTime.UtcNow;
            _logger.LogInformation("Running latest price fetch job.");
            var instruments = await _instrumentRepository.ListAllAsync();

            foreach (var instrument in instruments)
            {
                if (instrument.TrackingStatus != InstrumentTrackingStatus.Tracked)
                {
                    continue;
                }

                var fetcherResponse = await FetchLatestPrice(instrument);
                if (fetcherResponse == null)
                {
                    continue;
                }

                try
                {
                    var price = InstrumentPrice.Create(startTime.RoundDown(TimeSpan.FromMinutes(5)),
                        fetcherResponse.Price, instrument);
                    _instrumentPriceRepository.Add(price);

                    instrument.TrackingInfo.Update(startTime);
                    instrument.IncreaseVersion();
                    _instrumentRepository.Update(instrument);
                }
                catch (OperationNotAllowedException ex)
                {
                    _logger.LogError(ex.Message);
                }

            }

            await _instrumentRepository.UnitOfWork.CommitAsync();
            _logger.LogInformation("Finished latest price fetch job.");
            await _notificationService.SendNotificationAsync(NotificationType.NewDataAvailable);
        }
    }
}
