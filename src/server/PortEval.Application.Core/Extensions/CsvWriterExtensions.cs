using CsvHelper;
using PortEval.Application.Core.Common.BulkImport;
using PortEval.Application.Core.Interfaces;
using PortEval.Application.Models.DTOs.Converters.ClassMaps;

namespace PortEval.Application.Core.Extensions;

/// <summary>
///     Implements extension methods on CsvHelper's <see cref="CsvWriter" /> class.
/// </summary>
public static class CsvWriterExtensions
{
    /// <summary>
    ///     Registers CsvHelper class maps for PortEval's CSV import templates.
    /// </summary>
    /// <param name="csv">A <see cref="CsvWriter" /> instance to register class maps on.</param>
    public static void RegisterImportClassMaps(this CsvWriter csv)
    {
        csv.Context.RegisterClassMap<PortfolioClassMap>();
        csv.Context.RegisterClassMap<PositionImportClassMap>();
        csv.Context.RegisterClassMap<TransactionClassMap>();
        csv.Context.RegisterClassMap<InstrumentClassMap>();
        csv.Context.RegisterClassMap<InstrumentPriceClassMap>();
    }

    /// <summary>
    ///     Registers CsvHelper class maps for PortEval's CSV export templates.
    /// </summary>
    /// <param name="csv">A <see cref="CsvWriter" /> instance to register class maps on.</param>
    public static void RegisterExportClassMaps(this CsvWriter csv)
    {
        csv.Context.RegisterClassMap<PortfolioClassMap>();
        csv.Context.RegisterClassMap<PositionExportClassMap>();
        csv.Context.RegisterClassMap<TransactionClassMap>();
        csv.Context.RegisterClassMap<InstrumentClassMap>();
        csv.Context.RegisterClassMap<InstrumentPriceClassMap>();
    }

    /// <summary>
    ///     Writes CSV headers to an error log file of an import of records of type <see cref="T" />.
    /// </summary>
    /// <param name="csv">A <see cref="CsvWriter" /> instance to write headers to.</param>
    /// <typeparam name="T">Type of imported records.</typeparam>
    public static void WriteErrorHeaders<T>(this CsvWriter csv)
    {
        csv.WriteHeader<T>();
        csv.WriteField("Error");
        csv.NextRecord();
    }

    /// <summary>
    ///     Writes an error log entry to the CSV file.
    /// </summary>
    /// <param name="csv">A <see cref="CsvWriter" /> instance to write entry to.</param>
    /// <param name="entry">Entry to write to the file.</param>
    /// <typeparam name="T">Type of imported records.</typeparam>
    public static void WriteErrorEntry<T>(this CsvWriter csv, ProcessedRowErrorLogEntry<T> entry)
    {
        csv.WriteRecord(entry.Data);
        csv.WriteErrorField(entry);
        csv.NextRecord();
    }

    /// <summary>
    ///     Writes an error log entry to the CSV file.
    /// </summary>
    /// <param name="csv">A <see cref="CsvWriter" /> instance to write entry to.</param>
    /// <param name="entry">Entry to write to the file.</param>
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