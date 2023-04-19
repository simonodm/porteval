using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.FinancialDataFetcher;

namespace PortEval.Application.Core.Interfaces;

/// <summary>
///     A facade supporting retrieval of instrument prices, instrument splits, and currency exchange rates.
/// </summary>
public interface IFinancialDataFetcher
{
    /// <summary>
    ///     Retrieves historical daily prices of the supplied symbol in the supplied range.
    /// </summary>
    /// <param name="symbol">Symbol to retrieve prices for.</param>
    /// <param name="currencyCode">Currency code of the symbol</param>
    /// <param name="from">Date range start</param>
    /// <param name="to">Date range end</param>
    /// <returns>A task representing the asynchronous retrieval operation. Task result contains the retrieved data.</returns>
    public Task<IEnumerable<PricePoint>> GetHistoricalDailyPricesAsync(string symbol, string currencyCode,
        DateTime from, DateTime to);

    /// <summary>
    ///     Retrieves intraday prices of the supplied symbol in the supplied range.
    /// </summary>
    /// <param name="symbol">Symbol to retrieve prices for.</param>
    /// <param name="currencyCode">Currency code of the symbol</param>
    /// <param name="from">Range start</param>
    /// <param name="to">Range end</param>
    /// <param name="interval">Interval between individual price points</param>
    /// <returns>A task representing the asynchronous retrieval operation. Task result contains the retrieved data.</returns>
    public Task<IEnumerable<PricePoint>> GetIntradayPricesAsync(string symbol, string currencyCode, DateTime from,
        DateTime to,
        IntradayInterval interval = IntradayInterval.OneHour);

    /// <summary>
    ///     Retrieves the latest price of the supplied symbol.
    /// </summary>
    /// <param name="symbol">Symbol to retrieve prices for.</param>
    /// <param name="currencyCode">Currency code of the symbol</param>
    /// <returns>A task representing the asynchronous retrieval operation. Task result contains the retrieved data.</returns>
    public Task<PricePoint> GetLatestInstrumentPriceAsync(string symbol, string currencyCode);

    /// <summary>
    ///     Retrieves historical daily prices of the supplied cryptocurrency in the supplied range.
    /// </summary>
    /// <param name="symbol">Symbol to retrieve prices for.</param>
    /// <param name="targetCurrency">Currency to retrieve crypto price in.</param>
    /// <param name="from">Date range start</param>
    /// <param name="to">Date range end</param>
    /// <returns>A task representing the asynchronous retrieval operation. Task result contains the retrieved data.</returns>
    public Task<IEnumerable<PricePoint>> GetHistoricalDailyCryptoPricesAsync(string symbol, string targetCurrency,
        DateTime from, DateTime to);

    /// <summary>
    ///     Retrieves intraday prices of the supplied cryptocurrency in the supplied range.
    /// </summary>
    /// <param name="symbol">Symbol of the cryptocurrency to retrieve prices for.</param>
    /// <param name="targetCurrency">Currency to retrieve crypto price in.</param>
    /// <param name="from">Range start</param>
    /// <param name="to">Range end</param>
    /// <param name="interval">Interval between individual price points</param>
    /// <returns>A task representing the asynchronous retrieval operation. Task result contains the retrieved data.</returns>
    public Task<IEnumerable<PricePoint>> GetIntradayCryptoPricesAsync(string symbol, string targetCurrency,
        DateTime from, DateTime to, IntradayInterval interval = IntradayInterval.OneHour);

    /// <summary>
    ///     Retrieves the latest price of the supplied cryptocurrency.
    /// </summary>
    /// <param name="symbol">Symbol to retrieve prices for.</param>
    /// <param name="targetCurrency">Currency to retrieve crypto price in.</param>
    /// <returns>A task representing the asynchronous retrieval operation. Task result contains the retrieved data.</returns>
    public Task<PricePoint> GetLatestCryptoPriceAsync(string symbol, string targetCurrency);

    /// <summary>
    ///     Retrieves the latest exchange rates of the supplied currency.
    /// </summary>
    /// <param name="baseCurrency">Base currency code</param>
    /// <returns>A task representing the asynchronous retrieval operation. Task result contains the retrieved data.</returns>
    public Task<ExchangeRates> GetLatestExchangeRatesAsync(string baseCurrency);

    /// <summary>
    ///     Retrieves historical daily exchange rates of the supplied currency in the supplied range.
    /// </summary>
    /// <param name="baseCurrency">Base currency code</param>
    /// <param name="from">Date range start</param>
    /// <param name="to">Date range end</param>
    /// <returns>A task representing the asynchronous retrieval operation. Task result contains the retrieved data.</returns>
    public Task<IEnumerable<ExchangeRates>> GetHistoricalDailyExchangeRatesAsync(string baseCurrency, DateTime from,
        DateTime to);

    /// <summary>
    ///     Retrieves historical stock splits of the supplied symbol in the supplied range.
    /// </summary>
    /// <param name="symbol">Symbol to retrieve splits for.</param>
    /// <param name="from">Date range start.</param>
    /// <param name="to">Date range end.</param>
    /// <returns>A task representing the asynchronous retrieval operation. Task result contains the retrieved data.</returns>
    public Task<IEnumerable<InstrumentSplitData>> GetInstrumentSplitsAsync(string symbol, DateTime from,
        DateTime to);
}