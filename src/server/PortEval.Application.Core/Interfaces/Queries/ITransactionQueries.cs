using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;

namespace PortEval.Application.Core.Interfaces.Queries;

/// <summary>
///     Implements queries for transactions stored in the application's persistent storage.
/// </summary>
public interface ITransactionQueries
{
    /// <summary>
    ///     Retrieves transactions according to the specified filter and date range.
    /// </summary>
    /// <param name="filters">Transaction filters.</param>
    /// <param name="from">Date to retrieve transactions from.</param>
    /// <param name="to">Date to retrieve transactions until.</param>
    /// <returns>
    ///     A task representing the asynchronous query operation.
    ///     Task result contains an <see cref="IEnumerable{T}" /> containing retrieved transactions.
    /// </returns>
    Task<IEnumerable<TransactionDto>> GetTransactionsAsync(TransactionFilters filters, DateTime from,
        DateTime to);

    /// <summary>
    ///     Retrieves a transaction by ID.
    /// </summary>
    /// <param name="transactionId">ID of the transaction to retrieve.</param>
    /// <returns>
    ///     A task representing the asynchronous query operation.
    ///     Task result contains the retrieved transaction if it exists, <c>null</c> otherwise.
    /// </returns>
    Task<TransactionDto> GetTransactionAsync(int transactionId);
}