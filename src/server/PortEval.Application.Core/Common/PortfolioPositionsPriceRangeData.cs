using System.Collections.Generic;

namespace PortEval.Application.Core.Common
{
    internal class PortfolioPositionsPriceRangeData
    {
        public int PortfolioId { get; set; }
        public IEnumerable<PositionPriceRangeData> PositionsPriceRangeData { get; set; }
    }
}
