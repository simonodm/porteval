using PortEval.Domain.Models.Enums;

namespace PortEval.Infrastructure.Queries.Models;

/// <summary>
///     An internal helper model for mapping queried to-date range to a chart's to-date range.
/// </summary>
public class ToDateRangeQueryModel
{
    public DateRangeUnit ToDateRangeUnit { get; set; }
    public int ToDateRangeValue { get; set; }
}