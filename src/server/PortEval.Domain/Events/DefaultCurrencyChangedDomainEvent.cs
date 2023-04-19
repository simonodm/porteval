using PortEval.Domain.Models.Entities;

namespace PortEval.Domain.Events;

/// <summary>
///     A domain event representing a change in the application's default currency.
/// </summary>
public class DefaultCurrencyChangedDomainEvent : IDomainEvent
{
    /// <summary>
    ///     The new default currency of the application.
    /// </summary>
    public Currency NewDefaultCurrency { get; init; }

    /// <summary>
    ///     Initializes the domain event.
    /// </summary>
    /// <param name="newDefaultCurrency">The new default currency.</param>
    public DefaultCurrencyChangedDomainEvent(Currency newDefaultCurrency)
    {
        NewDefaultCurrency = newDefaultCurrency;
    }
}