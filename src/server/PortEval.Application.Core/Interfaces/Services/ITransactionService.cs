using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Core.Interfaces.Services
{
    /// <summary>
    /// Handles creation, modification and removal of transactions.
    /// </summary>
    public interface ITransactionService
    {
        /// <summary>
        /// Retrieves all transactions of the given position in the specified date range.
        /// </summary>
        /// <param name="filters">Filter settings.</param>
        /// <param name="dateRange">Date range of allowed transactions.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing retrieved transactions.
        /// </returns>
        public Task<OperationResponse<IEnumerable<TransactionDto>>> GetTransactionsAsync(TransactionFilters filters,
            DateRangeParams dateRange);

        /// <summary>
        /// Retrieves the specified transaction.
        /// </summary>
        /// <param name="transactionId">Transaction ID.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains a <see cref="OperationResponse{T}"/> containing the retrieved transaction.
        /// </returns>
        public Task<OperationResponse<TransactionDto>> GetTransactionAsync(int transactionId);

        /// <summary>
        /// Creates a transaction according to the supplied DTO.
        /// </summary>
        /// <param name="options">A DTO containing client's request body.</param>
        /// <returns>
        /// A task representing the asynchronous creation operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing the created transaction.
        /// </returns>
        public Task<OperationResponse<TransactionDto>> AddTransactionAsync(TransactionDto options);

        /// <summary>
        /// Modifies a transaction according to the supplied DTO.
        /// </summary>
        /// <param name="options">A DTO containing client's request body.</param>
        /// <returns>
        /// A task representing the asynchronous update operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing the updated transaction.
        /// </returns>
        public Task<OperationResponse<TransactionDto>> UpdateTransactionAsync(TransactionDto options);

        /// <summary>
        /// Deletes a transaction.
        /// </summary>
        /// <param name="transactionId">Transaction ID.</param>
        /// <returns>
        /// A task representing the asynchronous deletion operation.
        /// Task result contains an <see cref="OperationResponse"/> representing the status of the operation.
        /// </returns>
        public Task<OperationResponse> DeleteTransactionAsync(int transactionId);
    }
}
