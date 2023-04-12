using PortEval.Application.Models.FinancialDataFetcher;
using PortEval.DataFetcher.Interfaces;
using System;

namespace PortEval.Infrastructure.FinancialDataFetcher.Requests
{
    /// <summary>
    /// Request for intraday prices of the specified cryptocurrency.
    /// </summary>
    public class IntradayCryptoPricesRequest : InstrumentDataRequest, ITimeRangeRequest
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public IntradayInterval Interval { get; set; } = IntradayInterval.OneHour;
    }
}
