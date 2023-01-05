using MediatR;
using PortEval.Application.Features.Common;
using PortEval.Domain.Events;

namespace PortEval.Application.Features.Interfaces
{
    public interface IDomainEventHandler<T> : INotificationHandler<DomainEventNotificationAdapter<T>> where T : IDomainEvent
    {
    }
}
