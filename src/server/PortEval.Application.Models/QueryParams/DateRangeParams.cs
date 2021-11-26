using System;

namespace PortEval.Application.Models.QueryParams
{
    /// <summary>
    /// Represents a range between two dates.
    /// </summary>
    public class DateRangeParams
    {
        public DateTime From { get; set; } = DateTime.MinValue;
        public DateTime To { get; set; } = DateTime.Now;

        public DateRangeParams SetFrom(DateTime from)
        {
            return new DateRangeParams
            {
                From = from,
                To = To
            };
        }

        public DateRangeParams SetTo(DateTime to)
        {
            return new DateRangeParams
            {
                From = From,
                To = to
            };
        }
    }
}
