using PortEval.FinancialDataFetcher.Models;
using PortEval.FinancialDataFetcher.Requests;
using PortEval.FinancialDataFetcher.Responses;
using System.Collections.Generic;

namespace PortEval.FinancialDataFetcher.Interfaces.APIs
{
    /// <summary>
    /// Represents an API client which can retrieve intraday cryptocurrency prices.
    /// </summary>
    public interface IIntradayCryptoFinancialApi : IFinancialApi<IntradayCryptoPricesRequest, Response<IEnumerable<PricePoint>>>
    {
    }
}
