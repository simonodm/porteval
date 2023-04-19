using PortEval.Domain.Models.Entities;

namespace PortEval.Domain.Events;

/// <summary>
///     A domain event representing addition of a transaction to a position.
/// </summary>
public class TransactionAddedToPositionDomainEvent : IDomainEvent
{
    /// <summary>
    ///     The transaction being added.
    /// </summary>
    public Transaction Transaction { get; init; }

    /// <summary>
    ///     The position to which the transaction is being added.
    /// </summary>
    public Position Position { get; init; }

    /// <summary>
    ///     Initializes the domain event.
    /// </summary>
    /// <param name="transaction">The transaction being added.</param>
    /// <param name="position">The position to which the transaction is being added.</param>
    public TransactionAddedToPositionDomainEvent(Transaction transaction, Position position)
    {
        Transaction = transaction;
        Position = position;
    }
}