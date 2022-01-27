using PortEval.Application.Models.QueryParams;
using PortEval.Application.Services.Extensions;
using PortEval.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Application.Queries.Helpers
{
    /// <summary>
    /// Contains static helper methods for financial entity data calculations.
    /// </summary>
    internal static class CalculationUtils
    {
        /// <summary>
        /// Represents maximum time periods for each aggregation frequency.
        /// </summary>
        private static readonly Dictionary<AggregationFrequency, TimeSpan> _frequencyRangeLimits =
            new()
            {
                [AggregationFrequency.FiveMin] = TimeSpan.FromDays(14),
                [AggregationFrequency.Hour] = TimeSpan.FromDays(180),
                [AggregationFrequency.Day] = TimeSpan.FromDays(365 * 10),
                [AggregationFrequency.Week] = TimeSpan.FromDays(365 * 25),
                [AggregationFrequency.Month] = TimeSpan.FromDays(365 * 50),
                [AggregationFrequency.Year] = TimeSpan.FromDays(365 * 50)
            };


        /// <summary>
        /// Calculates aggregated financial data in the supplied date range with the supplied frequency.
        /// </summary>
        /// <typeparam name="TDto">Response DTO.</typeparam>
        /// <param name="dateRange">Date range to calculate.</param>
        /// <param name="frequency">Aggregation interval length.</param>
        /// <param name="calculateValue">Callback to calculate the desired data in a single interval.</param>
        /// <returns></returns>
        public static async Task<IEnumerable<TDto>> AggregateCalculations<TDto>(DateRangeParams dateRange,
            AggregationFrequency frequency, Func<DateRangeParams, Task<TDto>> calculateValue)
        {
            var limitedRange = LimitDateRange(dateRange, frequency);

            var ranges = GetAggregatedRanges(limitedRange.From, limitedRange.To, frequency);
            var result = await Task.WhenAll(ranges.Select(calculateValue));

            return result;
        }

        /// <summary>
        /// Divides the supplied date range into intervals based on the supplied aggregation frequency. Intervals are adjusted for calendar dates
        /// (i.e. a frequency of 1 month means that the interval will start on the first day of the month and end on the last day of the month,
        /// with the possible exception of <c>from</c> or <c>to</c> parameters).
        /// </summary>
        /// <param name="from">Date range start.</param>
        /// <param name="to">Date range end.</param>
        /// <param name="frequency">Interval length.</param>
        /// <returns>An <c>IEnumerable</c> of date ranges of the supplied interval length.</returns>
        private static IEnumerable<DateRangeParams> GetAggregatedRanges(DateTime from, DateTime to, AggregationFrequency frequency)
        {
            var current = from;
            var result = new List<DateRangeParams>();
            while (current < to)
            {
                var nextRangeStart = GetNextRangeStart(current, frequency);
                var range = new DateRangeParams
                {
                    From = current,
                    To = nextRangeStart.GetMin(to)
                };
                result.Add(range);
                current = nextRangeStart;
            }

            return result;
        }

        /// <summary>
        /// Calculates the start of the next interval based on the supplied date and aggregation frequency.
        /// </summary>
        /// <param name="time">Time to calculate the start of the next interval from.</param>
        /// <param name="frequency">Aggregation frequency.</param>
        /// <returns>Date and time of the next interval start.</returns>
        private static DateTime GetNextRangeStart(this DateTime time, AggregationFrequency frequency)
        {
            var current = time;
            switch (frequency)
            {
                case AggregationFrequency.FiveMin:
                    current = current.RoundDown(TimeSpan.FromMinutes(5)).AddMinutes(5);
                    break;
                case AggregationFrequency.Hour:
                    current = current.RoundDown(TimeSpan.FromHours(1)).AddHours(1);
                    break;
                case AggregationFrequency.Day:
                    current = current.RoundDown(TimeSpan.FromDays(1)).AddDays(1);
                    break;
                case AggregationFrequency.Week:
                    current = current.RoundDown(TimeSpan.FromDays(1)).AddDays(6 - (int)current.DayOfWeek + 1);
                    break;
                case AggregationFrequency.Month:
                    current = current.RoundDown(TimeSpan.FromDays(1)).AddMonths(1).AddDays(-current.Day + 1);
                    break;
                case AggregationFrequency.Year:
                    current = new DateTime(current.Year + 1, 1, 1);
                    break;
            }

            return current;
        }

        /// <summary>
        /// Limits the supplied date range in accordance to maximum durations allowed for each frequency. This is done to avoid extreme scenarios like
        /// processing every 5 minutes since 0001-01-01.
        /// </summary>
        /// <param name="range">Original date range.</param>
        /// <param name="frequency">Aggregation frequency.</param>
        /// <returns>Date range limited to maximum allowed values for the specified aggregation frequency.</returns>
        private static DateRangeParams LimitDateRange(DateRangeParams range, AggregationFrequency frequency)
        {
            var maxDuration = _frequencyRangeLimits[frequency];
            if (range.To - range.From <= maxDuration) return range;

            return range.SetFrom(range.To - maxDuration);
        }
    }
}
