using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Logging;
using PortEval.Application.Core.Common;
using PortEval.Application.Core.Interfaces;
using PortEval.Application.Core.Interfaces.BackgroundJobs;
using PortEval.Domain.Events;

namespace PortEval.Application.Core.DomainEventHandlers.DefaultCurrencyChanged;

/// <summary>
///     A domain event handler initiating the missing exchange rates fetch job after the default currency of the
///     application changes.
/// </summary>
public class
    MissingExchangeRatesFetchWhenDefaultCurrencyChangesDomainEventHandler : IDomainEventHandler<
        DefaultCurrencyChangedDomainEvent>
{
    private readonly IBackgroundJobClient _jobClient;
    private readonly ILogger _logger;

    /// <summary>
    ///     Initializes the domain event handler.
    /// </summary>
    public MissingExchangeRatesFetchWhenDefaultCurrencyChangesDomainEventHandler(IBackgroundJobClient jobClient,
        ILoggerFactory loggerFactory)
    {
        _jobClient = jobClient;
        _logger = loggerFactory.CreateLogger(
            typeof(MissingExchangeRatesFetchWhenDefaultCurrencyChangesDomainEventHandler));
    }

    /// <summary>
    ///     Handles the domain event.
    /// </summary>
    public Task Handle(DomainEventNotificationAdapter<DefaultCurrencyChangedDomainEvent> notification,
        CancellationToken cancellationToken)
    {
        _jobClient.Enqueue<IMissingExchangeRatesFetchJob>(job => job.RunAsync());
        _logger.LogInformation(
            $"Missing exchange rates job enqueued after default currency change to {notification.DomainEvent.NewDefaultCurrency.Code}.");
        return Task.CompletedTask;
    }
}