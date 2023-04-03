using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Core.Queries;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Interfaces.Queries
{
    /// <summary>
    /// Implements high performance read-only currency queries.
    /// </summary>
    public interface ICurrencyQueries
    {
        /// <summary>
        /// Retrieves all known currencies.
        /// </summary>
        /// <returns>A task representing the asynchronous database query. Task result contains a <see cref="QueryResponse{T}"/> wrapper over an <c>IEnumerable</c> of stored currencies.</returns>
        public Task<QueryResponse<IEnumerable<CurrencyDto>>> GetAllCurrencies();

        /// <summary>
        /// Retrieves the specified currency.
        /// </summary>
        /// <param name="currencyCode">Code of the currency to retrieve.</param>
        /// <returns>A task representing the asynchronous database query. Task result contains a <see cref="QueryResponse{T}"/> wrapper over the currency DTO with the specified code if it exists, null otherwise.</returns>
        public Task<QueryResponse<CurrencyDto>> GetCurrency(string currencyCode);
    }
}
