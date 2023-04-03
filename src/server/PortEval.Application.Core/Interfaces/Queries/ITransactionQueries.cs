﻿using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Core.Queries;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;

namespace PortEval.Application.Core.Interfaces.Queries
{
    /// <summary>
    /// Implements high performance read-only transaction queries.
    /// </summary>
    public interface ITransactionQueries
    {
        /// <summary>
        /// Retrieves all transactions of the given position in the specified date range.
        /// </summary>
        /// <param name="filters">Filter settings.</param>
        /// <param name="dateRange">Date range of allowed transactions.</param>
        /// <returns>A task representing the asynchronous database query. Task result contains a <see cref="QueryResponse{T}"/> wrapper over an <c>IEnumerable</c> of retrieved transaction DTOs.</returns>
        public Task<QueryResponse<IEnumerable<TransactionDto>>> GetTransactions(TransactionFilters filters, DateRangeParams dateRange);

        /// <summary>
        /// Retrieves the specified transaction.
        /// </summary>
        /// <param name="transactionId">Transaction ID.</param>
        /// <returns>A task representing the asynchronous database query. Task result contains a <see cref="QueryResponse{T}"/> wrapper over the retrieved transaction DTO if it exists, null otherwise.</returns>
        public Task<QueryResponse<TransactionDto>> GetTransaction(int transactionId);
    }
}
