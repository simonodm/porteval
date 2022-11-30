using PortEval.Domain.Models.Enums;
using System.Drawing;

namespace PortEval.Domain.Models.Entities
{
    public class ChartLinePortfolio : ChartLine
    {
        public int PortfolioId { get; }

        public ChartLinePortfolio(int id, int chartId, int width, LineDashType dash, Color color, int portfolioId) : base(id, chartId, width, dash, color)
        {
            PortfolioId = portfolioId;
        }

        public ChartLinePortfolio(int chartId, int width, LineDashType dash, Color color, int portfolioId) : base(chartId, width, dash, color)
        {
            PortfolioId = portfolioId;
        }
    }
}
