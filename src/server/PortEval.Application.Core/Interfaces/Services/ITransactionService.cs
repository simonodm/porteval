using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Models.Entities;

namespace PortEval.Application.Core.Interfaces.Services
{
    /// <summary>
    /// Handles creation, modification and removal of transactions.
    /// </summary>
    public interface ITransactionService
    {
        /// <summary>
        /// Creates a transaction according to the supplied DTO.
        /// </summary>
        /// <param name="options">A DTO containing client's request body.</param>
        /// <returns>A task representing the asynchronous creation operation. Task result contains newly created transaction entity.</returns>
        public Task<Transaction> AddTransactionAsync(TransactionDto options);

        /// <summary>
        /// Modifies a transaction according to the supplied DTO.
        /// </summary>
        /// <param name="options">A DTO containing client's request body.</param>
        /// <returns>A task representing the asynchronous update operation. Task result contains the updated transaction entity.</returns>
        public Task<Transaction> UpdateTransactionAsync(TransactionDto options);

        /// <summary>
        /// Deletes a transaction.
        /// </summary>
        /// <param name="transactionId">Transaction ID.</param>
        /// <returns>A task representing the asynchronous deletion operation.</returns>
        public Task DeleteTransactionAsync(int transactionId);
    }
}
