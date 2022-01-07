using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Models.DTOs.Converters
{
    public class AggregationFrequencyTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(AggregationFrequency) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s && AggregationFrequencyStringValues.StringToEnumMap.ContainsKey(s))
            {
                return AggregationFrequencyStringValues.StringToEnumMap[s];
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value.GetType() == typeof(AggregationFrequency) && destinationType == typeof(string))
            {
                return AggregationFrequencyStringValues.EnumToStringMap[(AggregationFrequency)value];
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
