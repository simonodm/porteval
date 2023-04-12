using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using System.Collections.Generic;

namespace PortEval.Application.Core.Common
{
    /// <summary>
    /// Represents a collection of a position's transactions and the corresponding instrument's price at the beginning and the end of a date range.
    /// </summary>
    public class PositionPriceRangeData
    {
        /// <summary>
        /// ID of the position.
        /// </summary>
        public int PositionId { get; set; }

        /// <summary>
        /// All position's transactions until the end of <see cref="DateRange"/> sorted by ascending time.
        /// </summary>
        public IEnumerable<TransactionDto> Transactions { get; set; }

        /// <summary>
        /// Position's instrument price at <see cref="DateRange"/> start.
        /// </summary>
        public InstrumentPriceDto PriceAtRangeStart { get; set; }

        /// <summary>
        /// Position's instrument price at <see cref="DateRange"/> end.
        /// </summary>
        public InstrumentPriceDto PriceAtRangeEnd { get; set; }

        /// <summary>
        /// Date range of the data.
        /// </summary>
        public DateRangeParams DateRange { get; set; }
    }
}
