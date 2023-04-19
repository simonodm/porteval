using CsvHelper;
using PortEval.Application.Models.DTOs.Converters.ClassMaps;

namespace PortEval.Application.Core.Extensions;

/// <summary>
///     Implements extension methods on CsvHelper's <see cref="CsvReader" /> class.
/// </summary>
public static class CsvReaderExtensions
{
    /// <summary>
    ///     Registers CsvHelper class maps for PortEval's CSV import templates.
    /// </summary>
    /// <param name="csv">A <see cref="CsvReader" /> instance to register class maps on.</param>
    public static void RegisterImportClassMaps(this CsvReader csv)
    {
        csv.Context.RegisterClassMap<PortfolioClassMap>();
        csv.Context.RegisterClassMap<PositionImportClassMap>();
        csv.Context.RegisterClassMap<TransactionClassMap>();
        csv.Context.RegisterClassMap<InstrumentClassMap>();
        csv.Context.RegisterClassMap<InstrumentPriceClassMap>();
    }
}