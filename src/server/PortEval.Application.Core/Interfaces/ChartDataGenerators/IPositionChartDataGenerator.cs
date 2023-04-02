using System.Collections.Generic;
using PortEval.Application.Core.Common;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Core.Interfaces.ChartDataGenerators
{
    /// <summary>
    /// Generates position chart lines data.
    /// </summary>
    public interface IPositionChartDataGenerator
    {
        /// <summary>
        /// Generates data for position price charts.
        /// </summary>
        /// <param name="positionPriceListData">A position's price list data.</param>
        /// <param name="dateRange">Date range of the chart.</param>
        /// <param name="frequency">Chart point interval.</param>
        /// <returns>An <see cref="IEnumerable{EntityChartPointDto}"/> containing chart line points.</returns>
        public IEnumerable<EntityChartPointDto> ChartValue(PositionPriceListData positionPriceListData,
            DateRangeParams dateRange, AggregationFrequency frequency);

        /// <summary>
        /// Generates data for position profit charts.
        /// </summary>
        /// <param name="positionPriceListData">A position's price list data.</param>
        /// <param name="dateRange">Date range of the chart.</param>
        /// <param name="frequency">Chart point interval.</param>
        /// <returns>An <see cref="IEnumerable{EntityChartPointDto}"/> containing chart line points.</returns>
        public IEnumerable<EntityChartPointDto> ChartProfit(PositionPriceListData positionPriceListData,
            DateRangeParams dateRange, AggregationFrequency frequency);

        /// <summary>
        /// Generates data for position performance charts.
        /// </summary>
        /// <param name="positionPriceListData">A position's price list data.</param>
        /// <param name="dateRange">Date range of the chart.</param>
        /// <param name="frequency">Chart point interval.</param>
        /// <returns>An <see cref="IEnumerable{EntityChartPointDto}"/> containing chart line points.</returns>
        public IEnumerable<EntityChartPointDto> ChartPerformance(PositionPriceListData positionPriceListData,
            DateRangeParams dateRange, AggregationFrequency frequency);

        /// <summary>
        /// Generates data for position aggregated profit charts.
        /// </summary>
        /// <param name="positionPriceListData">A position's price list data.</param>
        /// <param name="dateRange">Date range of the chart.</param>
        /// <param name="frequency">Chart point interval.</param>
        /// <returns>An <see cref="IEnumerable{EntityChartPointDto}"/> containing chart line points.</returns>
        public IEnumerable<EntityChartPointDto> ChartAggregatedProfit(PositionPriceListData positionPriceListData,
            DateRangeParams dateRange, AggregationFrequency frequency);

        /// <summary>
        /// Generates data for position aggregated performance charts.
        /// </summary>
        /// <param name="positionPriceListData">A position's price list data.</param>
        /// <param name="dateRange">Date range of the chart.</param>
        /// <param name="frequency">Chart point interval.</param>
        /// <returns>An <see cref="IEnumerable{EntityChartPointDto}"/> containing chart line points.</returns>
        public IEnumerable<EntityChartPointDto> ChartAggregatedPerformance(PositionPriceListData positionPriceListData,
            DateRangeParams dateRange, AggregationFrequency frequency);
    }
}
