using PortEval.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Queries.Interfaces
{
    /// <summary>
    /// High performance read-only currency exchange rate queries.
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
        /// Retrieves the last available exchange rate between the specified currencies before or equal to the specified time.
        /// </summary>
        /// <param name="baseCurrencyCode">Base currency code.</param>
        /// <param name="targetCurrencyCode">Target currency code.</param>
        /// <param name="time">Exchange rate time.</param>
        /// <returns>A task representing the asynchronous database query. Task result contains the latest currency exchange rate DTO before the specified time.</returns>
        public Task<QueryResponse<CurrencyExchangeRateDto>> GetExchangeRateAt(string baseCurrencyCode, string targetCurrencyCode,
            DateTime time);

        /// <summary>
        /// Converts the given chart point from the supplied base currency to the target currency using the exchange rate from the chart point time.
        /// </summary>
        /// <param name="baseCurrencyCode">Base currency code.</param>
        /// <param name="targetCurrencyCode">Target currency code</param>
        /// <param name="chartPoint">Chart point to convert.</param>
        /// <returns>Chart point converted to the target currency.</returns>
        public Task<EntityChartPointDto> ConvertChartPointCurrency(string baseCurrencyCode,
            string targetCurrencyCode, EntityChartPointDto chartPoint);
    }
}
