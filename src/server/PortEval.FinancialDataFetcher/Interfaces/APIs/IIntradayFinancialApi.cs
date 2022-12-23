using PortEval.FinancialDataFetcher.Models;
using PortEval.FinancialDataFetcher.Requests;
using PortEval.FinancialDataFetcher.Responses;
using System.Collections.Generic;

namespace PortEval.FinancialDataFetcher.Interfaces.APIs
{
    /// <summary>
    /// Represents an API which can retrieve instruments' intraday prices.
    /// </summary>
    public interface IIntradayFinancialApi : IFinancialApi<IntradayPricesRequest, Response<IEnumerable<PricePoint>>>
    {
    }
}