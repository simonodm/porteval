using Hangfire;
using Microsoft.Extensions.Logging;
using PortEval.Application.Features.Interfaces;
using PortEval.Application.Features.Interfaces.BackgroundJobs;
using PortEval.Domain.Events;
using System.Threading;
using System.Threading.Tasks;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Features.DomainEventHandlers.InstrumentCreated
{
    public class StartInitialPriceFetchWhenInstrumentCreatedDomainEventHandler : IDomainEventHandler<InstrumentCreatedDomainEvent>
    {
        private readonly IBackgroundJobClient _jobClient;
        private readonly ILogger _logger;

        public StartInitialPriceFetchWhenInstrumentCreatedDomainEventHandler(IBackgroundJobClient jobClient,
            ILoggerFactory loggerFactory)
        {
            _jobClient = jobClient;
            _logger = loggerFactory.CreateLogger(typeof(StartInitialPriceFetchWhenInstrumentCreatedDomainEventHandler));
        }

        public Task Handle(InstrumentCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            _jobClient.Enqueue<IInitialPriceFetchJob>(job => job.Run(notification.Instrument.Id));

            notification.Instrument.SetTrackingStatus(InstrumentTrackingStatus.SearchingForPrices);

            _logger.LogInformation($"Initial price fetch job enqueued for instrument {notification.Instrument.Symbol}, ID {notification.Instrument.Id}.");
            return Task.CompletedTask;
        }
    }
}
