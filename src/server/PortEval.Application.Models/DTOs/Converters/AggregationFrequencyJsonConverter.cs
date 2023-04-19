using System;
using Newtonsoft.Json;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Models.DTOs.Converters;

/// <summary>
///     Handles conversion between JSON and <see cref="AggregationFrequency">AggregationFrequency</see>.
/// </summary>
public class AggregationFrequencyJsonConverter : JsonConverter
{
    public override bool CanRead => true;
    public override bool CanWrite => true;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value != null)
            writer.WriteValue(AggregationFrequencyStringValues.EnumToStringMap[(AggregationFrequency)value]);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.Value == null) return null;

        if (reader.TokenType != JsonToken.String)
            throw new JsonSerializationException($"Value {reader.Value} is not allowed.");

        return AggregationFrequencyStringValues.StringToEnumMap[(string)reader.Value];
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(AggregationFrequency) || objectType == typeof(AggregationFrequency?);
    }
}