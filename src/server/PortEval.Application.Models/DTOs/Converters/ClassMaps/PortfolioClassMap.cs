using CsvHelper.Configuration;

namespace PortEval.Application.Models.DTOs.Converters.ClassMaps;

public sealed class PortfolioClassMap : ClassMap<PortfolioDto>
{
    public PortfolioClassMap()
    {
        Map(p => p.Id).Name("Portfolio ID").Default(default(int));
        Map(p => p.Name).Name("Name");
        Map(p => p.CurrencyCode).Name("Currency");
        Map(p => p.Note).Name("Note");
    }
}