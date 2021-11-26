namespace PortEval.Application.Models.DTOs
{
    public class PositionDto
    {
        public int Id { get; set; }
        public int PortfolioId { get; set; }
        public int InstrumentId { get; set; }
        public string Note { get; set; }
        public InstrumentDto Instrument { get; set; }
    }
}
