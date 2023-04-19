using PortEval.Domain.Models.Entities;

namespace PortEval.Domain.Events;

/// <summary>
///     A domain event representing a change executed on an existing transaction.
/// </summary>
public class TransactionChangedDomainEvent : IDomainEvent
{
    /// <summary>
    ///     The transaction being modified.
    /// </summary>
    public Transaction Transaction { get; init; }

    /// <summary>
    ///     The parent position of the modified transaction.
    /// </summary>
    public Position Position { get; init; }

    /// <summary>
    ///     Initializes the domain event.
    /// </summary>
    /// <param name="transaction">The transaction being modified.</param>
    /// <param name="position">The parent position of the modified transaction.</param>
    public TransactionChangedDomainEvent(Transaction transaction, Position position)
    {
        Transaction = transaction;
        Position = position;
    }
}