using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Models.DTOs.Converters.ClassMaps;

public sealed class InstrumentClassMap : ClassMap<InstrumentDto>
{
    public InstrumentClassMap()
    {
        Map(i => i.Id).Name("Instrument ID").Default(default(int));
        Map(i => i.Symbol).Name("Symbol");
        Map(i => i.Name).Name("Name");
        Map(i => i.Exchange).Name("Exchange");
        Map(i => i.Type).Name("Type").TypeConverter(new EnumConverter(typeof(InstrumentType)));
        Map(i => i.Type).TypeConverterOption.EnumIgnoreCase();
        Map(i => i.CurrencyCode).Name("Currency");
        Map(i => i.Note).Name("Note");
    }
}