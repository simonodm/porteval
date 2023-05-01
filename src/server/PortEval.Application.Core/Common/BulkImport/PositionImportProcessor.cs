using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators;

namespace PortEval.Application.Core.Common.BulkImport;

/// <summary>
///     Enables bulk import of position records.
/// </summary>
public class PositionImportProcessor : ImportProcessor<PositionDto, PositionDtoValidator>
{
    private readonly IPositionService _positionService;

    /// <summary>
    ///     Initializes the import processor.
    /// </summary>
    public PositionImportProcessor(IPositionService positionService)
    {
        _positionService = positionService;
    }

    /// <inheritdoc />
    protected override async Task<ProcessedRowErrorLogEntry<PositionDto>> ProcessItem(PositionDto row)
    {
        var logEntry = new ProcessedRowErrorLogEntry<PositionDto>(row);

        if (row.Id != default)
        {
            var response = await _positionService.UpdatePositionAsync(row);
            if (response.Status != OperationStatus.Ok)
            {
                logEntry.AddError(response.Message);
            }
        }
        else
        {
            var response = await _positionService.OpenPositionAsync(row);
            if (response.Status != OperationStatus.Ok)
            {
                logEntry.AddError(response.Message);
            }

            logEntry.Data.Id = response.Response?.Id ?? default;
        }

        return logEntry;
    }
}