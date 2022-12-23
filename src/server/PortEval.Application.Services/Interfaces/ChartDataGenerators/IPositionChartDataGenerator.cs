using PortEval.Application.Features.Common;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;
using System.Collections.Generic;

namespace PortEval.Application.Features.Interfaces.ChartDataGenerators
{
    /// <summary>
    /// Generates position-based chart lines data.
    /// </summary>
    public interface IPositionChartDataGenerator
    {
        /// <summary>
        /// Generates data for portfolio/position price charts.
        /// </summary>
        /// <param name="positionsPriceListData">
        /// A collection of positions' price list data, see <see cref="PositionPriceListData"/>.
        /// If this parameter contains data for more than one position, then the data is aggregated into a single line.
        /// This is done to enable portfolio line generation using the same interface.
        /// </param>
        /// <param name="dateRange">Date range of the chart.</param>
        /// <param name="frequency">Chart point interval.</param>
        /// <returns>An <see cref="IEnumerable{EntityChartPointDto}"/> containing chart line points.</returns>
        public IEnumerable<EntityChartPointDto> ChartValue(IEnumerable<PositionPriceListData> positionsPriceListData,
            DateRangeParams dateRange, AggregationFrequency frequency);

        /// <summary>
        /// Generates data for portfolio/position profit charts.
        /// </summary>
        /// <param name="positionsPriceListData">
        /// A collection of positions' price list data, see <see cref="PositionPriceListData"/>.
        /// If this parameter contains data for more than one position, then the data is aggregated into a single line.
        /// This is done to enable portfolio line generation using the same interface.
        /// </param>
        /// <param name="dateRange">Date range of the chart.</param>
        /// <param name="frequency">Chart point interval.</param>
        /// <returns>An <see cref="IEnumerable{EntityChartPointDto}"/> containing chart line points.</returns>
        public IEnumerable<EntityChartPointDto> ChartProfit(IEnumerable<PositionPriceListData> positionsPriceListData,
            DateRangeParams dateRange, AggregationFrequency frequency);

        /// <summary>
        /// Generates data for portfolio/position performance charts.
        /// </summary>
        /// <param name="positionsPriceListData">
        /// A collection of positions' price list data, see <see cref="PositionPriceListData"/>.
        /// If this parameter contains data for more than one position, then the data is aggregated into a single line.
        /// This is done to enable portfolio line generation using the same interface.
        /// </param>
        /// <param name="dateRange">Date range of the chart.</param>
        /// <param name="frequency">Chart point interval.</param>
        /// <returns>An <see cref="IEnumerable{EntityChartPointDto}"/> containing chart line points.</returns>
        public IEnumerable<EntityChartPointDto> ChartPerformance(IEnumerable<PositionPriceListData> positionsPriceListData,
            DateRangeParams dateRange, AggregationFrequency frequency);

        /// <summary>
        /// Generates data for portfolio/position aggregated profit charts.
        /// </summary>
        /// <param name="positionsPriceListData">
        /// A collection of positions' price list data, see <see cref="PositionPriceListData"/>.
        /// If this parameter contains data for more than one position, then the data is aggregated into a single line.
        /// This is done to enable portfolio line generation using the same interface.
        /// </param>
        /// <param name="dateRange">Date range of the chart.</param>
        /// <param name="frequency">Chart point interval.</param>
        /// <returns>An <see cref="IEnumerable{EntityChartPointDto}"/> containing chart line points.</returns>
        public IEnumerable<EntityChartPointDto> ChartAggregatedProfit(IEnumerable<PositionPriceListData> positionsPriceListData,
            DateRangeParams dateRange, AggregationFrequency frequency);

        /// <summary>
        /// Generates data for portfolio/position aggregated performance charts.
        /// </summary>
        /// <param name="positionsPriceListData">
        /// A collection of positions' price list data, see <see cref="PositionPriceListData"/>.
        /// If this parameter contains data for more than one position, then the data is aggregated into a single line.
        /// This is done to enable portfolio line generation using the same interface.
        /// </param>
        /// <param name="dateRange">Date range of the chart.</param>
        /// <param name="frequency">Chart point interval.</param>
        /// <returns>An <see cref="IEnumerable{EntityChartPointDto}"/> containing chart line points.</returns>
        public IEnumerable<EntityChartPointDto> ChartAggregatedPerformance(IEnumerable<PositionPriceListData> positionsPriceListData,
            DateRangeParams dateRange, AggregationFrequency frequency);
    }
}
