using PortEval.Domain.Models.Enums;
using System.Drawing;

namespace PortEval.Domain.Models.Entities
{
    public class ChartLinePosition : ChartLine
    {
        public int PositionId { get; }

        public ChartLinePosition(int chartId, int width, LineDashType dash, Color color, int positionId) : base(chartId, width, dash, color)
        {
            PositionId = positionId;
        }
    }
}
