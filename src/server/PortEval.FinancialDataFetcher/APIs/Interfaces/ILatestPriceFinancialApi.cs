using PortEval.FinancialDataFetcher.Models;
using PortEval.FinancialDataFetcher.Requests;
using PortEval.FinancialDataFetcher.Responses;

namespace PortEval.FinancialDataFetcher.APIs.Interfaces
{
    /// <summary>
    /// Represents an API which can retrieve latest instrument prices.
    /// </summary>
    public interface ILatestPriceFinancialApi : IFinancialApi<LatestInstrumentPriceRequest, Response<PricePoint>>
    {
    }
}
