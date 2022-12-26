using Swashbuckle.AspNetCore.Annotations;
using System;

namespace PortEval.Application.Models.DTOs
{
    [SwaggerSchema("Represents an instrument's price at a given time.")]
    public class InstrumentPriceDto
    {
        [SwaggerSchema("Price identifier.", ReadOnly = true)]
        public int Id { get; set; }

        [SwaggerSchema("Price time.")]
        public DateTime Time { get; set; }

        [SwaggerSchema("Price itself.")]
        public decimal Price { get; set; }

        [SwaggerSchema("Instrument identifier to which this price belongs.")]
        public int InstrumentId { get; set; }
    }
}
