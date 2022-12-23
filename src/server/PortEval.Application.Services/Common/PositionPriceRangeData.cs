using System;
using PortEval.Application.Models.DTOs;
using System.Collections.Generic;

namespace PortEval.Application.Features.Common
{
    /// <summary>
    /// Represents a collection of a position's transactions and the corresponding instrument's price at the beginning and the end of a date range.
    /// </summary>
    public class PositionPriceRangeData
    {
        /// <summary>
        /// Position's transactions sorted by ascending time.
        /// </summary>
        public IEnumerable<TransactionDto> Transactions { get; set; }

        /// <summary>
        /// Position's instrument price at range start.
        /// </summary>
        public InstrumentPriceDto PriceAtRangeStart { get; set; }

        /// <summary>
        /// Position's instrument price at range end.
        /// </summary>
        public InstrumentPriceDto PriceAtRangeEnd { get; set; }
    }
}
