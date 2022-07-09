using Newtonsoft.Json;
using PortEval.Domain.Models.Enums;
using System;

namespace PortEval.Application.Models.DTOs.Converters
{
    /// <summary>
    /// Handles conversion between JSON and <see cref="TemplateType" />
    /// </summary>
    public class TemplateTypeJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is not TemplateType templateType) return;

            var converted = templateType.ToString();
            writer.WriteValue(converted[..1].ToUpper() + converted[1..]);
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

            try
            {
                return Enum.Parse(typeof(TemplateType), ((string)existingValue).ToLower());
            }
            catch
            {
                return null;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(TemplateType) == objectType;
        }
    }
}
