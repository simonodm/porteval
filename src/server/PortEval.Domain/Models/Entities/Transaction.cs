using System;
using PortEval.Domain.Exceptions;

namespace PortEval.Domain.Models.Entities;

/// <summary>
///     Represents a financial transaction executed on a position.
/// </summary>
public class Transaction : Entity
{
    /// <summary>
    ///     ID of the transaction.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Time at which the transaction was executed.
    /// </summary>
    public DateTime Time { get; private set; }

    /// <summary>
    ///     Time at which the transaction was created in the application.
    /// </summary>
    public DateTime CreationTime { get; }

    /// <summary>
    ///     Transaction amount.
    /// </summary>
    public decimal Amount { get; private set; }

    /// <summary>
    ///     Transaction price per 1 unit.
    /// </summary>
    public decimal Price { get; private set; }

    /// <summary>
    ///     Transaction note.
    /// </summary>
    public string Note { get; private set; }

    /// <summary>
    ///     ID of the parent position.
    /// </summary>
    public int PositionId { get; private set; }

    /// <summary>
    ///     A reference to the parent position.
    /// </summary>
    public Position Position { get; }

    internal Transaction(int id, int positionId, DateTime time, DateTime creationTime,
        decimal amount, decimal price, string note = "") : this(positionId, time, creationTime, amount, price, note)
    {
        Id = id;
    }

    internal Transaction(int positionId, DateTime time, DateTime creationTime, decimal amount, decimal price,
        string note = "")
    {
        if (time < PortEvalConstants.FinancialDataStartTime)
            throw new OperationNotAllowedException(
                $"Transaction time must be later than {PortEvalConstants.FinancialDataStartTime}");

        if (amount == 0)
            throw new OperationNotAllowedException("Transaction amount cannot be zero.");

        if (price <= 0)
            throw new OperationNotAllowedException("Transaction price must be above zero.");

        PositionId = positionId;
        Time = time;
        CreationTime = creationTime;
        Amount = amount;
        Price = price;
        Note = note;
    }

    /// <summary>
    ///     A factory method for creating transactions.
    /// </summary>
    /// <param name="positionId">ID of the parent position.</param>
    /// <param name="time">Time at which the transaction was executed.</param>
    /// <param name="amount">Transaction amount.</param>
    /// <param name="price">Transaction price per 1 unit.</param>
    /// <param name="note">Transaction note.</param>
    /// <returns>The newly created transaction entity.</returns>
    public static Transaction Create(int positionId, DateTime time, decimal amount, decimal price, string note = "")
    {
        return new Transaction(positionId, time, DateTime.UtcNow, amount, price, note);
    }

    /// <summary>
    ///     Changes the time of the transaction.
    /// </summary>
    /// <param name="time">The new transaction time.</param>
    public void SetTime(DateTime time)
    {
        Time = time;
    }

    /// <summary>
    ///     Changes the amount of the transaction.
    /// </summary>
    /// <param name="amount">The new transaction amount.</param>
    public void SetAmount(decimal amount)
    {
        Amount = amount;
    }

    /// <summary>
    ///     Changes the price of the transaction.
    /// </summary>
    /// <param name="price">The new price of the transaction.</param>
    public void SetPrice(decimal price)
    {
        Price = price;
    }

    /// <summary>
    ///     Changes the note of the transaction.
    /// </summary>
    /// <param name="note">The new transaction note.</param>
    public void SetNote(string note)
    {
        Note = note;
    }

    /// <summary>
    ///     Adjusts the transaction for an instrument split.
    /// </summary>
    /// <param name="split">The split to adjust for.</param>
    public void AdjustForSplit(InstrumentSplit split)
    {
        if (split.Time > CreationTime && split.Time > Time)
        {
            Amount *= split.SplitRatio.Factor;
            Price /= split.SplitRatio.Factor;
        }
    }

    /// <summary>
    ///     Adjusts the transaction following an instrument split rollback.
    /// </summary>
    /// <param name="split">The split to adjust for.</param>
    public void AdjustForSplitRollback(InstrumentSplit split)
    {
        if (split.Time > CreationTime && split.Time > Time)
        {
            Amount /= split.SplitRatio.Factor;
            Price *= split.SplitRatio.Factor;
        }
    }
}