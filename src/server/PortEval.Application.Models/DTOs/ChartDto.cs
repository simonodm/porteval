using System;
using System.Collections.Generic;
using PortEval.Domain.Models.Enums;
using PortEval.Domain.Models.ValueObjects;
using Swashbuckle.AspNetCore.Annotations;

namespace PortEval.Application.Models.DTOs;

[SwaggerSchema("Represents a single chart")]
public class ChartDto
{
    [SwaggerSchema("The chart identifier", ReadOnly = true)]
    public int Id { get; set; }

    [SwaggerSchema("Chart name")] public string Name { get; set; }

    [SwaggerSchema("Chart date range start. Only applicable when IsToDate is not set to true.", Nullable = true)]
    public DateTime? DateRangeStart { get; set; }

    [SwaggerSchema("Chart date range end. Only applicable when IsToDate is not set to true.", Nullable = true)]
    public DateTime? DateRangeEnd { get; set; }

    [SwaggerSchema(
        "Determines whether the chart's date range is fixed, or whether the chart displays data for the given ToDateRange until the current date and time.",
        Nullable = true)]
    public bool? IsToDate { get; set; }

    [SwaggerSchema(
        "Determines the range in which the chart displays data until the current date and time. Only applicable when IsToDate is set to true.")]
    public ToDateRange ToDateRange { get; set; }

    [SwaggerSchema("Chart type. Determines what data it displays.")]
    public ChartType Type { get; set; }

    [SwaggerSchema("Chart currency code. Only applicable for charts with types Price, Profit, Aggregated Profit")]
    public string CurrencyCode { get; set; }

    [SwaggerSchema(
        "Chart aggregation frequency. Only applicable for charts with types Aggregated Profit or Aggregated Performance")]
    public AggregationFrequency? Frequency { get; set; }

    [SwaggerSchema("Collection of chart's lines.")]
    public List<ChartLineDto> Lines { get; set; }
}