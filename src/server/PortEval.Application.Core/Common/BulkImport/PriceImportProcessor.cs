using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators;

namespace PortEval.Application.Core.Common.BulkImport;

/// <summary>
///     Enables bulk import of instrument prices.
/// </summary>
public class PriceImportProcessor : ImportProcessor<InstrumentPriceDto, InstrumentPriceDtoValidator>
{
    private readonly IInstrumentPriceService _priceService;

    /// <summary>
    ///     Initializes the import processor.
    /// </summary>
    public PriceImportProcessor(IInstrumentPriceService priceService)
    {
        _priceService = priceService;
    }

    /// <inheritdoc />
    protected override async Task<ProcessedRowErrorLogEntry<InstrumentPriceDto>> ProcessItem(InstrumentPriceDto row)
    {
        var logEntry = new ProcessedRowErrorLogEntry<InstrumentPriceDto>(row);
        var existingPrice = await _priceService.GetInstrumentPriceAsync(row.InstrumentId, row.Time);
        if (existingPrice.Status == OperationStatus.NotFound || existingPrice.Response.Time != row.Time)
        {
            var newPrice = await _priceService.AddPricePointAsync(row);
            if (newPrice.Status != OperationStatus.Ok)
            {
                logEntry.AddError(newPrice.Message);
            }

            logEntry.Data.Id = newPrice.Response?.Id ?? default;
        }
        else
        {
            logEntry.AddError("Price already exists.");
        }

        return logEntry;
    }
}