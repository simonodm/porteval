using PortEval.DataFetcher.Interfaces;
using System;

namespace PortEval.Infrastructure.FinancialDataFetcher.Requests
{
    public class InstrumentSplitsRequest : ITimeRangeRequest
    {
        public string Symbol { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
