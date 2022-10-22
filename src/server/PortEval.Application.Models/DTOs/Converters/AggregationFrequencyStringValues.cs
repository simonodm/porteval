using PortEval.Domain.Models.Enums;
using System.Collections.Generic;

namespace PortEval.Application.Models.DTOs.Converters
{
    internal static class AggregationFrequencyStringValues
    {
        public static readonly Dictionary<AggregationFrequency, string> EnumToStringMap = new Dictionary<AggregationFrequency, string>
        {
            [AggregationFrequency.FiveMin] = "5min",
            [AggregationFrequency.Hour] = "hour",
            [AggregationFrequency.Day] = "day",
            [AggregationFrequency.Week] = "week",
            [AggregationFrequency.Month] = "month",
            [AggregationFrequency.Year] = "year"
        };

        public static readonly Dictionary<string, AggregationFrequency> StringToEnumMap = new Dictionary<string, AggregationFrequency>
        {
            ["5min"] = AggregationFrequency.FiveMin,
            ["hour"] = AggregationFrequency.Hour,
            ["day"] = AggregationFrequency.Day,
            ["week"] = AggregationFrequency.Week,
            ["month"] = AggregationFrequency.Month,
            ["year"] = AggregationFrequency.Year
        };
    }
}
