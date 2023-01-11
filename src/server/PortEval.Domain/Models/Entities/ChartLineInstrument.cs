using PortEval.Domain.Models.Enums;
using System.Drawing;

namespace PortEval.Domain.Models.Entities
{
    public class ChartLineInstrument : ChartLine
    {
        public int InstrumentId { get; }

        internal ChartLineInstrument(int id, int chartId, int width, LineDashType dash, Color color, int instrumentId) : base(id, chartId, width, dash, color)
        {
            InstrumentId = instrumentId;
        }

        internal ChartLineInstrument(int chartId, int width, LineDashType dash, Color color, int instrumentId) : base(chartId, width, dash, color)
        {
            InstrumentId = instrumentId;
        }

        public static ChartLineInstrument Create(int chartId, int width, LineDashType dash, Color color, Instrument instrument)
        {
            return new ChartLineInstrument(chartId, width, dash, color, instrument.Id);
        }
    }
}
