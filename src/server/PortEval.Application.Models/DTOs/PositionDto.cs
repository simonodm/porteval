using Swashbuckle.AspNetCore.Annotations;

namespace PortEval.Application.Models.DTOs
{
    [SwaggerSchema("Represents a single instrument in an investment portfolio.")]
    public class PositionDto
    {
        [SwaggerSchema("Position identifier.", ReadOnly = true)]
        public int Id { get; set; }

        [SwaggerSchema("Identifier of the portfolio to which this position belongs.")]
        public int PortfolioId { get; set; }

        [SwaggerSchema("Identifier of the instrument which this position represents.")]
        public int InstrumentId { get; set; }

        [SwaggerSchema("Position note.")]
        public string Note { get; set; }

        [SwaggerSchema("Instrument which this position represents.")]
        public InstrumentDto Instrument { get; set; }
    }
}
