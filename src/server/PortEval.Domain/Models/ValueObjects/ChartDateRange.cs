using System;
using PortEval.Domain.Exceptions;

namespace PortEval.Domain.Models.ValueObjects;

/// <summary>
///     Represents the date range of a chart.
/// </summary>
public class ChartDateRange : ValueObject
{
    /// <summary>
    ///     The start of the date range.
    ///     This field is only populated for fixed ranges.
    /// </summary>
    public DateTime? Start { get; private set; }

    /// <summary>
    ///     The end of the date range.
    ///     This field is only populated for fixed ranges.
    /// </summary>
    public DateTime? End { get; private set; }

    /// <summary>
    ///     Determines whether the range is fixed or to-date.
    /// </summary>
    public bool IsToDate { get; private set; }

    /// <summary>
    ///     To-date range configuration.
    ///     This field is only populated for to-date ranges.
    /// </summary>
    public ToDateRange ToDateRange { get; private set; }

    /// <summary>
    ///     Initializes a fixed from/to chart date range.
    /// </summary>
    /// <param name="start">Start date and time.</param>
    /// <param name="end">End date and time.</param>
    /// <exception cref="OperationNotAllowedException">Thrown if the range is invalid.</exception>
    public ChartDateRange(DateTime start, DateTime end)
    {
        if (end < start)
        {
            throw new OperationNotAllowedException("Chart date range start cannot be later than the end.");
        }

        if ((end - start).TotalDays < 1)
        {
            throw new OperationNotAllowedException("Chart date range cannot be less than 1 day.");
        }

        Start = start;
        End = end;
    }

    /// <summary>
    ///     Initializes a to-date chart date range.
    /// </summary>
    /// <param name="toDateRange">To-date range configuration.</param>
    public ChartDateRange(ToDateRange toDateRange)
    {
        IsToDate = true;
        ToDateRange = toDateRange;
    }
}