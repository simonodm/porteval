using PortEval.FinancialDataFetcher.Models;
using PortEval.FinancialDataFetcher.Requests;
using PortEval.FinancialDataFetcher.Responses;
using System.Collections.Generic;

namespace PortEval.FinancialDataFetcher.APIs.Interfaces
{
    /// <summary>
    /// Represents an API client which can retrieve historical daily instrument prices.
    /// </summary>
    public interface IHistoricalDailyFinancialApiClient : IFinancialApiClient<HistoricalDailyInstrumentPricesRequest, Response<IEnumerable<PricePoint>>>
    {
    }
}
