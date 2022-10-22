using PortEval.Domain.Models.Enums;
using System.Drawing;

namespace PortEval.Domain.Models.Entities
{
    public class ChartLineInstrument : ChartLine
    {
        public int InstrumentId { get; }

        public ChartLineInstrument(int chartId, int width, LineDashType dash, Color color, int instrumentId) : base(chartId, width, dash, color)
        {
            InstrumentId = instrumentId;
        }
    }
}
