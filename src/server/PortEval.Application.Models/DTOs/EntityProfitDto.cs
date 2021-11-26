using System;

namespace PortEval.Application.Models.DTOs
{
    public class EntityProfitDto
    {
        public decimal Profit { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
