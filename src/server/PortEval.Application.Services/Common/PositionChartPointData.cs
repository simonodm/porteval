using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using System.Collections.Generic;

namespace PortEval.Application.Features.Common
{
    /// <summary>
    /// Contains the necessary data to calculate a position's chart point value at a given time.
    /// </summary>
    public class PositionChartPointData
    {
        /// <summary>
        /// Instrument price at the start of the chart.
        /// </summary>
        public InstrumentPriceDto StartPrice { get; set; }

        /// <summary>
        /// Instrument price at the time of the previous chart point.
        /// </summary>
        public InstrumentPriceDto InstrumentPriceAtRangeStart { get; set; }

        /// <summary>
        /// Instrument price at the time of the current chart point.
        /// </summary>
        public InstrumentPriceDto InstrumentPriceAtRangeEnd { get; set; }

        /// <summary>
        /// Position's transactions until the current chart point time.
        /// </summary>
        public IEnumerable<TransactionDto> TransactionsToProcess { get; set; }

        /// <summary>
        /// Represents the range between the previous chart point and the current chart point.
        /// </summary>
        public DateRangeParams DateRange { get; set; }
    }
}
