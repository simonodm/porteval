using PortEval.Application.Features.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Services.BulkImportExport
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
                await _transactionService.UpdateTransactionAsync(row);
            }
            else
            {
                var transaction = await _transactionService.AddTransactionAsync(row);
                logEntry.Data.Id = transaction.Id;
            }

            return logEntry;
        }
    }
}
