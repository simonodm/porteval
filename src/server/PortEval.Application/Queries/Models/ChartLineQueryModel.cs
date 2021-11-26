using PortEval.Application.Models.DTOs.Enums;
using PortEval.Domain.Models.Enums;
using System.Drawing;

namespace PortEval.Application.Queries.Models
{
    internal class ChartLineQueryModel
    {
        public int Width { get; set; }
        public LineDashType Dash { get; set; }
        public Color Color { get; set; }
        public ChartLineType Type { get; set; }
        public int? PortfolioLine_PortfolioId { get; set; }
        public int? PositionLine_PortfolioId { get; set; }
        public int? PositionId { get; set; }
        public int? InstrumentId { get; set; }
    }
}
