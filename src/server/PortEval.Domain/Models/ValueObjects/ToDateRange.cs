using PortEval.Domain.Models.Enums;

namespace PortEval.Domain.Models.ValueObjects;

/// <summary>
///     Represents the to-date range of a chart.
///     To-date range is defined as a range starting a fixed time in the past, and ending on the current date and time.
/// </summary>
public class ToDateRange : ValueObject
{
    /// <summary>
    ///     Time unit of the range.
    /// </summary>
    public DateRangeUnit Unit { get; }

    /// <summary>
    ///     Value of the range in <see cref="Unit" />.
    /// </summary>
    public int Value { get; }

    /// <summary>
    ///     Initializes the to-date range.
    /// </summary>
    /// <param name="unit">Unit of the range.</param>
    /// <param name="value">Value of the range.</param>
    public ToDateRange(DateRangeUnit unit, int value)
    {
        Unit = unit;
        Value = value;
    }
}