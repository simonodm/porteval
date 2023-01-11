using PortEval.Domain;
using Swashbuckle.AspNetCore.Annotations;
using System;

namespace PortEval.Application.Models.QueryParams
{
    /// <summary>
    /// Represents a range between two dates.
    /// </summary>
    [SwaggerSchema("Represents a time period.")]
    public class DateRangeParams
    {
        [SwaggerSchema("Period start.")]
        public DateTime From { get; set; } = PortEvalConstants.FinancialDataStartTime;

        [SwaggerSchema("Period end.")]
        public DateTime To { get; set; } = DateTime.UtcNow;

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
