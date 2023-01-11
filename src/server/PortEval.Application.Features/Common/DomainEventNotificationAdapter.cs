using MediatR;
using PortEval.Domain.Events;

namespace PortEval.Application.Features.Common
{
    /// <summary>
    /// Wraps an <see cref="IDomainEvent" /> instance in a structure implementing MediatR's <see cref="INotification" /> interface.
    /// </summary>
    public class DomainEventNotificationAdapter<T> : INotification
        where T : IDomainEvent
    {
        /// <summary>
        /// The original domain event.
        /// </summary>
        public T DomainEvent { get; private set; }

        public DomainEventNotificationAdapter(T domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}
