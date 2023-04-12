using Hangfire;
using Microsoft.Extensions.Logging;
using PortEval.Application.Core.Common;
using PortEval.Application.Core.DomainEventHandlers.InstrumentSplitCreated;
using PortEval.Application.Core.Interfaces;
using PortEval.Application.Core.Interfaces.BackgroundJobs;
using PortEval.Domain.Events;
using System.Threading;
using System.Threading.Tasks;

namespace PortEval.Application.Core.DomainEventHandlers.InstrumentSplitRollbackRequested
{
    public class AdjustPricesAndTransactionsWhenInstrumentSplitRolledBackDomainEventHandler : IDomainEventHandler<InstrumentSplitRollbackRequestedDomainEvent>
    {
        private readonly IBackgroundJobClient _jobClient;
        private readonly ILogger _logger;

        public AdjustPricesAndTransactionsWhenInstrumentSplitRolledBackDomainEventHandler(IBackgroundJobClient jobClient,
            ILoggerFactory loggerFactory)
        {
            _jobClient = jobClient;
            _logger = loggerFactory.CreateLogger(
                typeof(AdjustPricesAndTransactionsWhenInstrumentSplitCreatedDomainEventHandler));
        }

        public Task Handle(DomainEventNotificationAdapter<InstrumentSplitRollbackRequestedDomainEvent> notification, CancellationToken cancellationToken)
        {
            _jobClient.Enqueue<ISplitPriceAndTransactionAdjustmentJob>(job => job.RunAsync());
            _logger.LogInformation("Split price and transaction adjustment job enqueued.");

            return Task.CompletedTask;
        }
    }
}
