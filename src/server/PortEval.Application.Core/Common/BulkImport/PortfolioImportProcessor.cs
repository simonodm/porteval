using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators;

namespace PortEval.Application.Core.Common.BulkImport;

/// <summary>
///     Enables bulk import of portfolio records.
/// </summary>
public class PortfolioImportProcessor : ImportProcessor<PortfolioDto, PortfolioDtoValidator>
{
    private readonly IPortfolioService _portfolioService;

    /// <summary>
    ///     Initializes the import processor.
    /// </summary>
    public PortfolioImportProcessor(IPortfolioService portfolioService)
    {
        _portfolioService = portfolioService;
    }

    /// <inheritdoc />
    protected override async Task<ProcessedRowErrorLogEntry<PortfolioDto>> ProcessItem(PortfolioDto row)
    {
        var logEntry = new ProcessedRowErrorLogEntry<PortfolioDto>(row);

        if (row.Id == default)
        {
            var response = await _portfolioService.CreatePortfolioAsync(row);
            if (response.Status != OperationStatus.Ok)
            {
                logEntry.AddError(response.Message);
            }

            logEntry.Data.Id = response.Response?.Id ?? default;
        }
        else
        {
            var response = await _portfolioService.UpdatePortfolioAsync(row);
            if (response.Status != OperationStatus.Ok)
            {
                logEntry.AddError(response.Message);
            }
        }

        return logEntry;
    }
}