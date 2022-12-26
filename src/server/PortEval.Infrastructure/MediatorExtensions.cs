using MediatR;
using PortEval.Domain.Models.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Infrastructure
{
    internal static class MediatorExtensions
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, PortEvalDbContext ctx)
        {
            var domainEntities = ctx.ChangeTracker
                .Entries<Entity>()
                .Where(entry => entry.Entity.DomainEvents != null && entry.Entity.DomainEvents.Any());

            var domainEvents = domainEntities
                .SelectMany(entry => entry.Entity.DomainEvents)
                .ToList();

            foreach (var entity in domainEntities)
            {
                entity.Entity.ClearDomainEvents();
            }

            foreach (var domainEvent in domainEvents)
            {
                await mediator.Publish(domainEvent);
            }
        }
    }
}
