using PortEval.Application.Core.Interfaces.Calculators;
using PortEval.Application.Core.Interfaces.ChartDataGenerators;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;
using System.Collections.Generic;

namespace PortEval.Application.Core.Common.ChartDataGenerators
{
    /// <inheritdoc cref="IPortfolioChartDataGenerator"/>
    public class PortfolioChartDataGenerator : PositionBasedChartDataGenerator, IPortfolioChartDataGenerator
    {
        public PortfolioChartDataGenerator(IPositionValueCalculator valueCalculator,
            IPositionProfitCalculator profitCalculator, IPositionPerformanceCalculator performanceCalculator)
            : base(valueCalculator, profitCalculator, performanceCalculator)
        {
        }

        /// <inheritdoc />
        public IEnumerable<EntityChartPointDto> ChartValue(PortfolioPositionsPriceListData portfolioData,
            DateRangeParams dateRange, AggregationFrequency frequency)
        {
            return ChartValue(portfolioData.PositionsPriceListData, dateRange, frequency);
        }

        /// <inheritdoc />
        public IEnumerable<EntityChartPointDto> ChartProfit(PortfolioPositionsPriceListData portfolioData, DateRangeParams dateRange, AggregationFrequency frequency)
        {
            return ChartProfit(portfolioData.PositionsPriceListData, dateRange, frequency);
        }

        /// <inheritdoc />
        public IEnumerable<EntityChartPointDto> ChartPerformance(PortfolioPositionsPriceListData portfolioData, DateRangeParams dateRange, AggregationFrequency frequency)
        {
            return ChartPerformance(portfolioData.PositionsPriceListData, dateRange, frequency);
        }

        /// <inheritdoc />
        public IEnumerable<EntityChartPointDto> ChartAggregatedProfit(PortfolioPositionsPriceListData portfolioData, DateRangeParams dateRange, AggregationFrequency frequency)
        {
            return ChartAggregatedProfit(portfolioData.PositionsPriceListData, dateRange, frequency);
        }

        /// <inheritdoc />
        public IEnumerable<EntityChartPointDto> ChartAggregatedPerformance(PortfolioPositionsPriceListData portfolioData, DateRangeParams dateRange, AggregationFrequency frequency)
        {
            return ChartAggregatedPerformance(portfolioData.PositionsPriceListData, dateRange, frequency);
        }
    }
}
