using Swashbuckle.AspNetCore.Annotations;

namespace PortEval.Application.Models.DTOs;

[SwaggerSchema("Represents a collection of statistics of a financial entity.")]
public class EntityStatisticsDto
{
    [SwaggerSchema("ID of the underlying financial entity.")]
    public int Id { get; set; }

    [SwaggerSchema("Total performance since the entity was created.")]
    public decimal TotalPerformance { get; set; }

    [SwaggerSchema("Performance of the entity in the last month.")]
    public decimal LastMonthPerformance { get; set; }

    [SwaggerSchema("Performance of the entity in the last week.")]
    public decimal LastWeekPerformance { get; set; }

    [SwaggerSchema("Performance of the entity in the last day.")]
    public decimal LastDayPerformance { get; set; }

    [SwaggerSchema("Total profit since the entity was created.")]
    public decimal TotalProfit { get; set; }

    [SwaggerSchema("Profit of the entity in the last month.")]
    public decimal LastMonthProfit { get; set; }

    [SwaggerSchema("Profit of the entity in the last week.")]
    public decimal LastWeekProfit { get; set; }

    [SwaggerSchema("Profit of the entity in the last day.")]
    public decimal LastDayProfit { get; set; }
}