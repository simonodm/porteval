﻿using System;
using Swashbuckle.AspNetCore.Annotations;

namespace PortEval.Application.Models.QueryParams
{
    /// <summary>
    /// Represents a range between two dates.
    /// </summary>
    [SwaggerSchema("Represents a time period.")]
    public class DateRangeParams
    {
        [SwaggerSchema("Period start.")]
        public DateTime From { get; set; } = DateTime.MinValue;

        [SwaggerSchema("Period end.")]
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
