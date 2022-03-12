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

        /// <summary>
        /// Converts the provided value between the specified currencies. If no direct exchange rate between the provided currencies is available, this method
        /// attempts to approximate the exchange rate by doing an indirect conversion to the application default currency.
        /// </summary>
        /// <param name="baseCurrencyCode">Currency to convert from.</param>
        /// <param name="targetCurrencyCode">Currency to convert to.</param>
        /// <param name="price">Value to convert.</param>
        /// <param name="time">Date and time of exchange rate to use.</param>
        /// <returns>A task representing the asynchronous database access operation. Task result contains a decimal representing the converted value.</returns>
        public Task<decimal> Convert(string baseCurrencyCode, string targetCurrencyCode, decimal price, DateTime time);
    }
}
