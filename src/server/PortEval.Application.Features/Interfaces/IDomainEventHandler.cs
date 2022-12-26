using MediatR;
using PortEval.Domain.Events;

namespace PortEval.Application.Features.Interfaces
{
    public interface IDomainEventHandler<in T> : INotificationHandler<T> where T : IDomainEvent
    {
    }
}
