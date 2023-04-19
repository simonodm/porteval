namespace PortEval.Domain.Models.Enums;

/// <summary>
///     Represents a generic frequency to aggregate data (such as prices or charts) by.
/// </summary>
public enum AggregationFrequency
{
    FiveMin,
    Hour,
    Day,
    Week,
    Month,
    Year
}