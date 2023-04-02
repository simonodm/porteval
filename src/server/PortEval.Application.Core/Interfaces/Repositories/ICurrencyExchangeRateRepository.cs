using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Domain.Models.Entities;

namespace PortEval.Application.Core.Interfaces.Repositories
{
    /// <summary>
    /// Represents a persistently stored collection of currency exchange rates.
    /// </summary>
    public interface ICurrencyExchangeRateRepository : IRepository
    {
        /// <summary>
        /// Lists all exchange rates from the provided currency, sorted by descending time.
        /// </summary>
        /// <param name="currencyFrom">Code of the currency to retrieve exchange rates of.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains an <see cref="IEnumerable{T}"/> containing exchange rates from the provided currency.
        /// </returns>
        public Task<IEnumerable<CurrencyExchangeRate>> ListExchangeRatesAsync(string currencyFrom);

        /// <summary>
        /// Retrieves the latest available exchange rate between the specified currencies at the specified time.
        /// </summary>
        /// <param name="currencyFrom">Source currency code.</param>
        /// <param name="currencyTo">Target currency code.</param>
        /// <param name="time">Time to find exchange rate at.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains the requested exchange rate if such exists, <c>null</c> otherwise.
        /// </returns>
        public Task<CurrencyExchangeRate> GetExchangeRateAtAsync(string currencyFrom, string currencyTo, DateTime time);

        /// <summary>
        /// Adds an exchange rate to the repository.
        /// </summary>
        /// <param name="exchangeRate">Exchange rate to add.</param>
        /// <returns>The added entity.</returns>
        public CurrencyExchangeRate Add(CurrencyExchangeRate exchangeRate);

        /// <summary>
        /// Upserts the provided exchange rates into the repository, performing matching based on source currency code, target currency code, and time.
        /// </summary>
        /// <remarks>
        /// For performance reasons, this operation runs in its own unit of work, so it is committed automatically.
        /// </remarks>
        /// <param name="rates">Exchange rates to upsert into the repository.</param>
        /// <returns>A task representing the asynchronous bulk upsert operation.</returns>
        public Task BulkUpsertAsync(IList<CurrencyExchangeRate> rates);
    }
}
