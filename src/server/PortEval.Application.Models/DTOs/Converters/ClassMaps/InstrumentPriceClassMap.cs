using CsvHelper.Configuration;

namespace PortEval.Application.Models.DTOs.Converters.ClassMaps;

public sealed class InstrumentPriceClassMap : ClassMap<InstrumentPriceDto>
{
    public InstrumentPriceClassMap()
    {
        Map(p => p.InstrumentId).Name("Instrument ID");
        Map(p => p.Price).Name("Price");
        Map(p => p.Time).Name("Time");
    }
}