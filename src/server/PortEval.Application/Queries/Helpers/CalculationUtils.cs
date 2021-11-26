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
            var result = new List<TDto>();
            
            var ranges = GetAggregatedRanges(dateRange.From, dateRange.To, frequency);
            foreach (var range in ranges)
            {
                var value = await calculateValue(range);
                if(value != null)
                {
                    result.Add(await calculateValue(range));
                }
            }

            return result;
        }

        public static DateTime GetMax(params DateTime[] times)
        {
            var currentMax = DateTime.MinValue;
            foreach (var time in times)
            {
                if (time > currentMax) currentMax = time;
            }

            return currentMax;
        }

        public static DateTime GetMin(params DateTime[] times)
        {
            var currentMin = DateTime.MaxValue;
            foreach (var time in times)
            {
                if (time < currentMin) currentMin = time;
            }

            return currentMin;
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
                    To = GetMin(nextRangeStart, to)
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
    }
}
