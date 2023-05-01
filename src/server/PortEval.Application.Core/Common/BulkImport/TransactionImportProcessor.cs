using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators;

namespace PortEval.Application.Core.Common.BulkImport;

/// <summary>
///     Enables bulk import of transaction records.
/// </summary>
public class TransactionImportProcessor : ImportProcessor<TransactionDto, TransactionDtoValidator>
{
    private readonly ITransactionService _transactionService;

    /// <summary>
    ///     Initializes the import processor.
    /// </summary>
    public TransactionImportProcessor(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    /// <inheritdoc />
    protected override async Task<ProcessedRowErrorLogEntry<TransactionDto>> ProcessItem(TransactionDto row)
    {
        var logEntry = new ProcessedRowErrorLogEntry<TransactionDto>(row);

        if (row.Id != default)
        {
            var response = await _transactionService.UpdateTransactionAsync(row);
            if (response.Status != OperationStatus.Ok)
            {
                logEntry.AddError(response.Message);
            }
        }
        else
        {
            var response = await _transactionService.AddTransactionAsync(row);
            if (response.Status != OperationStatus.Ok)
            {
                logEntry.AddError(response.Message);
            }

            logEntry.Data.Id = response.Response?.Id ?? default;
        }

        return logEntry;
    }
}