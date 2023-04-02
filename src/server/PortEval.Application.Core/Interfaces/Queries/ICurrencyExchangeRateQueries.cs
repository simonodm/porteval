using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Core.Queries;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;

namespace PortEval.Application.Core.Interfaces.Queries
{
    /// <summary>
    /// Implements high performance read-only currency exchange rate queries.
    /// </summary>
    public interface ICurrencyExchangeRateQueries
    {
        /// <summary>
        /// Retrieves available exchange rates of the provided base currency at the specified time.
        /// </summary>
        /// <param name="currencyCode">Base currency code.</param>
        /// <param name="time">Time to retrieve exchange rates at.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains an <c>IEnumerable</c> containing the exchange rate DTOs.</returns>
        public Task<QueryResponse<IEnumerable<CurrencyExchangeRateDto>>> GetExchangeRates(string currencyCode, DateTime time);

        /// <summary>
        /// Retrieves all exchange rates between the specified currencies ordered by time.
        /// </summary>
        /// <param name="baseCurrencyCode">Base currency code.</param>
        /// <param name="targetCurrencyCode">Target currency code.</param>
        /// <param name="dateRange">Date range in which the exchange rates should be retrieved.</param>
        /// <returns>A task representing the asynchronous database query. Task result contains an <c>IEnumerable</c> containing exchange rate DTOs.</returns>
        public Task<QueryResponse<IEnumerable<CurrencyExchangeRateDto>>> GetExchangeRates(string baseCurrencyCode,
            string targetCurrencyCode, DateRangeParams dateRange);

        /// <summary>
        /// Retrieves the last available exchange rate between the specified currencies before or equal to the specified time.
        /// </summary>
        /// <param name="baseCurrencyCode">Base currency code.</param>
        /// <param name="targetCurrencyCode">Target currency code.</param>
        /// <param name="time">Exchange rate time.</param>
        /// <returns>A task representing the asynchronous database query. Task result contains the latest currency exchange rate DTO before the specified time.</returns>
        public Task<QueryResponse<CurrencyExchangeRateDto>> GetExchangeRateAt(string baseCurrencyCode, string targetCurrencyCode,
            DateTime time);
    }
}
