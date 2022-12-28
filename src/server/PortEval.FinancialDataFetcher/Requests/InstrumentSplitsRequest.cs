using System;
using PortEval.FinancialDataFetcher.Interfaces;

namespace PortEval.FinancialDataFetcher.Requests
{
    public class InstrumentSplitsRequest : ITimeRangeRequest
    {
        public string Symbol { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
