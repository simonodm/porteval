using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Models.DTOs
{
    public class InstrumentDto
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Exchange { get; set; }
        public InstrumentType Type { get; set; }
        public string CurrencyCode { get; set; }
    }
}
