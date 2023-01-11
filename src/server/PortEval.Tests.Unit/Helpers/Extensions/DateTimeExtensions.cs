using System;

namespace PortEval.Tests.Unit.Helpers.Extensions
{
    internal static class DateTimeExtensions
    {
        public static bool InRange(this DateTime dt, DateTime expected, TimeSpan range)
        {
            return dt >= expected.Subtract(range) && dt <= expected.Add(range);
        }
    }
}
