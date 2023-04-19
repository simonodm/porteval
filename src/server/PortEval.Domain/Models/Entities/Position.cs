using System;
using System.Collections.Generic;
using System.Linq;
using PortEval.Domain.Events;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Enums;

namespace PortEval.Domain.Models.Entities;

/// <summary>
///     Represents an investment position.
/// </summary>
public class Position : VersionedEntity, IAggregateRoot
{
    /// <summary>
    ///     ID of the position.
    /// </summary>
    public int Id { get; }

    /// <summary>
    ///     Position note.
    /// </summary>
    public string Note { get; private set; }

    /// <summary>
    ///     ID of the parent portfolio.
    /// </summary>
    public int PortfolioId { get; private set; }

    /// <summary>
    ///     ID of the instrument represented by the position.
    /// </summary>
    public int InstrumentId { get; private set; }

    /// <summary>
    ///     A read-only collection of position's transactions.
    /// </summary>
    public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();

    private readonly List<Transaction> _transactions = new();

    internal Position(int id, int portfolioId, int instrumentId, string note) : this(portfolioId, instrumentId, note)
    {
        Id = id;
    }

    internal Position(int portfolioId, int instrumentId, string note)
    {
        PortfolioId = portfolioId;
        InstrumentId = instrumentId;
        Note = note;
    }

    /// <summary>
    ///     A factory method for creating positions.
    /// </summary>
    /// <param name="portfolio">A reference to the parent portfolio.</param>
    /// <param name="instrument">A reference to the instrument represented by the position.</param>
    /// <param name="note">Position note.</param>
    /// <returns>The newly created position entity.</returns>
    /// <exception cref="OperationNotAllowedException">Thrown if the instrument is an index.</exception>
    public static Position Create(Portfolio portfolio, Instrument instrument, string note)
    {
        if (instrument.Type == InstrumentType.Index)
            throw new OperationNotAllowedException("Cannot open a position for an index.");

        return new Position(portfolio.Id, instrument.Id, note);
    }

    /// <summary>
    ///     Finds a position transaction by ID.
    /// </summary>
    /// <param name="transactionId">ID of the transaction to find.</param>
    /// <returns>The found transaction entity if it exists, <c>null</c> otherwise.</returns>
    public Transaction FindTransaction(int transactionId)
    {
        return _transactions.FirstOrDefault(t => t.Id == transactionId);
    }

    /// <summary>
    ///     Adds a transaction to the position.
    /// </summary>
    /// <param name="amount">Transaction amount.</param>
    /// <param name="price">Transaction price per 1 unit.</param>
    /// <param name="time">Time of the transaction.</param>
    /// <param name="note">Transaction note.</param>
    /// <returns>The added transaction entity.</returns>
    /// <exception cref="OperationNotAllowedException">
    ///     Thrown if this transaction would result in the position's amount falling
    ///     below zero.
    /// </exception>
    public Transaction AddTransaction(decimal amount, decimal price, DateTime time, string note = "")
    {
        var transaction = Transaction.Create(Id, time, amount, price, note);

        if (GetAmountAt(time) + transaction.Amount < 0)
            throw new OperationNotAllowedException(
                $"Failed to add transaction to position {Id}: position amount cannot fall below zero.");

        _transactions.Add(transaction);
        AddDomainEvent(new TransactionAddedToPositionDomainEvent(transaction, this));
        return transaction;
    }

    /// <summary>
    ///     Updates a position transaction.
    /// </summary>
    /// <param name="id">ID of the transaction.</param>
    /// <param name="amount">New transaction amount.</param>
    /// <param name="price">New transaction price per 1 unit.</param>
    /// <param name="time">New time of the transaction.</param>
    /// <param name="note">New transaction note.</param>
    /// <returns>The updated transaction entity.</returns>
    /// <exception cref="ItemNotFoundException">Thrown if a transaction with the specified ID does not exist on this position.</exception>
    /// <exception cref="OperationNotAllowedException">
    ///     Thrown if the transaction change would result in the position's amount
    ///     falling below zero.
    /// </exception>
    public Transaction UpdateTransaction(int id, decimal amount, decimal price, DateTime time, string note)
    {
        var transaction = FindTransaction(id);
        if (transaction == null) throw new ItemNotFoundException($"Transaction {id} not found in position {Id}.");

        if ((transaction.Time > time && GetAmountAt(time) + amount < 0)
            || (transaction.Time <= time && GetAmountAt(time) - transaction.Amount + amount < 0))
            throw new OperationNotAllowedException("Position amount cannot fall below zero.");

        transaction.SetTime(time);
        transaction.SetAmount(amount);
        transaction.SetPrice(price);
        transaction.SetNote(note);

        AddDomainEvent(new TransactionChangedDomainEvent(transaction, this));
        return transaction;
    }

    /// <summary>
    ///     Removes a transaction from this position.
    /// </summary>
    /// <param name="transactionId">ID of the transaction to remove.</param>
    /// <exception cref="ItemNotFoundException">Thrown if a transaction with the specified ID does not exist on this position.</exception>
    /// <exception cref="OperationNotAllowedException">
    ///     Thrown if removal of the transaction would result in the position's
    ///     amount falling below zero.
    /// </exception>
    public void RemoveTransaction(int transactionId)
    {
        var transaction = FindTransaction(transactionId);
        if (transaction == null)
            throw new ItemNotFoundException($"Position {Id} does not contain transaction {transactionId}.");

        // It is necessary to check whether removing the transaction causes position amount to fall below zero at any point after the transaction.
        var amount = GetAmountAt(transaction.Time) - transaction.Amount;
        foreach (var existingTransaction in _transactions.OrderBy(t => t.Time).Where(t => t.Time > transaction.Time))
        {
            amount += existingTransaction.Amount;
            if (amount < 0)
                throw new OperationNotAllowedException(
                    $"Failed to remove transaction {transactionId}: position amount cannot fall below zero.");
        }

        _transactions.Remove(transaction);
    }

    /// <summary>
    ///     Changes the note of the position.
    /// </summary>
    /// <param name="note">The new note.</param>
    public void SetNote(string note)
    {
        Note = note;
    }

    private decimal GetAmountAt(DateTime time)
    {
        return _transactions.Where(t => t.Time <= time).Sum(t => t.Amount);
    }
}