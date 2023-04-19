using System;

namespace PortEval.Domain.Models.Entities;

/// <summary>
///     Represents the price of a financial instrument at a certain time.
/// </summary>
public class InstrumentPrice : VersionedEntity, IAggregateRoot
{
    /// <summary>
    ///     ID of the price.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Time of the price.
    /// </summary>
    public DateTime Time { get; }

    /// <summary>
    ///     Time at which the price was created in the application.
    /// </summary>
    public DateTime CreationTime { get; }

    /// <summary>
    ///     The price in instrument's currency.
    /// </summary>
    public decimal Price { get; private set; }

    /// <summary>
    ///     ID of the instrument to which this price belongs.
    /// </summary>
    public int InstrumentId { get; private set; }

    internal InstrumentPrice(int id, DateTime time, DateTime creationTime, decimal price, int instrumentId)
        : this(time, creationTime, price, instrumentId)
    {
        Id = id;
    }

    internal InstrumentPrice(DateTime time, DateTime creationTime, decimal price, int instrumentId)
    {
        if (time < PortEvalConstants.FinancialDataStartTime)
            throw new InvalidOperationException(
                $"Price time must be later than {PortEvalConstants.FinancialDataStartTime}");

        if (price <= 0)
            throw new InvalidOperationException("Instrument price must be above zero.");

        Time = time;
        CreationTime = creationTime;
        Price = price;
        InstrumentId = instrumentId;
    }

    /// <summary>
    ///     A factory method for creating an instrument price.
    /// </summary>
    /// <param name="time">Time of the price.</param>
    /// <param name="price">Price in instrument's currency.</param>
    /// <param name="instrument">A reference to the parent instrument.</param>
    /// <returns>The newly created instrument price entity.</returns>
    public static InstrumentPrice Create(DateTime time, decimal price, Instrument instrument)
    {
        return new InstrumentPrice(time, DateTime.UtcNow, price, instrument.Id);
    }

    /// <summary>
    ///     Adjusts the price for a stock split.
    /// </summary>
    /// <param name="split">The split to adjust for.</param>
    public void AdjustForSplit(InstrumentSplit split)
    {
        if (split.Time > CreationTime && split.Time > Time) Price /= split.SplitRatio.Factor;
    }

    /// <summary>
    ///     Adjusts the price following an instrument split rollback.
    /// </summary>
    /// <param name="split">The split to adjust for.</param>
    public void AdjustForSplitRollback(InstrumentSplit split)
    {
        if (split.Time > CreationTime && split.Time > Time) Price *= split.SplitRatio.Factor;
    }
}