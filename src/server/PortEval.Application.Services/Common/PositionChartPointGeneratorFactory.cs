using PortEval.Application.Features.Interfaces.ChartDataGenerators;
using PortEval.Application.Models.QueryParams;
using System.Collections.Generic;

namespace PortEval.Application.Features.Common
{
    public class PositionChartPointGeneratorFactory : IPositionChartPointGeneratorFactory
    {
        public IPositionChartPointGenerator CreatePositionChartPointGenerator(PositionPriceListData positionPriceListData, IEnumerable<DateRangeParams> ranges)
        {
            return new PositionChartPointGenerator(positionPriceListData, ranges);
        }
    }
}
