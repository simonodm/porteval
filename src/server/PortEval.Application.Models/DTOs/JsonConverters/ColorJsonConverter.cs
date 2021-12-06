using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Globalization;
using PortEval.Domain.Exceptions;

namespace PortEval.Application.Models.DTOs.JsonConverters
{
    public class ColorJsonConverter : JsonConverter<Color>
    {
        private Color HexToColor(string hexValue)
        {
            if (hexValue.Length == 7 &&
                hexValue.StartsWith('#') &&
                int.TryParse($"{hexValue[1]}{hexValue[2]}", NumberStyles.HexNumber, null, out var red) &&
                int.TryParse($"{hexValue[3]}{hexValue[4]}", NumberStyles.HexNumber, null, out var green) &&
                int.TryParse($"{hexValue[5]}{hexValue[6]}", NumberStyles.HexNumber, null, out var blue))
            {
                return Color.FromArgb(red, green, blue);
            }

            throw new OperationNotAllowedException($"Invalid hex value color: {hexValue}.");
        }

        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            writer.WriteValue($"#{value.R:X2}{value.G:X2}{value.B:X2}");
        }

        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var value = reader.Value;
            if (value is string hexValue)
            {
                return HexToColor(hexValue);
            }

            throw new JsonSerializationException($"Cannot convert type {value?.GetType()} to color.");
        }
    }
}
