using System;

namespace PortEval.Application.Services.Queries.Models
{
    public class TransactionDetailsQueryModel
    {
        public DateTime Time { get; set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
        public decimal InstrumentPriceAtRangeStart { get; set; }
        public decimal InstrumentPriceAtRangeEnd { get; set; }
    }
}
