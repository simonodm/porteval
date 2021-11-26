using PortEval.Application.Models.DTOs.Enums;
using PortEval.Domain.Models.Enums;
using System.Drawing;

namespace PortEval.Application.Models.DTOs
{
    public class ChartLineDto
    {
        public int Width { get; set; }
        public LineDashType Dash { get; set; }
        public Color Color { get; set; }
        public ChartLineType Type { get; set; }
        public int? PortfolioId { get; set; }
        public int? PositionId { get; set; }
        public int? InstrumentId { get; set; }
    }
}
