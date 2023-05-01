using System;

namespace PortEval.Application.Core.Extensions;

/// <summary>
///     Implements <see cref="DateTime" /> extension methods.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    ///     Rounds down the supplied <c>DateTime</c> object to the nearest time represented by a <c>TimeSpan</c>.
    /// </summary>
    /// <param name="time">Date and time to round down.</param>
    /// <param name="timeSpan">Time representing the round down target.</param>
    /// <returns>A rounded down <c>DateTime</c>.</returns>
    public static DateTime RoundDown(this DateTime time, TimeSpan timeSpan)
    {
        var ticks = time.Ticks - time.Ticks % timeSpan.Ticks;
        return new DateTime(ticks);
    }

    /// <summary>
    ///     Rounds up the supplied <c>DateTime</c> object to the nearest time represented by a <c>TimeSpan</c>.
    /// </summary>
    /// <param name="time">Date and time to round up.</param>
    /// <param name="timeSpan">Time representing the round up target.</param>
    /// <returns>A rounded up <c>DateTime</c>.</returns>
    public static DateTime RoundUp(this DateTime time, TimeSpan timeSpan)
    {
        if (time.Ticks % timeSpan.Ticks == 0)
        {
            return time;
        }

        var ticks = time.Ticks + (timeSpan.Ticks - time.Ticks % timeSpan.Ticks);
        return new DateTime(ticks);
    }

    /// <summary>
    ///     Returns the smaller <c>DateTime</c> value out of the two provided.
    /// </summary>
    /// <param name="time">First time.</param>
    /// <param name="otherTime">Other time.</param>
    /// <returns>The smaller <c>DateTime</c></returns>
    public static DateTime GetMin(this DateTime time, DateTime otherTime)
    {
        return new DateTime(Math.Min(time.Ticks, otherTime.Ticks));
    }

    /// <summary>
    ///     Returns the larger <c>DateTime</c> value out of the two provided.
    /// </summary>
    /// <param name="time">First time.</param>
    /// <param name="otherTime">Other time.</param>
    /// <returns>The larger <c>DateTime</c></returns>
    public static DateTime GetMax(this DateTime time, DateTime otherTime)
    {
        return new DateTime(Math.Max(time.Ticks, otherTime.Ticks));
    }
}