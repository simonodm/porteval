using PortEval.Application.Features.Interfaces.ChartDataGenerators;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using System.Collections.Generic;

namespace PortEval.Application.Features.Common
{
    public class InstrumentChartPointGeneratorFactory : IInstrumentChartPointGeneratorFactory
    {
        public IInstrumentChartPointGenerator CreateInstrumentChartPointGenerator(IEnumerable<InstrumentPriceDto> prices, IEnumerable<DateRangeParams> ranges)
        {
            return new InstrumentChartPointDataGenerator(prices, ranges);
        }
    }
}
