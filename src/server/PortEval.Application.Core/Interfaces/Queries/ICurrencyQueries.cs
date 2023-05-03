using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;

namespace PortEval.Application.Core.Interfaces.Queries;

/// <summary>
///     Implements queries for currency data stored in the application's persistent storage.
/// </summary>
public interface ICurrencyQueries
{
    /// <summary>
    ///     Retrieves all available currencies.
    /// </summary>
    /// <returns>
    ///     A task representing the asynchronous query.
    ///     Task result contains an <see cref="IEnumerable{T}" /> containing available currencies.
    /// </returns>
    Task<IEnumerable<CurrencyDto>> GetAllCurrenciesAsync();

    /// <summary>
    ///     Retrieves a currency by its 3-letter code.
    /// </summary>
    /// <param name="currencyCode">Code of the currency to retrieve.</param>
    /// <returns>
    ///     A task representing the asynchronous query.
    ///     Task result contains the retrieved currency if it exists, <c>null</c> otherwise.
    /// </returns>
    Task<CurrencyDto> GetCurrencyAsync(string currencyCode);

    /// <summary>
    ///     Retrieves the default currency of the application.
    /// </summary>
    /// <returns>
    ///     A task representing the asynchronous query.
    ///     Task result contains the application's default currency.
    /// </returns>
    Task<CurrencyDto> GetDefaultCurrencyAsync();

    /// <summary>
    ///     Retrieves the exchange rate between two currencies at the specified time.
    /// </summary>
    /// <param name="currencyFrom">Base currency.</param>
    /// <param name="currencyTo">Target currency.</param>
    /// <param name="time">Time of the exchange rate.</param>
    /// <returns>
    ///     A task representing the asynchronous query.
    ///     Task result contains the last available exchange rate with time earlier than <paramref name="time"/> if it exists,
    ///     <c>null</c> otherwise.
    /// </returns>
    Task<CurrencyExchangeRateDto> GetCurrencyExchangeRateAsync(string currencyFrom, string currencyTo,
        DateTime time);

    /// <summary>
    ///     Retrieves a collection of direct exchange rates from the specified currency at the specified time.
    /// </summary>
    /// <param name="currencyCode">Base currency.</param>
    /// <param name="time">Time of the exchange rates.</param>
    /// <returns>
    ///     A task representing the asynchronous query.
    ///     Task result contains an <see cref="IEnumerable{T}" /> containing exchange rates from the specified currency at the
    ///     specified time.
    /// </returns>
    Task<IEnumerable<CurrencyExchangeRateDto>> GetDirectExchangeRatesAsync(string currencyCode,
        DateTime time);

    /// <summary>
    ///     Retrieves a collection of direct exchange rates between specified currencies in the specified date range.
    /// </summary>
    /// <param name="baseCurrencyCode">Base currency.</param>
    /// <param name="targetCurrencyCode">Target currency.</param>
    /// <param name="dateRange">Date range in which to retrieve exchange rates.</param>
    /// <returns>
    ///     A task representing the asynchronous query.
    ///     Task result contains an <see cref="IEnumerable{T}" /> containing exchange rates between the specified currencies in
    ///     the specified date range.
    /// </returns>
    Task<IEnumerable<CurrencyExchangeRateDto>> GetDirectExchangeRatesAsync(string baseCurrencyCode,
        string targetCurrencyCode, DateRangeParams dateRange);

    /// <summary>
    ///     Retrieves a collection of inverse exchange rates between specified currencies in the specified date range.
    /// </summary>
    /// <param name="baseCurrencyCode">Base currency.</param>
    /// <param name="targetCurrencyCode">Target currency.</param>
    /// <param name="dateRange">Date range in which to retrieve exchange rates.</param>
    /// <returns>
    ///     A task representing the asynchronous query.
    ///     Task result contains an <see cref="IEnumerable{T}" /> containing inverse exchange rates between the specified
    ///     currencies in the specified date range.
    /// </returns>
    /// <remarks>
    ///     An inverse exchange rate is defined as follows: if exchange rate between currencies A and B is X, then inverse
    ///     exchange rate would be 1/X.
    /// </remarks>
    Task<IEnumerable<CurrencyExchangeRateDto>> GetInversedExchangeRatesAsync(string baseCurrencyCode,
        string targetCurrencyCode, DateRangeParams dateRange);
}