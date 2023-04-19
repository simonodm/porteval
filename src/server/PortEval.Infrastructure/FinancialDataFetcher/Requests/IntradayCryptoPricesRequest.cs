using System;
using PortEval.Application.Models.FinancialDataFetcher;

namespace PortEval.Infrastructure.FinancialDataFetcher.Requests;

/// <summary>
///     Request for intraday prices of the specified cryptocurrency.
/// </summary>
public class IntradayCryptoPricesRequest : InstrumentDataRequest, IInstrumentTimeRangeRequest
{
    /// <summary>
    ///     The interval in which to retrieve the intraday prices.
    /// </summary>
    public IntradayInterval Interval { get; set; } = IntradayInterval.OneHour;

    /// <inheritdoc />
    public DateTime From { get; set; }

    /// <inheritdoc />
    public DateTime To { get; set; }
}