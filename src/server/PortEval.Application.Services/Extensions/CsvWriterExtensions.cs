using CsvHelper;
using PortEval.Application.Services.BulkImportExport;

namespace PortEval.Application.Services.Extensions
{
    internal static class CsvWriterExtensions
    {
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
