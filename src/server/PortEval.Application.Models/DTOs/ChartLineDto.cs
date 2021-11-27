using PortEval.Application.Models.DTOs.Enums;
using PortEval.Domain.Models.Enums;
using System.Drawing;
using Swashbuckle.AspNetCore.Annotations;

namespace PortEval.Application.Models.DTOs
{
    [SwaggerSchema("A single chart line.")]
    public class ChartLineDto
    {
        [SwaggerSchema("Determines the line's width in pixels.")]
        public int Width { get; set; }

        [SwaggerSchema("Determines the line's dash, e. g. solid, dashed, dotted.")]
        public LineDashType Dash { get; set; }

        [SwaggerSchema("Determines the line's color.", Format="#RRGGBB")]
        public Color Color { get; set; }

        [SwaggerSchema("Determines the kind of entity the line displays data for - portfolio, position or instrument.")]
        public ChartLineType Type { get; set; }

        [SwaggerSchema("Portfolio identifier for lines of type Portfolio.")]
        public int? PortfolioId { get; set; }

        [SwaggerSchema("Position identifier for lines of type Position.")]
        public int? PositionId { get; set; }

        [SwaggerSchema("Instrument identifier for lines of type Instrument.")]
        public int? InstrumentId { get; set; }
    }
}
