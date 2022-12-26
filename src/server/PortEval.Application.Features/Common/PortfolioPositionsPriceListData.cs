using System.Collections.Generic;

namespace PortEval.Application.Features.Common
{
    public class PortfolioPositionsPriceListData
    {
        public int PortfolioId { get; set; }
        public IEnumerable<PositionPriceListData> PositionsPriceListData { get; set; }
    }
}
