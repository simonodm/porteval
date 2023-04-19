using Swashbuckle.AspNetCore.Annotations;

namespace PortEval.Application.Models.DTOs;

[SwaggerSchema("Represents a collection of a position's financial metrics.")]
public class PositionStatisticsDto : EntityStatisticsDto
{
    [SwaggerSchema("Position's break-even point.")]
    public decimal BreakEvenPoint { get; set; }
}