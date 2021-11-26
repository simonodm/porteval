using PortEval.FinancialDataFetcher.Models;
using PortEval.FinancialDataFetcher.Requests;
using PortEval.FinancialDataFetcher.Responses;

namespace PortEval.FinancialDataFetcher.APIs.Interfaces
{
    /// <summary>
    /// Represents an API which can retrieve latest exchange rates.
    /// </summary>
    public interface ILatestExchangeRatesFinancialApiClient : IFinancialApiClient<LatestExchangeRatesRequest, Response<ExchangeRates>>
    {
    }
}
