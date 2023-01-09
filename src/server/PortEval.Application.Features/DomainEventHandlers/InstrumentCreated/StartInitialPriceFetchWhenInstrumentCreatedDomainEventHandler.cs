using Hangfire;
using Microsoft.Extensions.Logging;
using PortEval.Application.Features.Common;
using PortEval.Application.Features.Interfaces;
using PortEval.Application.Features.Interfaces.BackgroundJobs;
using PortEval.Domain.Events;
using PortEval.Domain.Models.Enums;
using System.Threading;
using System.Threading.Tasks;

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

        public Task Handle(DomainEventNotificationAdapter<InstrumentCreatedDomainEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            _jobClient.Enqueue<IInitialPriceFetchJob>(job => job.Run(domainEvent.Instrument.Id));

            domainEvent.Instrument.SetTrackingStatus(InstrumentTrackingStatus.SearchingForPrices);

            _logger.LogInformation($"Initial price fetch job enqueued for instrument {domainEvent.Instrument.Symbol}, ID {domainEvent.Instrument.Id}.");
            return Task.CompletedTask;
        }
    }
}
