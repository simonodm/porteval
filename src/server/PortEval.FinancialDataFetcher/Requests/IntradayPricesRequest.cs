using PortEval.FinancialDataFetcher.Models;

namespace PortEval.FinancialDataFetcher.Requests
{
    /// <summary>
    /// Request for the intraday prices of the supplied symbol.
    /// </summary>
    public class IntradayPricesRequest : TimeRangeRequest
    {
        public IntradayInterval Interval { get; set; } = IntradayInterval.OneHour;
    }
}
