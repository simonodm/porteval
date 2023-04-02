using PortEval.DataFetcher.Interfaces;
using System;
using PortEval.Application.Models.FinancialDataFetcher;

namespace PortEval.Infrastructure.FinancialDataFetcher.Requests
{
    /// <summary>
    /// Request for the intraday prices of the supplied instrument.
    /// </summary>
    public class IntradayInstrumentPricesRequest : InstrumentDataRequest, ITimeRangeRequest
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public IntradayInterval Interval { get; set; } = IntradayInterval.OneHour;
    }
}
