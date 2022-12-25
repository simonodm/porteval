using PortEval.Application.Features.Interfaces.Calculators;
using PortEval.Application.Features.Interfaces.ChartDataGenerators;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;
using System.Collections.Generic;

namespace PortEval.Application.Features.Common.ChartDataGenerators
{
    /// <inheritdoc cref="IPositionChartDataGenerator"/>
    public class PositionChartDataGenerator : PositionBasedChartDataGenerator, IPositionChartDataGenerator
    {
        public PositionChartDataGenerator(IPositionValueCalculator valueCalculator,
            IPositionProfitCalculator profitCalculator, IPositionPerformanceCalculator performanceCalculator)
            : base(valueCalculator, profitCalculator, performanceCalculator)
        {
        }

        /// <inheritdoc />
        public IEnumerable<EntityChartPointDto> ChartValue(PositionPriceListData positionPriceListData, DateRangeParams dateRange,
            AggregationFrequency frequency)
        {
            return ChartValue(new[] { positionPriceListData }, dateRange, frequency);
        }

        /// <inheritdoc />
        public IEnumerable<EntityChartPointDto> ChartProfit(PositionPriceListData positionPriceListData, DateRangeParams dateRange,
            AggregationFrequency frequency)
        {
            return ChartProfit(new[] { positionPriceListData }, dateRange, frequency);
        }

        /// <inheritdoc />
        public IEnumerable<EntityChartPointDto> ChartPerformance(PositionPriceListData positionPriceListData, DateRangeParams dateRange,
            AggregationFrequency frequency)
        {
            return ChartPerformance(new[] { positionPriceListData }, dateRange, frequency);
        }

        /// <inheritdoc />
        public IEnumerable<EntityChartPointDto> ChartAggregatedProfit(PositionPriceListData positionPriceListData, DateRangeParams dateRange,
            AggregationFrequency frequency)
        {
            return ChartAggregatedProfit(new[] { positionPriceListData }, dateRange, frequency);
        }

        /// <inheritdoc />
        public IEnumerable<EntityChartPointDto> ChartAggregatedPerformance(PositionPriceListData positionPriceListData, DateRangeParams dateRange,
            AggregationFrequency frequency)
        {
            return ChartAggregatedPerformance(new[] { positionPriceListData }, dateRange, frequency);
        }
    }
}
