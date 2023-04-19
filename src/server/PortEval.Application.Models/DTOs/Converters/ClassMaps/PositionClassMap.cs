using CsvHelper.Configuration;

namespace PortEval.Application.Models.DTOs.Converters.ClassMaps;

public abstract class PositionClassMap : ClassMap<PositionDto>
{
    protected PositionClassMap()
    {
        Map(p => p.Id).Name("Position ID").Default(default(int));
        Map(p => p.InstrumentId).Name("Instrument ID");
        Map(p => p.PortfolioId).Name("Portfolio ID");
        Map(p => p.Note).Name("Note");
    }
}