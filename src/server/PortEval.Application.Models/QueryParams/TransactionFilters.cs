using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortEval.Application.Models.QueryParams
{
    public class TransactionFilters
    {
        public int? PortfolioId { get; set; }
        public int? PositionId { get; set; }
        public int? InstrumentId { get; set; }
    }
}
