using Newtonsoft.Json;
using PortEval.Domain.Models.Enums;
using System;
using System.Collections.Generic;

namespace PortEval.Application.Models.DTOs.JsonConverters
{
    /// <summary>
    /// Handles conversion between JSON and <see cref="AggregationFrequency">AggregationFrequency</see>.
    /// </summary>
    public class AggregationFrequencyJsonConverter : JsonConverter
    {
        private readonly Dictionary<AggregationFrequency, string> _enumToStringMap = new Dictionary<AggregationFrequency, string>
        {
            [AggregationFrequency.FiveMin] = "5min",
            [AggregationFrequency.Hour] = "hour",
            [AggregationFrequency.Day] = "day",
            [AggregationFrequency.Week] = "week",
            [AggregationFrequency.Month] = "month",
            [AggregationFrequency.Year] = "year"
        };

        private readonly Dictionary<string, AggregationFrequency> _stringToEnumMap = new Dictionary<string, AggregationFrequency>
        {
            ["5min"] = AggregationFrequency.FiveMin,
            ["hour"] = AggregationFrequency.Hour,
            ["day"] = AggregationFrequency.Day,
            ["week"] = AggregationFrequency.Week,
            ["month"] = AggregationFrequency.Month,
            ["year"] = AggregationFrequency.Year
        };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value != null)
            {
                writer.WriteValue(_enumToStringMap[(AggregationFrequency)value]);
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
            return typeof(AggregationFrequency) == objectType || typeof(AggregationFrequency) == objectType;
        }

        public override bool CanRead => true;
        public override bool CanWrite => true;
    }
}
