using System.Collections.Generic;
using PortEval.Domain.Events;

namespace PortEval.Domain.Models.Entities;

/// <summary>
///     A base class for domain entities.
/// </summary>
public abstract class Entity
{
    /// <summary>
    ///     A read-only list of domain events emitted by the entity.
    /// </summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents?.AsReadOnly();

    private List<IDomainEvent> _domainEvents;

    /// <summary>
    ///     Emits a domain event based in this entity.
    /// </summary>
    /// <param name="domainEvent">Domain event to emit.</param>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents ??= new List<IDomainEvent>();
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    ///     Clears the domain events of this entity.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}