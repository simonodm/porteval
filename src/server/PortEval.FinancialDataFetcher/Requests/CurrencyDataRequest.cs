using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortEval.FinancialDataFetcher.Requests
{
    public class CurrencyDataRequest : IRequest
    {
        public string CurrencyCode { get; set; }
    }
}
