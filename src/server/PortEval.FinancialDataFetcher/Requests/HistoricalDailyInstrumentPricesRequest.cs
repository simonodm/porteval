using System;

namespace PortEval.FinancialDataFetcher.Requests
{
    /// <summary>
    /// Request for the historical daily prices of the specified symbol.
    /// </summary>
    public class HistoricalDailyInstrumentPricesRequest : InstrumentDataRequest, ITimeRangeRequest
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
