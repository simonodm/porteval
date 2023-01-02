using Hangfire;
using Microsoft.Extensions.Logging;
using PortEval.Application.Features.Interfaces;
using PortEval.Application.Features.Interfaces.BackgroundJobs;
using PortEval.Domain.Events;
using System.Threading;
using System.Threading.Tasks;

namespace PortEval.Application.Features.DomainEventHandlers.InstrumentSplitCreated
{
    public class AdjustPricesAndTransactionsWhenInstrumentSplitCreatedDomainEventHandler : IDomainEventHandler<InstrumentSplitCreatedDomainEvent>
    {
        private readonly IBackgroundJobClient _jobClient;
        private readonly ILogger _logger;

        public AdjustPricesAndTransactionsWhenInstrumentSplitCreatedDomainEventHandler(IBackgroundJobClient jobClient,
            ILoggerFactory loggerFactory)
        {
            _jobClient = jobClient;
            _logger = loggerFactory.CreateLogger(
                typeof(AdjustPricesAndTransactionsWhenInstrumentSplitCreatedDomainEventHandler));
        }

        public Task Handle(InstrumentSplitCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            _jobClient.Enqueue<ISplitPriceAndTransactionAdjustmentJob>(job => job.Run());
            _logger.LogInformation("Split price and transaction adjustment job enqueued.");

            return Task.CompletedTask;
        }
    }
}
