using System;
using PortEval.Domain.Events;
using PortEval.Domain.Models.ValueObjects;

namespace PortEval.Domain.Models.Entities;

/// <summary>
///     Represents a fiat currency.
/// </summary>
public class Currency : VersionedEntity, IAggregateRoot
{
    /// <summary>
    ///     Currency's 3-letter code.
    /// </summary>
    public string Code { get; private set; }

    /// <summary>
    ///     Name of the currency.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    ///     Currency symbol.
    /// </summary>
    public string Symbol { get; private set; }

    /// <summary>
    ///     Whether this currency is the application's default currency.
    /// </summary>
    public bool IsDefault { get; private set; }

    /// <summary>
    ///     Information on whether the currency is tracked by the automatic exchange rate retrieval mechanism.
    /// </summary>
    public TrackingInformation TrackingInfo { get; private set; }

    internal Currency(string code, string name, string symbol, bool isDefault = false)
    {
        Code = code;
        Name = name;
        Symbol = symbol;
        IsDefault = isDefault;
    }

    /// <summary>
    ///     A factory method for creating a currency.
    /// </summary>
    /// <param name="code">A 3-letter code of the currency.</param>
    /// <param name="name">Name of the currency.</param>
    /// <param name="symbol">Currency symbol.</param>
    /// <param name="isDefault">Whether this is the application's default currency.</param>
    /// <returns>The newly created currency entity.</returns>
    public static Currency Create(string code, string name, string symbol, bool isDefault = false)
    {
        return new Currency(code, name, symbol, isDefault);
    }

    internal void SetAsDefault()
    {
        IsDefault = true;
        AddDomainEvent(new DefaultCurrencyChangedDomainEvent(this));
    }

    internal void UnsetDefault()
    {
        IsDefault = false;
    }

    /// <summary>
    ///     Sets the currency as tracked from the specified date and time.
    /// </summary>
    /// <param name="time">Date and time from which the currency is tracked.</param>
    public void SetTrackingFrom(DateTime time)
    {
        TrackingInfo = new TrackingInformation(time, DateTime.UtcNow);
    }
}