using PortEval.Application.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Queries.Interfaces
{
    /// <summary>
    /// High performance read-only currency queries.
    /// </summary>
    public interface ICurrencyQueries
    {
        /// <summary>
        /// Retrieves all known currencies.
        /// </summary>
        /// <returns>A task representing the asynchronous database query. Task result contains an <c>IEnumerable</c> of stored currencies.</returns>
        public Task<QueryResponse<IEnumerable<CurrencyDto>>> GetAllCurrencies();

        /// <summary>
        /// Retrieves the specified currency.
        /// </summary>
        /// <param name="currencyCode">Code of the currency to retrieve.</param>
        /// <returns>A task representing the asynchronous database query. Task result contains the currency DTO with the specified code if it exists, null otherwise.</returns>
        public Task<QueryResponse<CurrencyDto>> GetCurrency(string currencyCode);
    }
}
