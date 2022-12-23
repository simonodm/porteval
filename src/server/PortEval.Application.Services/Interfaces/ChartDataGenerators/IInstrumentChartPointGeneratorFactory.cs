using System.Collections.Generic;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;

namespace PortEval.Application.Features.Interfaces.ChartDataGenerators
{
    public interface IInstrumentChartPointGeneratorFactory
    {
        public IInstrumentChartPointGenerator CreateInstrumentChartPointGenerator(IEnumerable<InstrumentPriceDto> prices, IEnumerable<DateRangeParams> ranges);
    }
}
