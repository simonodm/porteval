using CsvHelper;
using PortEval.Application.Features.Services.BulkImportExport;
using PortEval.Application.Features.Services.BulkImportExport.ClassMaps;
using PortEval.Application.Features.Services.BulkImportExport.Interfaces;

namespace PortEval.Application.Features.Extensions
{
    public static class CsvWriterExtensions
    {
        public static void RegisterImportClassMaps(this CsvWriter csv)
        {
            csv.Context.RegisterClassMap<PortfolioClassMap>();
            csv.Context.RegisterClassMap<PositionImportClassMap>();
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

        public static void WriteErrorEntry<T>(this CsvWriter csv, ProcessedRowErrorLogEntry<T> entry)
        {
            csv.WriteRecord(entry.Data);
            csv.WriteErrorField(entry);
            csv.NextRecord();
        }

        public static void WriteErrorEntry(this CsvWriter csv, RawRowErrorLogEntry entry)
        {
            foreach (var field in entry.RawRowFields)
            {
                csv.WriteField(field);
            }
            csv.WriteErrorField(entry);
            csv.NextRecord();
        }

        private static void WriteErrorField(this CsvWriter csv, IErrorLogEntry entry)
        {
            csv.WriteField(entry.IsError ? string.Join(' ', entry.ErrorMessages) : "OK");
        }
    }
}
