using PortEval.Domain.Models.Enums;
using System.Drawing;

namespace PortEval.Domain.Models.Entities
{
    public abstract class ChartLine : Entity
    {
        public int Id { get; private set; }
        public int Width { get; private set; }
        public LineDashType Dash { get; private set; }
        public int ChartId { get; private set; }
        public Chart Chart { get; private set; }
        public Color Color { get; private set; }

        protected ChartLine(int id, int chartId, int width, LineDashType dash, Color color) : this(chartId, width, dash, color)
        {
            Id = id;
        }

        protected ChartLine(int chartId, int width, LineDashType dash, Color color)
        {
            ChartId = chartId;
            Width = width;
            Dash = dash;
            Color = color;
        }

        public void SetWidth(int width)
        {
            Width = width;
        }

        public void SetDash(LineDashType dash)
        {
            Dash = dash;
        }

        public void SetColor(Color color)
        {
            Color = color;
        }
    }
}
