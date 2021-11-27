﻿using PortEval.Domain.Models.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace PortEval.Application.Models.DTOs
{
    [SwaggerSchema("Represents a single instrument.")]
    public class InstrumentDto
    {
        [SwaggerSchema("Instrument identifier.", ReadOnly = true)]
        public int Id { get; set; }

        [SwaggerSchema("Instrument ticker.")]
        public string Symbol { get; set; }

        [SwaggerSchema("Instrument name.")]
        public string Name { get; set; }

        [SwaggerSchema("Exchange at which the given instrument is traded.")]
        public string Exchange { get; set; }

        [SwaggerSchema("Instrument type.")]
        public InstrumentType Type { get; set; }

        [SwaggerSchema("Instrument currency code.")]
        public string CurrencyCode { get; set; }
    }
}
