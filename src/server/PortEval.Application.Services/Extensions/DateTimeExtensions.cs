using System;

namespace PortEval.Application.Services.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Rounds down the supplied <c>DateTime</c> object to the nearest time represented by a <c>TimeSpan</c>.
        /// </summary>
        /// <param name="time">Date and time to round down.</param>
        /// <param name="timeSpan">Time representing the round down target.</param>
        /// <returns>A rounded down <c>DateTime</c>.</returns>
        public static DateTime RoundDown(this DateTime time, TimeSpan timeSpan)
        {
            var ticks = time.Ticks - time.Ticks % timeSpan.Ticks;
            return new DateTime(ticks);
        }
    }
}
