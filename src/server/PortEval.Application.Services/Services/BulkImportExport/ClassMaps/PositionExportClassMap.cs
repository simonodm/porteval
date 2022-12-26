﻿namespace PortEval.Application.Features.Services.BulkImportExport.ClassMaps
{
    public sealed class PositionExportClassMap : PositionClassMap
    {
        public PositionExportClassMap()
        {
            Map(p => p.Instrument.Symbol).Name("Instrument symbol");
            Map(p => p.Instrument.Name).Name("Instrument name");
        }
    }
}