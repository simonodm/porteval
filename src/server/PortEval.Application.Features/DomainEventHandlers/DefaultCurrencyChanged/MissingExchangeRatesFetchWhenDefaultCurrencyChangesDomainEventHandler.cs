using Hangfire;
using Microsoft.Extensions.Logging;
using PortEval.Application.Features.Interfaces.BackgroundJobs;
using PortEval.Domain.Events;
using System.Threading;
using System.Threading.Tasks;
using PortEval.Application.Features.Common;
using PortEval.Application.Features.Interfaces;

namespace PortEval.Application.Features.DomainEventHandlers.DefaultCurrencyChanged
{
    public class MissingExchangeRatesFetchWhenDefaultCurrencyChangesDomainEventHandler : IDomainEventHandler<DefaultCurrencyChangedDomainEvent>
    {
        private readonly IBackgroundJobClient _jobClient;
        private readonly ILogger _logger;

        public MissingExchangeRatesFetchWhenDefaultCurrencyChangesDomainEventHandler(IBackgroundJobClient jobClient,
            ILoggerFactory loggerFactory)
        {
            _jobClient = jobClient;
            _logger = loggerFactory.CreateLogger(
                typeof(MissingExchangeRatesFetchWhenDefaultCurrencyChangesDomainEventHandler));
        }

        public Task Handle(DomainEventNotificationAdapter<DefaultCurrencyChangedDomainEvent> notification, CancellationToken cancellationToken)
        {
            _jobClient.Enqueue<IMissingExchangeRatesFetchJob>(job => job.Run());
            _logger.LogInformation($"Missing exchange rates job enqueued after default currency change to {notification.DomainEvent.NewDefaultCurrency.Code}.");
            return Task.CompletedTask;
        }
    }
}
