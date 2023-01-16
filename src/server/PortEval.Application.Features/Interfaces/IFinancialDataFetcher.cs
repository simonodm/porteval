﻿using PortEval.Application.Models.PriceFetcher;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Interfaces
{
    /// <summary>
    /// Aggregates multiple API clients supporting retrieval of instrument prices and currency exchange rates.
    /// </summary>
    public interface IFinancialDataFetcher
    {
        /// <summary>
        /// Retrieves historical daily prices of the supplied symbol in the supplied range.
        /// </summary>
        /// <param name="symbol">Symbol to retrieve prices for.</param>
        /// <param name="currencyCode">Currency code of the symbol</param>
        /// <param name="from">Date range start</param>
        /// <param name="to">Date range end</param>
        /// <returns>A Response object containing operation status and historical prices if the operation was successful.</returns>
        public Task<IEnumerable<PricePoint>> GetHistoricalDailyPrices(string symbol, string currencyCode, DateTime from, DateTime to);

        /// <summary>
        /// Retrieves intraday prices of the supplied symbol in the supplied range.
        /// </summary>
        /// <param name="symbol">Symbol to retrieve prices for.</param>
        /// <param name="currencyCode">Currency code of the symbol</param>
        /// <param name="from">Range start</param>
        /// <param name="to">Range end</param>
        /// <param name="interval">Interval between individual price points</param>
        /// <returns>A Response object containing operation status and intraday prices if the operation was successful.</returns>
        public Task<IEnumerable<PricePoint>> GetIntradayPrices(string symbol, string currencyCode, DateTime from, DateTime to,
            IntradayInterval interval = IntradayInterval.OneHour);

        /// <summary>
        /// Retrieves the latest price of the supplied symbol.
        /// </summary>
        /// <param name="symbol">Symbol to retrieve prices for.</param>
        /// <param name="currencyCode">Currency code of the symbol</param>
        /// <returns>A Response object containing operation status and latest price point if the operation was successful.</returns>
        public Task<PricePoint> GetLatestInstrumentPrice(string symbol, string currencyCode);

        /// <summary>
        /// Retrieves historical daily prices of the supplied cryptocurrency in the supplied range.
        /// </summary>
        /// <param name="symbol">Symbol to retrieve prices for.</param>
        /// <param name="targetCurrency">Currency to retrieve crypto price in.</param>
        /// <param name="from">Date range start</param>
        /// <param name="to">Date range end</param>
        /// <returns>A Response object containing operation status and historical prices if the operation was successful.</returns>
        public Task<IEnumerable<PricePoint>> GetHistoricalDailyCryptoPrices(string symbol, string targetCurrency,
            DateTime from, DateTime to);

        /// <summary>
        /// Retrieves intraday prices of the supplied cryptocurrency in the supplied range.
        /// </summary>
        /// <param name="symbol">Symbol of the cryptocurrency to retrieve prices for.</param>
        /// <param name="targetCurrency">Currency to retrieve crypto price in.</param>
        /// <param name="from">Range start</param>
        /// <param name="to">Range end</param>
        /// <param name="interval">Interval between individual price points</param>
        /// <returns>A Response object containing operation status and intraday prices if the operation was successful.</returns>
        public Task<IEnumerable<PricePoint>> GetIntradayCryptoPrices(string symbol, string targetCurrency,
            DateTime from, DateTime to, IntradayInterval interval = IntradayInterval.OneHour);

        /// <summary>
        /// Retrieves the latest price of the supplied cryptocurrency.
        /// </summary>
        /// <param name="symbol">Symbol to retrieve prices for.</param>
        /// <param name="targetCurrency">Currency to retrieve crypto price in.</param>
        /// <returns>A Response object containing operation status and latest price point if the operation was successful.</returns>
        public Task<PricePoint> GetLatestCryptoPrice(string symbol, string targetCurrency);

        /// <summary>
        /// Retrieves the latest exchange rates of the supplied currency.
        /// </summary>
        /// <param name="baseCurrency">Base currency code</param>
        /// <returns>A Response object containing operation status and the latest exchange rates if the operation was successful.</returns>
        public Task<ExchangeRates> GetLatestExchangeRates(string baseCurrency);

        /// <summary>
        /// Retrieves historical daily exchange rates of the supplied currency in the supplied range.
        /// </summary>
        /// <param name="baseCurrency">Base currency code</param>
        /// <param name="from">Date range start</param>
        /// <param name="to">Date range end</param>
        /// <returns>A Response object containing operation status and the retrieved exchange rates if the operation was successful.</returns>
        public Task<IEnumerable<ExchangeRates>> GetHistoricalDailyExchangeRates(string baseCurrency, DateTime from, DateTime to);

        /// <summary>
        /// Retrieves historical stock splits of the supplied symbol in the supplied range.
        /// </summary>
        /// <param name="symbol">Symbol to retrieve splits for.</param>
        /// <param name="from">Date range start.</param>
        /// <param name="to">Date range end.</param>
        /// <returns>A Response object containing operation status and the retrieved instrument splits if the operation was successful.</returns>
        public Task<IEnumerable<InstrumentSplitData>> GetInstrumentSplits(string symbol, DateTime from,
            DateTime to);
    }
}