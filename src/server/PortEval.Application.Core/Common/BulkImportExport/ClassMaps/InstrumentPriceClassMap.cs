using CsvHelper.Configuration;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Common.BulkImportExport.ClassMaps
{
    public sealed class InstrumentPriceClassMap : ClassMap<InstrumentPriceDto>
    {
        public InstrumentPriceClassMap()
        {
            Map(p => p.Id).Name("Price ID").Default(default(int));
            Map(p => p.InstrumentId).Name("Instrument ID");
            Map(p => p.Price).Name("Price");
            Map(p => p.Time).Name("Time");
        }
    }
}
