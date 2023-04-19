using System.Collections.Generic;

namespace PortEval.Application.Core.Common;

/// <summary>
///     Wraps a collection of <see cref="PositionPriceListData" /> instances corresponding to positions of a portfolio.
/// </summary>
public class PortfolioPositionsPriceListData
{
    /// <summary>
    ///     ID of the parent portfolio.
    /// </summary>
    public int PortfolioId { get; set; }

    /// <summary>
    ///     A collection of transaction and instrument price wrappers over positions of a portfolio represented by
    ///     <see cref="PortfolioId" />.
    /// </summary>
    public IEnumerable<PositionPriceListData> PositionsPriceListData { get; set; }
}