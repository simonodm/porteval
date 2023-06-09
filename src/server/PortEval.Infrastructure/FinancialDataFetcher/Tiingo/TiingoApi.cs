﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Models.FinancialDataFetcher;
using PortEval.DataFetcher;
using PortEval.DataFetcher.Models;
using PortEval.DataFetcher.Responses;
using PortEval.Infrastructure.FinancialDataFetcher.Extensions;
using PortEval.Infrastructure.FinancialDataFetcher.Requests;
using PortEval.Infrastructure.FinancialDataFetcher.Tiingo.Models;

namespace PortEval.Infrastructure.FinancialDataFetcher.Tiingo;

/// <summary>
///     Tiingo API client supporting historical, intraday and latest instrument prices.
/// </summary>
public class TiingoApi : DataSource
{
    private const string TiingoDailyBaseUrl = "https://api.tiingo.com/tiingo/daily";
    private const string TiingoIexBaseUrl = "https://api.tiingo.com/iex";
    private const string TiingoCryptoBaseUrl = "https://api.tiingo.com/tiingo/crypto";

    [RequestProcessor(typeof(HistoricalDailyInstrumentPricesRequest), typeof(IEnumerable<PricePoint>))]
    public Task<Response<IEnumerable<PricePoint>>> ProcessAsync(HistoricalDailyInstrumentPricesRequest request)
    {
        return GetTiingoEndOfDayPricesAsync(request.Symbol, request.From, request.To);
    }

    [RequestProcessor(typeof(LatestInstrumentPriceRequest), typeof(PricePoint))]
    public Task<Response<PricePoint>> ProcessAsync(LatestInstrumentPriceRequest request)
    {
        return GetTiingoIexTopOfBookAsync(request.Symbol);
    }

    [RequestProcessor(typeof(HistoricalDailyCryptoPricesRequest), typeof(IEnumerable<PricePoint>))]
    public Task<Response<IEnumerable<PricePoint>>> ProcessAsync(HistoricalDailyCryptoPricesRequest request)
    {
        return GetTiingoCryptoHistoricalPricesAsync(request.Symbol, request.CurrencyCode, request.From,
            request.To);
    }

    [RequestProcessor(typeof(IntradayCryptoPricesRequest), typeof(IEnumerable<PricePoint>))]
    public Task<Response<IEnumerable<PricePoint>>> ProcessAsync(IntradayCryptoPricesRequest request)
    {
        return GetTiingoCryptoHistoricalPricesAsync(request.Symbol, request.CurrencyCode, request.From,
            request.To, request.Interval);
    }

    [RequestProcessor(typeof(LatestCryptoPriceRequest), typeof(PricePoint))]
    public Task<Response<PricePoint>> ProcessAsync(LatestCryptoPriceRequest request)
    {
        return GetTiingoCryptoTopOfBookAsync(request.Symbol, request.CurrencyCode);
    }

    private async Task<Response<IEnumerable<PricePoint>>> GetTiingoEndOfDayPricesAsync(string symbol, DateTime from,
        DateTime to)
    {
        var startDate = from.ToString("yyyy-M-d");

        var urlBuilder = new QueryUrlBuilder($"{TiingoDailyBaseUrl}/{symbol}/prices");
        urlBuilder.AddQueryParam("token", Configuration.Credentials.Token);
        urlBuilder.AddQueryParam("startDate", startDate);

        var result =
            await HttpClient.GetJsonAsync<IEnumerable<TiingoPriceResponseModel>>(urlBuilder.ToString(),
                Configuration.RateLimiter);

        var sortedPrices = result.Result?.OrderByDescending(price => price.Time) ??
                           Enumerable.Empty<TiingoPriceResponseModel>();

        var responseResult = new List<PricePoint>();

        foreach (var price in sortedPrices)
        {
            if (price.Time < from)
            {
                break;
            }

            if (price.Time <= to)
            {
                responseResult.Add(new PricePoint
                {
                    CurrencyCode = "USD",
                    Price = price.Price,
                    Symbol = symbol,
                    Time = price.Time.ToUniversalTime()
                });
            }
        }

        return new Response<IEnumerable<PricePoint>>
        {
            StatusCode = result.StatusCode,
            ErrorMessage = result.ErrorMessage,
            Result = responseResult
        };
    }

    private async Task<Response<PricePoint>> GetTiingoIexTopOfBookAsync(string symbol)
    {
        var urlBuilder = new QueryUrlBuilder($"{TiingoIexBaseUrl}/{symbol}");
        urlBuilder.AddQueryParam("token", Configuration.Credentials.Token);

        var result =
            await HttpClient.GetJsonAsync<IEnumerable<TiingoIexTopPriceResponseModel>>(urlBuilder.ToString(),
                Configuration.RateLimiter);

        return new Response<PricePoint>
        {
            StatusCode = result.StatusCode,
            ErrorMessage = result.ErrorMessage,
            Result = result.Result != null && result.Result.Any()
                ? new PricePoint
                {
                    CurrencyCode = "USD",
                    Price = result.Result.First().Price,
                    Symbol = symbol,
                    Time = DateTime.UtcNow
                }
                : null
        };
    }

