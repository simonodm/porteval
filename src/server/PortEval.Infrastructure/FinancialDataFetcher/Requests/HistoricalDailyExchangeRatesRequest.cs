using System;

namespace PortEval.Infrastructure.FinancialDataFetcher.Requests;

/// <summary>
///     Request for the historical daily exchange rates of the specified currency.
/// </summary>
public class HistoricalDailyExchangeRatesRequest : CurrencyDataRequest, ITimeRangeRequest
{
    /// <inheritdoc />
    public DateTime From { get; set; }

    /// <inheritdoc />
    public DateTime To { get; set; }
}