using System;
using PortEval.FinancialDataFetcher.Models;

namespace PortEval.FinancialDataFetcher.Requests
{
    /// <summary>
    /// Request for the intraday prices of the supplied symbol.
    /// </summary>
    public class IntradayPricesRequest : InstrumentDataRequest, ITimeRangeRequest
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public IntradayInterval Interval { get; set; } = IntradayInterval.OneHour;
    }
}
