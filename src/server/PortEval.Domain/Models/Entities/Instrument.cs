using System;
using PortEval.Domain.Events;
using PortEval.Domain.Models.Enums;
using PortEval.Domain.Models.ValueObjects;

namespace PortEval.Domain.Models.Entities;

/// <summary>
///     Represents a financial instrument.
/// </summary>
public class Instrument : VersionedEntity, IAggregateRoot
{
    /// <summary>
    ///     ID of the instrument.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Name of the instrument.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    ///     Ticker symbol of the instrument.
    /// </summary>
    public string Symbol { get; private set; }

    /// <summary>
    ///     The ticker symbol of the stock exchange at which the instrument is traded.
    /// </summary>
    public string Exchange { get; private set; }

    /// <summary>
    ///     The type of the instrument.
    /// </summary>
    public InstrumentType Type { get; private set; }

    /// <summary>
    ///     The code of the currency in which the instrument is traded.
    /// </summary>
    public string CurrencyCode { get; private set; }

    /// <summary>
    ///     A user-defined note.
    /// </summary>
    public string Note { get; private set; }

    /// <summary>
    ///     Specifies whether this instrument is tracked by the automatic price retrieval mechanism.
    /// </summary>
    public InstrumentTrackingStatus TrackingStatus { get; private set; }

    /// <summary>
    ///     Information about the automated price retrieval tracking of the instrument.
    /// </summary>
    public TrackingInformation TrackingInfo { get; private set; }

    internal Instrument(int id, string name, string symbol, string exchange, InstrumentType type, string currencyCode,
        string note) : this(name, symbol, exchange, type, currencyCode, note)
    {
        Id = id;
    }

    internal Instrument(string name, string symbol, string exchange, InstrumentType type, string currencyCode,
        string note)
    {
        Name = name;
        Symbol = symbol;
        Exchange = exchange;
        Type = type;
        CurrencyCode = currencyCode;
        Note = note;
        TrackingStatus = InstrumentTrackingStatus.Created;
    }

    /// <summary>
    ///     A factory method for creating an instrument.
    /// </summary>
    /// <param name="name">The name of the instrument.</param>
    /// <param name="symbol">The ticker symbol of the instrument.</param>
    /// <param name="exchange">The stock exchange at which the instrument is traded.</param>
    /// <param name="type">The type of the instrument.</param>
    /// <param name="currencyCode">The code of the currency in which the instrument is traded.</param>
    /// <param name="note">A user-defined note.</param>
    /// <returns>The newly created instrument entity.</returns>
    public static Instrument Create(string name, string symbol, string exchange, InstrumentType type,
        string currencyCode, string note)
    {
        var instrument = new Instrument(name, symbol, exchange, type, currencyCode, note);
        instrument.AddDomainEvent(new InstrumentCreatedDomainEvent(instrument));

        return instrument;
    }

    /// <summary>
    ///     Renames the instrument.
    /// </summary>
    /// <param name="name">The new name of the instrument.</param>
    public void Rename(string name)
    {
        Name = name;
    }

    /// <summary>
    ///     Changes the stock exchange at which the instrument is traded.
    /// </summary>
    /// <param name="exchange">The new stock exchange ticker symbol.</param>
    public void SetExchange(string exchange)
    {
        Exchange = exchange;
    }

    /// <summary>
    ///     Changes the note of the instrument.
    /// </summary>
    /// <param name="note">The new instrument note.</param>
    public void SetNote(string note)
    {
        Note = note;
    }

    /// <summary>
    ///     Sets instrument as tracked from the specified date and time.
    /// </summary>
    /// <param name="startTime">The time of the first automatically retrieved price of the instrument.</param>
    /// <param name="trackedSince">The time at which the instrument started being tracked.</param>
    public void SetTrackingFrom(DateTime startTime, DateTime? trackedSince = null)
    {
        TrackingStatus = InstrumentTrackingStatus.Tracked;
        TrackingInfo = new TrackingInformation(startTime, trackedSince ?? DateTime.UtcNow);
    }

    /// <summary>
    ///     Changes the tracking status of the instrument.
    /// </summary>
    /// <param name="trackingStatus">The new tracking status.</param>
    public void SetTrackingStatus(InstrumentTrackingStatus trackingStatus)
    {
        TrackingStatus = trackingStatus;
    }
}