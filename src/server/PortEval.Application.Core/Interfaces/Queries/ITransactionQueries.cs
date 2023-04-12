using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Core.Interfaces.Queries
{
    public interface ITransactionQueries
    {
        Task<IEnumerable<TransactionDto>> GetTransactionsAsync(TransactionFilters filters, DateTime from,
            DateTime to);

        Task<TransactionDto> GetTransactionAsync(int transactionId);
    }
}