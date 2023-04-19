using PortEval.DataFetcher.Interfaces;

namespace PortEval.Infrastructure.FinancialDataFetcher.Requests;

/// <summary>
///     A base class for requests for some instrument's data.
/// </summary>
public abstract class InstrumentDataRequest : IRequest
{
    /// <summary>
    ///     The ticker symbol of the instrument to retrieve data for.
    /// </summary>
    public string Symbol { get; set; }

    /// <summary>
    ///     The currency code of the instrument.
    /// </summary>
    public string CurrencyCode { get; set; }
}