using System;
using System.Collections.Generic;

namespace PortEval.Application.Models.FinancialDataFetcher;

/// <summary>
///     Represents exchange rates from a single currency to multiple currencies at a single point in time.
/// </summary>
public class ExchangeRates
{
    /// <summary>
    ///     Base currency.
    /// </summary>
    public string Currency { get; set; }

    /// <summary>
    ///     Time of the exchange rates.
    /// </summary>
    public DateTime Time { get; set; }

    /// <summary>
    ///     A dictionary of exchange rates, where the key represents the target currency, and the value represents the exchange
    ///     rate.
    /// </summary>
    public Dictionary<string, decimal> Rates { get; set; }
}