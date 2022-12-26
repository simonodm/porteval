using CsvHelper.Configuration;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Features.Services.BulkImportExport.ClassMaps
{
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
}
