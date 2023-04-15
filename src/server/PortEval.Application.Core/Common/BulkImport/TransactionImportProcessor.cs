using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators;
using System.Threading.Tasks;

namespace PortEval.Application.Core.Common.BulkImportExport
{
    public class TransactionImportProcessor : ImportProcessor<TransactionDto, TransactionDtoValidator>
    {
        private readonly ITransactionService _transactionService;

        public TransactionImportProcessor(ITransactionService transactionService) : base()
        {
            _transactionService = transactionService;
        }

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
}
