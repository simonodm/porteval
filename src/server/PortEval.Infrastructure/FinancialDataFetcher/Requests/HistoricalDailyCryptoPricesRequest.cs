using System;

namespace PortEval.Infrastructure.FinancialDataFetcher.Requests;

/// <summary>
///     Request for historical daily prices of the specified cryptocurrency.
/// </summary>
public class HistoricalDailyCryptoPricesRequest : InstrumentDataRequest, IInstrumentTimeRangeRequest
{
    /// <inheritdoc />
    public DateTime From { get; set; }

    /// <inheritdoc />
    public DateTime To { get; set; }
}