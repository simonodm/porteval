using System.Drawing;
using PortEval.Domain.Models.Enums;

namespace PortEval.Domain.Models.Entities;

/// <summary>
///     A chart line representing a portfolio.
/// </summary>
public class ChartLinePortfolio : ChartLine
{
    /// <summary>
    ///     ID of the portfolio represented by this line.
    /// </summary>
    public int PortfolioId { get; }

    internal ChartLinePortfolio(int id, int chartId, int width, LineDashType dash, Color color, int portfolioId) : base(
        id, chartId, width, dash, color)
    {
        PortfolioId = portfolioId;
    }

    internal ChartLinePortfolio(int chartId, int width, LineDashType dash, Color color, int portfolioId) : base(chartId,
        width, dash, color)
    {
        PortfolioId = portfolioId;
    }

    /// <summary>
    ///     A factory method creating the chart line according to the specified parameters.
    /// </summary>
    /// <param name="chartId">ID of the parent chart.</param>
    /// <param name="width">Chart line width in pixels.</param>
    /// <param name="dash">Chart line dash type.</param>
    /// <param name="color">Chart line color.</param>
    /// <param name="portfolio">A reference to the portfolio represented by this line.</param>
    /// <returns>The newly created portfolio chart line.</returns>
    public static ChartLinePortfolio Create(int chartId, int width, LineDashType dash, Color color, Portfolio portfolio)
    {
        return new ChartLinePortfolio(chartId, width, dash, color, portfolio.Id);
    }
}