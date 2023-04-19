using System;

namespace PortEval.Infrastructure.FinancialDataFetcher.Requests;

/// <summary>
///     A request for instrument splits.
/// </summary>
public class InstrumentSplitsRequest : ITimeRangeRequest
{
    /// <summary>
    ///     The ticker symbol of the instrument to retrieve splits for.
    /// </summary>
    public string Symbol { get; set; }

    /// <inheritdoc />
    public DateTime From { get; set; }

    /// <inheritdoc />
    public DateTime To { get; set; }
}