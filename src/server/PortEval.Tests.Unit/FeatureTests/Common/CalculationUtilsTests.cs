using PortEval.Application.Features.Queries.Helpers;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.FeatureTests.Common
{
    public class CalculationUtilsTests
    {
        [Theory]
        [InlineData("2022-01-01", "2022-02-01", AggregationFrequency.Day)]
        [InlineData("2022-05-03", "2022-05-04", AggregationFrequency.Hour)]
        [InlineData("2020-03-17 12:00", "2020-03-17 14:00", AggregationFrequency.FiveMin)]
        [InlineData("2012-07-01", "2012-07-28", AggregationFrequency.Week)]
        [InlineData("2007-01-01", "2007-12-01", AggregationFrequency.Month)]
        [InlineData("2004-01-01", "2016-01-01", AggregationFrequency.Year)]
        public async Task AggregateCalculations_AggregatesUsingCorrectFrequency(string dateFrom, string dateTo,
            AggregationFrequency frequency)
        {
            var startDate = DateTime.Parse(dateFrom);
            var endDate = DateTime.Parse(dateTo);

            var dateRange = new DateRangeParams
            {
                From = startDate,
                To = endDate
            };
            var interval = AggregationFrequencyToTimeSpan(frequency);

            var result = (await CalculationUtils.AggregateCalculations(dateRange, frequency, GetDateRangeStart)).ToList();

            Assert.Equal(Math.Round((endDate - startDate) / interval), result.Count);

            var current = startDate;
            foreach (var item in result)
            {
                Assert.Equal(current, item, interval - TimeSpan.FromMinutes(1));
                current += interval;
            }
        }

        [Theory]
        [InlineData("2000-01-01", "2000-02-14", AggregationFrequency.FiveMin)]
        [InlineData("2002-01-01", "2022-01-01", AggregationFrequency.Hour)]
        [InlineData("2011-01-01", "2022-01-01", AggregationFrequency.Day)]
        [InlineData("1990-01-01", "2022-01-01", AggregationFrequency.Week)]
        [InlineData("1970-01-01", "2022-01-01", AggregationFrequency.Month)]
        [InlineData("1922-01-01", "2022-01-01", AggregationFrequency.Year)]
        public async Task AggregateCalculations_LimitsDateRangeTo_WhenDateRangeIsTooLongForFrequency(
            string dateFrom, string dateTo, AggregationFrequency frequency)
        {
            var startDate = DateTime.Parse(dateFrom);
            var endDate = DateTime.Parse(dateTo);

            var dateRange = new DateRangeParams
            {
                From = startDate,
                To = endDate
            };

            var result =
                (await CalculationUtils.AggregateCalculations(dateRange, frequency, GetDateRangeStart)).ToList();

            Assert.True(result[0] > startDate);
        }

        private Task<DateTime> GetDateRangeStart(DateRangeParams dateRange)
        {
            return Task.FromResult(dateRange.From);
        }

        private TimeSpan AggregationFrequencyToTimeSpan(AggregationFrequency frequency)
        {
            return frequency switch
            {
                AggregationFrequency.FiveMin => TimeSpan.FromMinutes(5),
                AggregationFrequency.Hour => TimeSpan.FromHours(1),
                AggregationFrequency.Day => TimeSpan.FromDays(1),
                AggregationFrequency.Week => TimeSpan.FromDays(7),
                AggregationFrequency.Month => TimeSpan.FromDays(30),
                AggregationFrequency.Year => TimeSpan.FromDays(365),
                _ => throw new ArgumentOutOfRangeException(nameof(frequency))
            };
        }
    }
}
