using System;
using Swashbuckle.AspNetCore.Annotations;

namespace PortEval.Application.Models.DTOs;

[SwaggerSchema("Represents an entity's profit over a given time period.")]
public class EntityProfitDto
{
    [SwaggerSchema("Entity profit.", ReadOnly = true)]
    public decimal Profit { get; set; }

    [SwaggerSchema("Code of the currency in which profit is provided.", ReadOnly = true)]
    public string CurrencyCode { get; set; }

    [SwaggerSchema("Time period start.")] public DateTime From { get; set; }

    [SwaggerSchema("Time period end.")] public DateTime To { get; set; }
}