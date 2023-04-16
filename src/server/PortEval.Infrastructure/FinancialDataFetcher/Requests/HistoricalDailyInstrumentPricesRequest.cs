using PortEval.DataFetcher.Interfaces;
using System;

namespace PortEval.Infrastructure.FinancialDataFetcher.Requests
{
    /// <summary>
    /// Request for the historical daily prices of the specified symbol.
    /// </summary>
    public class HistoricalDailyInstrumentPricesRequest : InstrumentDataRequest, IInstrumentTimeRangeRequest
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
