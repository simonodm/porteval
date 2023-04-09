using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Logging;
using PortEval.Application.Core.Common;
using PortEval.Application.Core.Interfaces;
using PortEval.Application.Core.Interfaces.BackgroundJobs;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Domain.Events;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Core.DomainEventHandlers.InstrumentCreated
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

            _jobClient.Enqueue<IInitialPriceFetchJob>(job => job.RunAsync(domainEvent.Instrument.Id));

            _logger.LogInformation($"Initial price fetch job enqueued for instrument {domainEvent.Instrument.Symbol}, ID {domainEvent.Instrument.Id}.");
        }
    }
}
