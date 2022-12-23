using PortEval.Application.Features.Common;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;
using System.Collections.Generic;

namespace PortEval.Application.Features.Interfaces.ChartDataGenerators
{
    public interface IPositionChartDataGenerator
    {
        public IEnumerable<EntityChartPointDto> ChartValue(IEnumerable<PositionPriceListData> positionsPriceListData,
            DateRangeParams dateRange, AggregationFrequency frequency);
        public IEnumerable<EntityChartPointDto> ChartProfit(IEnumerable<PositionPriceListData> positionsPriceData,
            DateRangeParams dateRange, AggregationFrequency frequency);
        public IEnumerable<EntityChartPointDto> ChartPerformance(IEnumerable<PositionPriceListData> positionsPriceData,
            DateRangeParams dateRange, AggregationFrequency frequency);
        public IEnumerable<EntityChartPointDto> ChartAggregatedProfit(IEnumerable<PositionPriceListData> positionsPriceData,
            DateRangeParams dateRange, AggregationFrequency frequency);
        public IEnumerable<EntityChartPointDto> ChartAggregatedPerformance(IEnumerable<PositionPriceListData> positionsPriceData,
            DateRangeParams dateRange, AggregationFrequency frequency);
    }
}
