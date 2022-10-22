using System;

namespace PortEval.Application.Services.BulkImportExport.ClassMaps
{
    public sealed class PositionImportClassMap : PositionClassMap
    {
        public PositionImportClassMap()
        {
            Map(p => p.Time).Name("Time").Default((DateTime?)null);
            Map(p => p.Amount).Name("Amount").Default((decimal?)null);
            Map(p => p.Price).Name("Price").Default((decimal?)null);
        }
    }
}
