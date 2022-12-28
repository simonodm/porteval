using System;

namespace PortEval.FinancialDataFetcher.Interfaces
{
    public interface ITimeRangeRequest : IRequest
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
