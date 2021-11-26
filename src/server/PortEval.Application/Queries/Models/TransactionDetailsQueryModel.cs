﻿using System;

namespace PortEval.Application.Queries.Models
{
    internal class TransactionDetailsQueryModel
    {
        public DateTime Time { get; set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
        public decimal InstrumentPriceAtRangeStart { get; set; }
        public decimal InstrumentPriceAtRangeEnd { get; set; }
    }
}
