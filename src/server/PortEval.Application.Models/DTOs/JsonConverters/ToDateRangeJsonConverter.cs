using Newtonsoft.Json;
using PortEval.Domain.Models.Enums;
using System;
using System.Collections.Generic;

namespace PortEval.Application.Models.DTOs.JsonConverters
{
    /// <summary>
    /// Handles conversion between JSON and <see cref="ToDateRange">ToDateRange</see>.
    /// </summary>
    public class ToDateRangeJsonConverter : JsonConverter
    {
        private readonly Dictionary<ToDateRange, string> _enumToStringMap = new Dictionary<ToDateRange, string>
        {
            [ToDateRange.OneDay] = "1day",
            [ToDateRange.FiveDays] = "5days",
            [ToDateRange.OneMonth] = "1month",
            [ToDateRange.ThreeMonths] = "3months",
            [ToDateRange.SixMonths] = "6months",
            [ToDateRange.OneYear] = "1year"
        };

        private readonly Dictionary<string, ToDateRange> _stringToEnumMap = new Dictionary<string, ToDateRange>
        {
            ["1day"] = ToDateRange.OneDay,
            ["5days"] = ToDateRange.FiveDays,
            ["1month"] = ToDateRange.OneMonth,
            ["3months"] = ToDateRange.ThreeMonths,
            ["6months"] = ToDateRange.SixMonths,
            ["1year"] = ToDateRange.OneYear
        };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value != null)
            {
                writer.WriteValue(_enumToStringMap[(ToDateRange)value]);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                return null;
            }

            if (reader.TokenType != JsonToken.String)
            {
                throw new JsonSerializationException($"Value {reader.Value} is not allowed.");
            }

            return _stringToEnumMap[(string)reader.Value];
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(ToDateRange) == objectType || typeof(ToDateRange?) == objectType;
        }

        public override bool CanRead => true;
        public override bool CanWrite => true;
    }
}
