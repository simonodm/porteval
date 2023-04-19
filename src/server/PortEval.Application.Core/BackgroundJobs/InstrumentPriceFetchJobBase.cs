using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces;
using PortEval.Application.Models.FinancialDataFetcher;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Core.BackgroundJobs;

/// <summary>
///     An abstract class providing helpers for instrument price retrieval jobs.
/// </summary>
public abstract class InstrumentPriceFetchJobBase
{
    private readonly IFinancialDataFetcher _priceFetcher;

    /// <summary>
    ///     Initializes the class with an <see cref="IFinancialDataFetcher" /> instance.
    /// </summary>
    /// <param name="priceFetcher">Price fetcher to use for price retrieval.</param>
    protected InstrumentPriceFetchJobBase(IFinancialDataFetcher priceFetcher)
    {
        _priceFetcher = priceFetcher;
    }

    /// <summary>
    ///     Retrieves historical daily prices of an instrument.
    /// </summary>
    /// <param name="instrument">Instrument to retrieve prices of.</param>
    /// <param name="from">Date to retrieve prices from.</param>
    /// <param name="to">Date to retrieve prices to.</param>
    /// <returns>
    ///     A task representing the asynchronous retrieval operation.
    ///     Task result contains an <see cref="IEnumerable{T}" /> containing retrieved instrument prices.
    /// </returns>
    protected async Task<IEnumerable<PricePoint>> FetchHistoricalDailyPrices(Instrument instrument, DateTime from,
        DateTime to)
    {
        IEnumerable<PricePoint> response;

        if (instrument.Type == InstrumentType.CryptoCurrency)
            response = await _priceFetcher.GetHistoricalDailyCryptoPricesAsync(instrument.Symbol,
                instrument.CurrencyCode, from, to);
        else
            response = await _priceFetcher.GetHistoricalDailyPricesAsync(instrument.Symbol, instrument.CurrencyCode,
                from, to);

        return response ?? Enumerable.Empty<PricePoint>();
    }

    /// <summary>
    ///     Retrieves intraday prices of an instrument.
    /// </summary>
    /// <param name="instrument">Instrument to retrieve prices of.</param>
    /// <param name="from">Date and time to retrieve prices from.</param>
    /// <param name="to">Date and time to retrieve prices to.</param>
    /// <param name="interval">Desired price interval.</param>
    /// <returns>
    ///     A task representing the asynchronous retrieval operation.
    ///     Task result contains an <see cref="IEnumerable{T}" /> containing retrieved instrument prices.
    /// </returns>
    protected async Task<IEnumerable<PricePoint>> FetchIntradayPrices(Instrument instrument, DateTime from,
        DateTime to, IntradayInterval interval)
    {
        IEnumerable<PricePoint> response;

        if (instrument.Type == InstrumentType.CryptoCurrency)
            response = await _priceFetcher.GetIntradayCryptoPricesAsync(instrument.Symbol, instrument.CurrencyCode,
                from, to, interval);
        else
            response = await _priceFetcher.GetIntradayPricesAsync(instrument.Symbol, instrument.CurrencyCode, from, to,
                interval);

        return response ?? Enumerable.Empty<PricePoint>();
    }

    /// <summary>
    ///     Retrieves the current price of an instrument.
    /// </summary>
    /// <param name="instrument">Instrument to retrieve current price of.</param>
    /// <returns>
    ///     A task representing the asynchronous retrieval operation.
    ///     Task result contains the current price of the instrument.
    /// </returns>
    protected async Task<PricePoint> FetchLatestPrice(Instrument instrument)
    {
        PricePoint response;

        if (instrument.Type == InstrumentType.CryptoCurrency)
            response = await _priceFetcher.GetLatestCryptoPriceAsync(instrument.Symbol, instrument.CurrencyCode);
        else
            response = await _priceFetcher.GetLatestInstrumentPriceAsync(instrument.Symbol, instrument.CurrencyCode);

        return response;
    }
}