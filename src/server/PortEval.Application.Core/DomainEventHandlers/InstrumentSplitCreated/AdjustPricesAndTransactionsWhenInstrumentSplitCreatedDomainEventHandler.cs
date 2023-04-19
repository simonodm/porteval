using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Logging;
using PortEval.Application.Core.Common;
using PortEval.Application.Core.Interfaces;
using PortEval.Application.Core.Interfaces.BackgroundJobs;
using PortEval.Domain.Events;

namespace PortEval.Application.Core.DomainEventHandlers.InstrumentSplitCreated;

/// <summary>
///     A domain event handler initiating the post-split adjustment job after an instrument split is created.
/// </summary>
public class
    AdjustPricesAndTransactionsWhenInstrumentSplitCreatedDomainEventHandler : IDomainEventHandler<
        InstrumentSplitCreatedDomainEvent>
{
    private readonly IBackgroundJobClient _jobClient;
    private readonly ILogger _logger;

    /// <summary>
    ///     Initializes the domain event handler.
    /// </summary>
    public AdjustPricesAndTransactionsWhenInstrumentSplitCreatedDomainEventHandler(IBackgroundJobClient jobClient,
        ILoggerFactory loggerFactory)
    {
        _jobClient = jobClient;
        _logger = loggerFactory.CreateLogger(
            typeof(AdjustPricesAndTransactionsWhenInstrumentSplitCreatedDomainEventHandler));
    }

    /// <summary>
    ///     Handles the domain event.
    /// </summary>
    public Task Handle(DomainEventNotificationAdapter<InstrumentSplitCreatedDomainEvent> notification,
        CancellationToken cancellationToken)
    {
        _jobClient.Enqueue<ISplitPriceAndTransactionAdjustmentJob>(job => job.RunAsync());
        _logger.LogInformation("Split price and transaction adjustment job enqueued.");

        return Task.CompletedTask;
    }
}