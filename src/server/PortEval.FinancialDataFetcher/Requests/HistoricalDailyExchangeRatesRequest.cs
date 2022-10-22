using System;

namespace PortEval.FinancialDataFetcher.Requests
{
    /// <summary>
    /// Request for the historical daily exchange rates of the specified currency.
    /// </summary>
    public class HistoricalDailyExchangeRatesRequest : CurrencyDataRequest, ITimeRangeRequest
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
