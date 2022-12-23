using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using System.Collections.Generic;

namespace PortEval.Application.Features.Common
{
    public class PositionChartPointData
    {
        public InstrumentPriceDto StartPrice { get; set; }
        public InstrumentPriceDto InstrumentPriceAtRangeStart { get; set; }
        public InstrumentPriceDto InstrumentPriceAtRangeEnd { get; set; }
        public IEnumerable<TransactionDto> TransactionsToProcess { get; set; }
        public DateRangeParams DateRange { get; set; }
    }
}
