using System;

namespace PortEval.Application.Models.DTOs
{
    public class InstrumentPriceDto
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public decimal Price { get; set; }
        public int InstrumentId { get; set; }
    }
}
