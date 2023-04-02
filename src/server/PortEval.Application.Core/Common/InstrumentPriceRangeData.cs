using PortEval.Application.Models.QueryParams;

namespace PortEval.Application.Core.Common
{
    /// <summary>
    /// Contains the necessary data to calculate an instrument's metrics in a given range.
    /// </summary>
    public class InstrumentPriceRangeData
    {
        /// <summary>
        /// Instrument price at the start of the range defined in <see cref="DateRange"/>
        /// </summary>
        public decimal PriceAtRangeStart { get; set; }

        /// <summary>
        /// Instrument price at the end of the range defined in <see cref="DateRange"/>.
        /// </summary>
        public decimal PriceAtRangeEnd { get; set; }

        /// <summary>
        /// Time of the chart point.
        /// </summary>
        public DateRangeParams DateRange { get; set; }
    }
}
