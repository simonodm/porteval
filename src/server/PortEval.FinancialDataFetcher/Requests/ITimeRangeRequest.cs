using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortEval.FinancialDataFetcher.Requests
{
    public interface ITimeRangeRequest
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
