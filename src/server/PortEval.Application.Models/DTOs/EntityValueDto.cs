using System;

namespace PortEval.Application.Models.DTOs
{
    public class EntityValueDto
    {
        public decimal Value { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime Time { get; set; }
    }
}
