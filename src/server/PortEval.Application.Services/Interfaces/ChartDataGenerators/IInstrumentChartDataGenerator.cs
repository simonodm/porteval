using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;
using System.Collections.Generic;

namespace PortEval.Application.Features.Interfaces.ChartDataGenerators
{
    public interface IInstrumentChartDataGenerator
    {
        public IEnumerable<EntityChartPointDto> ChartPrices(IEnumerable<InstrumentPriceDto> prices,
            DateRangeParams dateRange, AggregationFrequency frequency);

        public IEnumerable<EntityChartPointDto> ChartProfit(IEnumerable<InstrumentPriceDto> prices,
            DateRangeParams dateRange, AggregationFrequency frequency);

        public IEnumerable<EntityChartPointDto> ChartPerformance(IEnumerable<InstrumentPriceDto> prices,
            DateRangeParams dateRange, AggregationFrequency frequency);

        public IEnumerable<EntityChartPointDto> ChartAggregatedProfit(IEnumerable<InstrumentPriceDto> prices,
            DateRangeParams dateRange, AggregationFrequency frequency);

        public IEnumerable<EntityChartPointDto> ChartAggregatedPerformance(IEnumerable<InstrumentPriceDto> prices,
            DateRangeParams dateRange, AggregationFrequency frequency);
    }
}
