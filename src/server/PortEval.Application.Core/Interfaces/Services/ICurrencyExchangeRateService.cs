using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;

namespace PortEval.Application.Core.Interfaces.Services;

/// <summary>
///     A service providing functionality related to currency exchange rates.
/// </summary>
public interface ICurrencyExchangeRateService
{
    /// <summary>
    ///     Retrieves the exchange rate between the specified currencies at the specified time.
    /// </summary>
    /// <param name="currencyFrom">Base currency.</param>
    /// <param name="currencyTo">Target currency.</param>
    /// <param name="time">Time of the exchange rate.</param>
    /// <returns>
    ///     A task representing the asynchronous retrieval operation.
    ///     Task result contains an <see cref="OperationResponse{T}" /> wrapper over the retrieved exchange rate.
    /// </returns>
    Task<OperationResponse<CurrencyExchangeRateDto>> GetExchangeRateAtAsync(string currencyFrom, string currencyTo,
        DateTime
            time);

    /// <summary>
    ///     Retrieves the exchange rates from the specified base currency at the specified time.
    /// </summary>
    /// <param name="currencyCode">Base currency.</param>
    /// <param name="time">Time of the exchange rates.</param>
    /// <returns>
    ///     A task representing the asynchronous retrieval operation.
    ///     Task result contains an <see cref="OperationResponse{T}" /> wrapper over the retrieved exchange rates.
    /// </returns>
    Task<OperationResponse<IEnumerable<CurrencyExchangeRateDto>>> GetExchangeRatesAsync(string currencyCode,
        DateTime time);

    /// <summary>
    ///     Retrieves the exchange rate between the specified currencies in the specified date range.
    /// </summary>
    /// <param name="currencyFrom">Base currency.</param>
    /// <param name="currencyTo">Target currency.</param>
    /// <param name="dateRange">Date range to retrieve exchange rates in.</param>
    /// <returns>
    ///     A task representing the asynchronous retrieval operation.
    ///     Task result contains an <see cref="OperationResponse{T}" /> wrapper over the retrieved exchange rates.
    /// </returns>
    Task<OperationResponse<IEnumerable<CurrencyExchangeRateDto>>> GetExchangeRatesAsync(string currencyFrom,
        string currencyTo, DateRangeParams dateRange);
}