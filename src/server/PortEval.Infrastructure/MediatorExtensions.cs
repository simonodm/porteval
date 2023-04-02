using MediatR;
using PortEval.Domain.Events;
using PortEval.Domain.Models.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Core.Common;

namespace PortEval.Infrastructure
{
    internal static class MediatorExtensions
    {
        /// <summary>
        /// Dispatches all domain events detected in the current context to appropriate domain event handlers.
        /// </summary>
        /// <param name="mediator">A mediator to use to dispatch the events.</param>
        /// <param name="ctx">The database context of the current operation.</param>
        /// <returns>A task representing the asynchronous dispatch operation.</returns>
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
