using System;
using CsvHelper.Configuration;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Services.BulkImportExport.ClassMaps
{
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
}
