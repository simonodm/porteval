using System;
using Swashbuckle.AspNetCore.Annotations;

namespace PortEval.Application.Models.DTOs;

[SwaggerSchema("Represents a position's break-even point at a certain time.")]
public class PositionBreakEvenPointDto
{
    [SwaggerSchema("Break-even point.")] public decimal BreakEvenPoint { get; set; }

    [SwaggerSchema("Time at which the break-even point applies.")]
    public DateTime Time { get; set; }
}