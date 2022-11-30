using PortEval.Domain.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Services.Interfaces.Repositories
{
    /// <summary>
    /// Represents a persistently stored collection of currencies.
    /// </summary>
    public interface ICurrencyRepository : IRepository
    {
        /// <summary>
        /// Lists all currencies.
        /// </summary>
        /// <returns>A task representing the asynchronous retrieval operation. The task result contains an <c>IEnumerable</c> containing all existing currencies.</returns>
        public Task<IEnumerable<Currency>> ListAllAsync();

        /// <summary>
        /// Finds a currency by code.
        /// </summary>
        /// <param name="currencyCode">Currency code.</param>
        /// <returns>A task representing the asynchronous search operation. The task result contains the currency entity with the supplied ID if it exists, null otherwise.</returns>
        public Task<Currency> FindAsync(string currencyCode);

        /// <summary>
        /// Gets the application-wide default currency.
        /// </summary>
        /// <returns>A task representing the asynchronous retrieval operation. The task result contains the default currency entity.</returns>
        public Task<Currency> GetDefaultCurrencyAsync();

        /// <summary>
        /// Updates a currency.
        /// </summary>
        /// <param name="currency">Updated currency entity.</param>
        /// <returns>The updated currency.</returns>
        public Currency Update(Currency currency);

        /// <summary>
        /// Checks whether a currency with the supplied currency code exists.
        /// </summary>
        /// <param name="currencyCode">Currency code.</param>
        /// <returns>true if a currency with the supplied currency code exists in the database, false otherwise</returns>
        public Task<bool> ExistsAsync(string currencyCode);
    }
}
