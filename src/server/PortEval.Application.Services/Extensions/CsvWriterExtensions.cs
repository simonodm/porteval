﻿using CsvHelper;
using PortEval.Application.Services.BulkImportExport;
using PortEval.Application.Services.BulkImportExport.ClassMaps;

namespace PortEval.Application.Services.Extensions
{
    public static class CsvWriterExtensions
    {
        public static void RegisterImportClassMaps(this CsvWriter csv)
        {
            csv.Context.RegisterClassMap<PortfolioClassMap>();
            csv.Context.RegisterClassMap<PositionClassMap>();
            csv.Context.RegisterClassMap<TransactionClassMap>();
            csv.Context.RegisterClassMap<InstrumentClassMap>();
            csv.Context.RegisterClassMap<InstrumentPriceClassMap>();
        }

        public static void RegisterExportClassMaps(this CsvWriter csv)
        {
            csv.Context.RegisterClassMap<PortfolioClassMap>();
            csv.Context.RegisterClassMap<PositionExportClassMap>();
            csv.Context.RegisterClassMap<TransactionClassMap>();
            csv.Context.RegisterClassMap<InstrumentClassMap>();
            csv.Context.RegisterClassMap<InstrumentPriceClassMap>();
        }

        public static void WriteErrorHeaders<T>(this CsvWriter csv)
        {
            csv.WriteHeader<T>();
            csv.WriteField("Error");
            csv.NextRecord();
        }

        public static void WriteErrorEntry<T>(this CsvWriter csv, ErrorLogEntry<T> entry)
        {
            csv.WriteRecord(entry.Row);
            csv.WriteField(entry.IsError ? string.Join(' ', entry.ErrorMessages) : "OK");
            csv.NextRecord();
        }
    }
}
