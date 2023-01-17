using Hangfire;
using Microsoft.Extensions.Logging;
using PortEval.Application.Features.Common;
using PortEval.Application.Features.Interfaces;
using PortEval.Application.Features.Interfaces.BackgroundJobs;
using PortEval.Domain.Events;
using PortEval.Domain.Models.Enums;
using System.Threading;
using System.Threading.Tasks;
using PortEval.Application.Features.Interfaces.Repositories;

namespace PortEval.Application.Features.DomainEventHandlers.InstrumentCreated
{
    public class StartInitialPriceFetchWhenInstrumentCreatedDomainEventHandler : IDomainEventHandler<InstrumentCreatedDomainEvent>
    {
        private readonly IBackgroundJobClient _jobClient;
        private readonly IInstrumentRepository _instrumentRepository;
        private readonly ILogger _logger;

        public StartInitialPriceFetchWhenInstrumentCreatedDomainEventHandler(IBackgroundJobClient jobClient,
            IInstrumentRepository instrumentRepository, ILoggerFactory loggerFactory)
        {
            _jobClient = jobClient;
            _instrumentRepository = instrumentRepository;
            _logger = loggerFactory.CreateLogger(typeof(StartInitialPriceFetchWhenInstrumentCreatedDomainEventHandler));
        }

        public async Task Handle(DomainEventNotificationAdapter<InstrumentCreatedDomainEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            domainEvent.Instrument.SetTrackingStatus(InstrumentTrackingStatus.SearchingForPrices);
            _instrumentRepository.Update(domainEvent.Instrument);
            await _instrumentRepository.UnitOfWork.CommitAsync();

            _jobClient.Enqueue<IInitialPriceFetchJob>(job => job.Run(domainEvent.Instrument.Id));

            _logger.LogInformation($"Initial price fetch job enqueued for instrument {domainEvent.Instrument.Symbol}, ID {domainEvent.Instrument.Id}.");
        }
    }
}
