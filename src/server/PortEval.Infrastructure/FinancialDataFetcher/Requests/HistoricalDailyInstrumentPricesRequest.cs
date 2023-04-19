using System;

namespace PortEval.Infrastructure.FinancialDataFetcher.Requests;

/// <summary>
///     Request for the historical daily prices of the specified symbol.
/// </summary>
public class HistoricalDailyInstrumentPricesRequest : InstrumentDataRequest, IInstrumentTimeRangeRequest
{
    /// <inheritdoc />
    public DateTime From { get; set; }

    /// <inheritdoc />
    public DateTime To { get; set; }
}