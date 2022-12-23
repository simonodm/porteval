using Swashbuckle.AspNetCore.Annotations;
using System;

namespace PortEval.Application.Models.DTOs
{
    [SwaggerSchema("Represents a single exchange rate between two currencies at a specific time.")]
    public class CurrencyExchangeRateDto
    {
        [SwaggerSchema("Exchange rate identifier", ReadOnly = true)]
        public int Id { get; set; }

        [SwaggerSchema("Currency from which the exchange rate is.", ReadOnly = true)]
        public string CurrencyFromCode { get; set; }

        [SwaggerSchema("Target currency of the exchange rate.", ReadOnly = true)]
        public string CurrencyToCode { get; set; }

        [SwaggerSchema("Exchange rate time.", ReadOnly = true)]
        public DateTime Time { get; set; }

        [SwaggerSchema("The exchange rate itself.", ReadOnly = true)]
        public decimal ExchangeRate { get; set; }
    }
}
