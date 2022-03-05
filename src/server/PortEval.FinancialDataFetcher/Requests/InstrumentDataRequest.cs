using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortEval.Domain.Models.Enums;

namespace PortEval.FinancialDataFetcher.Requests
{
    public class InstrumentDataRequest : IRequest
    {
        public string Symbol { get; set; }
        public string CurrencyCode { get; set; }
        public InstrumentType Type { get; set; }
    }
}
