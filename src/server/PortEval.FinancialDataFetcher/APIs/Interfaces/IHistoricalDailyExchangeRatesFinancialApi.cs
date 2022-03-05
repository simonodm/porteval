using PortEval.FinancialDataFetcher.Models;
using PortEval.FinancialDataFetcher.Requests;
using PortEval.FinancialDataFetcher.Responses;
using System.Collections.Generic;

namespace PortEval.FinancialDataFetcher.APIs.Interfaces
{
    /// <summary>
    /// Represents an API client which can retrieve historical daily exchange rates.
    /// </summary>
    public interface IHistoricalDailyExchangeRatesFinancialApi : IFinancialApi<HistoricalDailyExchangeRatesRequest, Response<IEnumerable<ExchangeRates>>>
    {
    }
}
