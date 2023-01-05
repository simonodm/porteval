using System;
using MediatR;
using PortEval.Domain.Models.Entities;
using PortEval.Application.Features.Common;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Domain.Events;

namespace PortEval.Infrastructure
{
    internal static class MediatorExtensions
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, PortEvalDbContext ctx)
        {
            var domainEntities = ctx.ChangeTracker
                .Entries<Entity>()
                .Where(entry => entry.Entity.DomainEvents != null && entry.Entity.DomainEvents.Any());

            var notifications = domainEntities
                .SelectMany(entry => entry.Entity.DomainEvents)
                .Select(CreateNotificationFromDomainEvent)
                .ToList();

            foreach (var entity in domainEntities)
            {
                entity.Entity.ClearDomainEvents();
            }

            foreach (var notification in notifications)
            {
                await mediator.Publish(notification);
            }
        }

        private static INotification CreateNotificationFromDomainEvent(IDomainEvent domainEvent)
        {
            var genericDispatcherType = typeof(DomainEventNotificationAdapter<>).MakeGenericType(domainEvent.GetType());
            return (INotification)Activator.CreateInstance(genericDispatcherType, domainEvent);
        }
    }
}
