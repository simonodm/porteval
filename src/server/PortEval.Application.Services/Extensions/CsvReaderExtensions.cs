﻿using CsvHelper;
using PortEval.Application.Services.BulkImportExport.ClassMaps;

namespace PortEval.Application.Services.Extensions
{
    public static class CsvReaderExtensions
    {
        public static void RegisterImportClassMaps(this CsvReader csv)
        {
            csv.Context.RegisterClassMap<PortfolioClassMap>();
            csv.Context.RegisterClassMap<PositionImportClassMap>();
            csv.Context.RegisterClassMap<TransactionClassMap>();
            csv.Context.RegisterClassMap<InstrumentClassMap>();
            csv.Context.RegisterClassMap<InstrumentPriceClassMap>();
        }

        public static void RegisterExportClassMaps(this CsvReader csv)
        {
            csv.Context.RegisterClassMap<PortfolioClassMap>();
            csv.Context.RegisterClassMap<PositionExportClassMap>();
            csv.Context.RegisterClassMap<TransactionClassMap>();
            csv.Context.RegisterClassMap<InstrumentClassMap>();
            csv.Context.RegisterClassMap<InstrumentPriceClassMap>();
        }
    }
}