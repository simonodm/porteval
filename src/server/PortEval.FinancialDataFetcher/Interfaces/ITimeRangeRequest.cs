using System;

namespace PortEval.FinancialDataFetcher.Interfaces
{
    public interface ITimeRangeRequest
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
