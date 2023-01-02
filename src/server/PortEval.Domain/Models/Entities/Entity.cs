using PortEval.Domain.Events;
using System.Collections.Generic;

namespace PortEval.Domain.Models.Entities
{
    public abstract class Entity
    {
        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents?.AsReadOnly();
        private List<IDomainEvent> _domainEvents;

        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents ??= new List<IDomainEvent>();
            _domainEvents.Add(domainEvent);
        }

        public void RemoveDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents?.Remove(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
