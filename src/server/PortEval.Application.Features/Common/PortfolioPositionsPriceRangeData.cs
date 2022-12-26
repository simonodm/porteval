using System.Collections.Generic;

namespace PortEval.Application.Features.Common
{
    internal class PortfolioPositionsPriceRangeData
    {
        public int PortfolioId { get; set; }
        public IEnumerable<PositionPriceRangeData> PositionsPriceRangeData { get; set; }
    }
}