    private async Task<Response<PricePoint>> GetTiingoCryptoTopOfBookAsync(string ticker, string currency)
    {
        var urlBuilder = new QueryUrlBuilder($"{TiingoCryptoBaseUrl}/top");
        urlBuilder.AddQueryParam("tickers", GetCryptoTicker(ticker, currency));
        urlBuilder.AddQueryParam("token", Configuration.Credentials.Token);

        var result =
            await HttpClient.GetJsonAsync<IEnumerable<TiingoCryptoTopPriceResponseModel>>(urlBuilder.ToString(),
                Configuration.RateLimiter);

        return new Response<PricePoint>
        {
            StatusCode = result.StatusCode,
            ErrorMessage = result.ErrorMessage,
            Result = result.Result?.First()?.Data?.Any() != null
                ? new PricePoint
                {
                    CurrencyCode = currency,
                    Price = result.Result.First().Data.First().LastPrice,
                    Symbol = ticker,
                    Time = DateTime.UtcNow
                }
                : null
        };
    }

    private async Task<Response<IEnumerable<PricePoint>>> GetTiingoCryptoHistoricalPricesAsync(string ticker,
        string currency,
        DateTime from, DateTime to, IntradayInterval? interval = null)
    {
        // Tiingo crypto endpoint returns prices only in range startDate + (approximately) 5730 days, so multiple downloads need to be done in case
        // endDate - startDate is longer than that range.

        // The logic below downloads the prices iteratively, waiting for the previous download to finish to calculate the next time range to download. It keeps
        // attempting to download such ranges until one of the following happens:
        //  a) all the data until the end of the requested time range has been downloaded
        //  b) last 2 downloaded price ranges ended on the same date and time (in case price data ends before the requested time range end)
        //
        // After which it returns all the successfully retrieved prices.
        //
        // Specifically, if a download fails (returns an error code or empty data), there is a possibility c) that its range start + Tiingo date range limit
        // is still earlier than the first available price data. In that case the algorithm below adds 1000 days to the current start date and attempts again until
        // the end of the requested time range is reached (or a download succeeds).

        const int daysToAddOnFailure = 1000;

        var endDate = to.Date.AddDays(1).ToString("yyyy-M-d");
        var resampleFreq = "1day";
        if (interval != null)
        {
            resampleFreq = interval == IntradayInterval.FiveMinutes ? "5min" : "60min";
        }

        var prices = new List<TiingoCryptoPricePoint>();
        var lastPriceTime = from;
        var anySuccessful = false;
        var anyUnexpectedError = false;

        while (true)
        {
            var urlBuilder = new QueryUrlBuilder($"{TiingoCryptoBaseUrl}/prices");
            urlBuilder.AddQueryParam("tickers", GetCryptoTicker(ticker, currency));
            urlBuilder.AddQueryParam("startDate", lastPriceTime.ToString("yyyy-M-d"));
            urlBuilder.AddQueryParam("endDate", endDate);
            urlBuilder.AddQueryParam("resampleFreq", resampleFreq);
            urlBuilder.AddQueryParam("token", Configuration.Credentials.Token);

            var result =
                await HttpClient.GetJsonAsync<IEnumerable<TiingoCryptoPriceResponseModel>>(urlBuilder.ToString(),
                    Configuration.RateLimiter);

            if (result.StatusCode == StatusCode.Ok)
            {
                anySuccessful = true;
                if (result.Result?.FirstOrDefault()?.Data?.FirstOrDefault() != null)
                {
                    if (result.Result.First().Data.Last().Time == lastPriceTime) // b)
                    {
                        break;
                    }

                    prices.AddRange(result.Result.First().Data);
                    lastPriceTime = prices[^1].Time;
                    if (lastPriceTime >= to - ResampleFreqToTimeSpan(resampleFreq)) // a)
                    {
                        break;
                    }
                }
                else if (prices.Count == 0 && lastPriceTime < to - TimeSpan.FromDays(365)) // d)
                {
                    lastPriceTime += TimeSpan.FromDays(daysToAddOnFailure);
                }
                else
                {
                    break;
                }
            }
            else // c)
            {
                if (lastPriceTime < to - TimeSpan.FromDays(daysToAddOnFailure))
                {
                    lastPriceTime += TimeSpan.FromDays(daysToAddOnFailure);
                }
                else
                {
                    break;
                }

                if (result.StatusCode == StatusCode.OtherError)
                {
                    anyUnexpectedError = true;
                    break;
                }
            }
        }

        var resultStatusCode = anyUnexpectedError ? StatusCode.ConnectionError : StatusCode.Ok;

        return new Response<IEnumerable<PricePoint>>
        {
            StatusCode = resultStatusCode,
            ErrorMessage = anySuccessful ? "" : "An error has occurred",
            Result = prices
                .Where(price => price.Time >= from && price.Time <= to)
                .Select(price => new PricePoint
                {
                    CurrencyCode = currency,
                    Price = price.Price,
                    Symbol = ticker,
                    Time = price.Time.ToUniversalTime()
                })
        };
    }

    private TimeSpan ResampleFreqToTimeSpan(string resampleFreq)
    {
        switch (resampleFreq)
        {
            case "5min":
                return TimeSpan.FromMinutes(5);
            case "60min":
                return TimeSpan.FromMinutes(60);
            case "1day":
                return TimeSpan.FromDays(1);
            default:
                throw new Exception($"Unrecognized resample frequency provided: {resampleFreq}.");
        }
    }

    private string GetCryptoTicker(string symbol, string currencyCode)
    {
        if (symbol.Substring(symbol.Length - 3, 3) == currencyCode)
        {
            return symbol;
        }

        return symbol + currencyCode;
    }
}