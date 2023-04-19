using PortEval.DataFetcher.Interfaces;

namespace PortEval.Infrastructure.FinancialDataFetcher.Requests;

/// <summary>
///     A base class for data fetch requests for currency data.
/// </summary>
public abstract class CurrencyDataRequest : IRequest
{
    /// <summary>
    ///     3-letter code of the currency, the data of which is being retrieved.
    /// </summary>
    public string CurrencyCode { get; set; }
}