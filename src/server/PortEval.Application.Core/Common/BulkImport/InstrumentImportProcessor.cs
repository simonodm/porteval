using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators;

namespace PortEval.Application.Core.Common.BulkImport;

/// <summary>
///     Enables bulk import of instrument records.
/// </summary>
public class InstrumentImportProcessor : ImportProcessor<InstrumentDto, InstrumentDtoValidator>
{
    private readonly IInstrumentService _instrumentService;

    /// <summary>
    ///     Initializes the import processor.
    /// </summary>
    public InstrumentImportProcessor(IInstrumentService instrumentService)
    {
        _instrumentService = instrumentService;
    }

    /// <inheritdoc />
    protected override async Task<ProcessedRowErrorLogEntry<InstrumentDto>> ProcessItem(InstrumentDto row)
    {
        var logEntry = new ProcessedRowErrorLogEntry<InstrumentDto>(row);

        if (row.Id != default)
        {
            var response = await _instrumentService.UpdateInstrumentAsync(row);
            if (response.Status != OperationStatus.Ok)
            {
                logEntry.AddError(response.Message);
            }
        }
        else
        {
            var response = await _instrumentService.CreateInstrumentAsync(row);
            if (response.Status != OperationStatus.Ok)
            {
                logEntry.AddError(response.Message);
            }

            logEntry.Data.Id = response.Response?.Id ?? default;
        }

        return logEntry;
    }
}