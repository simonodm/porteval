using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Interfaces.Services
{
    /// <summary>
    /// Provides operations to retrieve and modify currencies.
    /// </summary>
    public interface ICurrencyService
    {
        /// <summary>
        /// Retrieves all known currencies.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing all available currencies.
        /// </returns>
        public Task<OperationResponse<IEnumerable<CurrencyDto>>> GetAllCurrenciesAsync();
        
        /// <summary>
        /// Retrieves a currency by its three-letter code.
        /// </summary>
        /// <param name="currencyCode">Code of the currency.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing the retrieved currency if it exists.
        /// </returns>
        public Task<OperationResponse<CurrencyDto>> GetCurrencyAsync(string currencyCode);

        /// <summary>
        /// Updates a currency based on supplied options. Only affects the <c>IsDefault</c> field.
        /// </summary>
        /// <param name="options">DTO of the updated currency.</param>
        /// <returns>
        /// A task representing the asynchronous update operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing the updated currency if it exists.
        /// </returns>
        public Task<OperationResponse<CurrencyDto>> UpdateAsync(CurrencyDto options);
    }
}
