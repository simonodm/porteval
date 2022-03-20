using System;
using CsvHelper.Configuration;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Services.BulkImportExport.ClassMaps
{
    public sealed class PositionClassMap : ClassMap<PositionDto>
    {
        public PositionClassMap()
        {
            Map(p => p.Id).Name("Position ID").Default(default(int));
            Map(p => p.InstrumentId).Name("Instrument ID");
            Map(p => p.PortfolioId).Name("Portfolio ID");
            Map(p => p.Note).Name("Note");
            Map(p => p.Time).Name("Time").Default((DateTime?)null);
            Map(p => p.Amount).Name("Amount").Default((decimal?)null);
            Map(p => p.Price).Name("Price").Default((decimal?)null);
        }
    }
}
