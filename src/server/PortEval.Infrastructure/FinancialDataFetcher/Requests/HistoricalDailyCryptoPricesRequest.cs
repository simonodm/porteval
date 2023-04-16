using System;

namespace PortEval.Infrastructure.FinancialDataFetcher.Requests
{
    /// <summary>
    /// Request for historical daily prices of the specified cryptocurrency.
    /// </summary>
    public class HistoricalDailyCryptoPricesRequest : InstrumentDataRequest, IInstrumentTimeRangeRequest
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
