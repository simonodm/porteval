using System;

namespace PortEval.Application.Core.BackgroundJobs.Helpers;

/// <summary>
///     Represents a range between two times and an interval.
/// </summary>
public struct TimeRange
{
    /// <summary>
    ///     Represents the start of the time range.
    /// </summary>
    public DateTime From;

    /// <summary>
    ///     Represents the end of the time range.
    /// </summary>
    public DateTime To;

    /// <summary>
    ///     Represents the inner interval of the entries in the time range.
    /// </summary>
    public TimeSpan Interval;
}