﻿using Swashbuckle.AspNetCore.Annotations;
using System;

namespace PortEval.Application.Models.DTOs
{
    [SwaggerSchema("Represents an entity's value at a given time.")]
    public class EntityValueDto
    {
        [SwaggerSchema("Value itself.", ReadOnly = true)]
        public decimal Value { get; set; }

        [SwaggerSchema("Code of the currency in which the value is provided.", ReadOnly = true)]
        public string CurrencyCode { get; set; }

        [SwaggerSchema("Time at which the value is provided.")]
        public DateTime Time { get; set; }
    }
}
