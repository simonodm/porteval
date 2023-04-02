using System.Collections.Generic;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Core.Interfaces.ChartDataGenerators
{
    /// <summary>
    /// Generates instrument chart lines data.
    /// </summary>
    public interface IInstrumentChartDataGenerator
    {
        /// <summary>
        /// Generates data for instrument price charts.
        /// </summary>
        /// <param name="prices">Instrument prices sorted by ascending time.</param>
        /// <param name="dateRange">Date range of the chart.</param>
        /// <param name="frequency">Chart point interval.</param>
        /// <returns>An <see cref="IEnumerable{EntityChartPointDto}"/> containing chart line points.</returns>
        public IEnumerable<EntityChartPointDto> ChartPrices(IEnumerable<InstrumentPriceDto> prices,
            DateRangeParams dateRange, AggregationFrequency frequency);

        /// <summary>
        /// Generates data for instrument profit charts.
        /// </summary>
        /// <param name="prices">Instrument prices sorted by ascending time.</param>
        /// <param name="dateRange">Date range of the chart.</param>
        /// <param name="frequency">Chart point interval.</param>
        /// <returns>An <see cref="IEnumerable{EntityChartPointDto}"/> containing chart line points.</returns>
        public IEnumerable<EntityChartPointDto> ChartProfit(IEnumerable<InstrumentPriceDto> prices,
            DateRangeParams dateRange, AggregationFrequency frequency);

        /// <summary>
        /// Generates data for instrument performance charts.
        /// </summary>
        /// <param name="prices">Instrument prices sorted by ascending time.</param>
        /// <param name="dateRange">Date range of the chart.</param>
        /// <param name="frequency">Chart point interval.</param>
        /// <returns>An <see cref="IEnumerable{EntityChartPointDto}"/> containing chart line points.</returns>
        public IEnumerable<EntityChartPointDto> ChartPerformance(IEnumerable<InstrumentPriceDto> prices,
            DateRangeParams dateRange, AggregationFrequency frequency);

        /// <summary>
        /// Generates data for instrument aggregated profit charts.
        /// </summary>
        /// <param name="prices">Instrument prices sorted by ascending time.</param>
        /// <param name="dateRange">Date range of the chart.</param>
        /// <param name="frequency">Chart point interval.</param>
        /// <returns>An <see cref="IEnumerable{EntityChartPointDto}"/> containing chart line points.</returns>
        public IEnumerable<EntityChartPointDto> ChartAggregatedProfit(IEnumerable<InstrumentPriceDto> prices,
            DateRangeParams dateRange, AggregationFrequency frequency);

        /// <summary>
        /// Generates data for instrument aggregated performance charts.
        /// </summary>
        /// <param name="prices">Instrument prices sorted by ascending time.</param>
        /// <param name="dateRange">Date range of the chart.</param>
        /// <param name="frequency">Chart point interval.</param>
        /// <returns>An <see cref="IEnumerable{EntityChartPointDto}"/> containing chart line points.</returns>
        public IEnumerable<EntityChartPointDto> ChartAggregatedPerformance(IEnumerable<InstrumentPriceDto> prices,
            DateRangeParams dateRange, AggregationFrequency frequency);
    }
}
