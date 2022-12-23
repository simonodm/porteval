using System;

namespace PortEval.Application.Features.Common
{
    /// <summary>
    /// Contains the necessary data to calculate an instrument's chart point value at a given time.
    /// </summary>
    public class InstrumentPriceChartPointData
    {
        /// <summary>
        /// Instrument price at the start of the chart.
        /// </summary>
        public decimal StartPrice { get; set; }

        /// <summary>
        /// Instrument price at <see cref="Time"/>.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Time of the chart point.
        /// </summary>
        public DateTime Time { get; set; }
    }
}
