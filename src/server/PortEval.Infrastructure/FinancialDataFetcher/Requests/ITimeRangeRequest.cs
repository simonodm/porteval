using System;
using PortEval.DataFetcher.Interfaces;

namespace PortEval.Infrastructure.FinancialDataFetcher.Requests
{
    public interface ITimeRangeRequest : IRequest
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
