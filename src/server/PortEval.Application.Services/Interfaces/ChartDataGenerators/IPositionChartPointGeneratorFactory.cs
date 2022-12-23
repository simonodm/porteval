using PortEval.Application.Features.Common;
using PortEval.Application.Models.QueryParams;
using System.Collections.Generic;

namespace PortEval.Application.Features.Interfaces.ChartDataGenerators
{
    public interface IPositionChartPointGeneratorFactory
    {
        public IPositionChartPointGenerator CreatePositionChartPointGenerator(PositionPriceListData positionPriceListData, IEnumerable<DateRangeParams> ranges);
    }
}
