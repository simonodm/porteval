using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Logging;
using PortEval.Application.Core.Common;
using PortEval.Application.Core.Interfaces;
using PortEval.Application.Core.Interfaces.BackgroundJobs;
using PortEval.Domain.Events;

namespace PortEval.Application.Core.DomainEventHandlers.DefaultCurrencyChanged
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
            _jobClient.Enqueue<IMissingExchangeRatesFetchJob>(job => job.RunAsync());
            _logger.LogInformation($"Missing exchange rates job enqueued after default currency change to {notification.DomainEvent.NewDefaultCurrency.Code}.");
            return Task.CompletedTask;
        }
    }
}
