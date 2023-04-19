using MediatR;
using PortEval.Domain.Events;

namespace PortEval.Application.Core.Common;

/// <summary>
///     Wraps an <see cref="IDomainEvent" /> instance in a structure implementing MediatR's <see cref="INotification" />
///     interface.
/// </summary>
public class DomainEventNotificationAdapter<T> : INotification
    where T : IDomainEvent
{
    /// <summary>
    ///     The original domain event.
    /// </summary>
    public T DomainEvent { get; private set; }

    /// <summary>
    ///     Initializes the adapter with the specified domain event.
    /// </summary>
    /// <param name="domainEvent">Domain event wrapper by this adapter.</param>
    public DomainEventNotificationAdapter(T domainEvent)
    {
        DomainEvent = domainEvent;
    }
}