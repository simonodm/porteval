using System;
using Newtonsoft.Json;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Models.DTOs.Converters;

public class ImportStatusJsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is ImportStatus status)
        {
            writer.WriteValue(ImportStatusStringValues.EnumToStringMap[status]);
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

        return ImportStatusStringValues.StringToEnumMap[(string)reader.Value];
    }

    public override bool CanConvert(Type objectType)
    {
        return typeof(ImportStatus) == objectType || typeof(ImportStatus?) == objectType;
    }
}