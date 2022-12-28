using PortEval.FinancialDataFetcher.Models;
using PortEval.FinancialDataFetcher.Requests;
using PortEval.FinancialDataFetcher.Responses;

namespace PortEval.FinancialDataFetcher.Interfaces.APIs
{
    /// <summary>
    /// Represents an API client which can retrieve latest cryptocurrency prices.
    /// </summary>
    internal interface ILatestCryptoPriceFinancialApi : IFinancialApi<LatestCryptoPriceRequest, Response<PricePoint>>
    {
    }
}
