using System;

namespace PortEval.FinancialDataFetcher.Requests
{
    /// <summary>
    /// A base class representing a request for data in the specified time range.
    /// </summary>
    public abstract class TimeRangeRequest : Request
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
