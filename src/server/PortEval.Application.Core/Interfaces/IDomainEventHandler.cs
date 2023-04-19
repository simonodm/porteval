using MediatR;
using PortEval.Application.Core.Common;
using PortEval.Domain.Events;

namespace PortEval.Application.Core.Interfaces;

/// <summary>
///     Represents a handler of a specific type of domain events.
/// </summary>
/// <typeparam name="T">Type of the domain event.</typeparam>
public interface IDomainEventHandler<T> : INotificationHandler<DomainEventNotificationAdapter<T>> where T : IDomainEvent
{
}