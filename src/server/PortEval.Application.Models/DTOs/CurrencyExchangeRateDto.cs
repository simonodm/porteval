using System;

namespace PortEval.Application.Models.DTOs
{
    public class CurrencyExchangeRateDto
    {
        public int Id { get; set; }
        public string CurrencyFromCode { get; set; }
        public string CurrencyToCode { get; set; }
        public DateTime Time { get; set; }
        public decimal ExchangeRate { get; set; }
    }
}
