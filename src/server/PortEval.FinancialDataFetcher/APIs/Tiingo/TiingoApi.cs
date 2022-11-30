using PortEval.FinancialDataFetcher.APIs.Interfaces;
using PortEval.FinancialDataFetcher.APIs.Tiingo.Models;
using PortEval.FinancialDataFetcher.Models;
using PortEval.FinancialDataFetcher.Requests;
using PortEval.FinancialDataFetcher.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using PortEval.Domain.Models.Enums;

namespace PortEval.FinancialDataFetcher.APIs.Tiingo
{
    /// <summary>
    /// Tiingo API client supporting historical, intraday and latest instrument prices.
    /// </summary>
    internal class TiingoApi : IHistoricalDailyFinancialApi, IIntradayFinancialApi, ILatestPriceFinancialApi
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private readonly RateLimiter _rateLimiter;

        private const string TIINGO_DAILY_BASE_URL = "https://api.tiingo.com/tiingo/daily";
        private const string TIINGO_IEX_BASE_URL = "https://api.tiingo.com/iex";
        private const string TIINGO_CRYPTO_BASE_URL = "https://api.tiingo.com/tiingo/crypto";

        public TiingoApi(HttpClient httpClient, string apiKey, RateLimiter rateLimiter = null)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
            _rateLimiter = rateLimiter;
        }

        public async Task<Response<IEnumerable<PricePoint>>> Process(HistoricalDailyInstrumentPricesRequest request)
        {
            if (request.Type == InstrumentType.CryptoCurrency)
            {
                return await GetTiingoCryptoHistoricalPrices(request.Symbol, request.CurrencyCode, request.From, request.To);
            }

            return await GetTiingoEndOfDayPrices(request.Symbol, request.From, request.To);
        }

        public async Task<Response<IEnumerable<PricePoint>>> Process(IntradayPricesRequest request)
        {
            if (request.Type == InstrumentType.CryptoCurrency)
            {
                return await GetTiingoCryptoHistoricalPrices(request.Symbol, request.CurrencyCode, request.From, request.To, request.Interval);
            }

            return await GetTiingoIexHistoricalPrices(request.Symbol, request.From, request.To, request.Interval);
        }

        public async Task<Response<PricePoint>> Process(LatestInstrumentPriceRequest request)
        {
            if (request.Type == InstrumentType.CryptoCurrency)
            {
                return await GetTiingoCryptoTopOfBook(request.Symbol, request.CurrencyCode);
            }

            return await GetTiingoIexTopOfBook(request.Symbol);
        }

        private async Task<Response<IEnumerable<PricePoint>>> GetTiingoEndOfDayPrices(string symbol, DateTime from, DateTime to)
        {
            var startDate = from.ToString("yyyy-M-d");
            var endDate = to.ToString("yyyy-M-d");

            var urlBuilder = new QueryUrlBuilder($"{TIINGO_DAILY_BASE_URL}/{symbol}/prices");
            urlBuilder.AddQueryParam("token", _apiKey);
            urlBuilder.AddQueryParam("startDate", startDate);
            urlBuilder.AddQueryParam("endDate", endDate);

            var result = await _httpClient.FetchJson<IEnumerable<TiingoPriceResponseModel>>(urlBuilder.ToString(), _rateLimiter);

            return new Response<IEnumerable<PricePoint>>
            {
                StatusCode = result.StatusCode,
                ErrorMessage = result.ErrorMessage,
                Result = result.Result?
                    .Where(price => price.Time >= from && price.Time <= to)
                    .Select(price => new PricePoint
                    {
                        CurrencyCode = "USD",
                        Price = price.Price,
                        Symbol = symbol,
                        Time = price.Time.ToUniversalTime()
                    })
            };
        }

        private async Task<Response<IEnumerable<PricePoint>>> GetTiingoIexHistoricalPrices(string symbol, DateTime from,
            DateTime to, IntradayInterval? interval = null)
        {
            var startDate = from.Date.AddDays(1).ToString("yyyy-M-d");
            var endDate = to.Date.AddDays(1).ToString("yyyy-M-d");
            string resampleFreq = "1day";
            if (interval != null)
            {
                resampleFreq = interval == IntradayInterval.FiveMinutes ? "5min" : "60min";
            }

            var urlBuilder = new QueryUrlBuilder($"{TIINGO_IEX_BASE_URL}/{symbol}/prices");
            urlBuilder.AddQueryParam("token", _apiKey);
            urlBuilder.AddQueryParam("startDate", startDate);
            urlBuilder.AddQueryParam("endDate", endDate);
            urlBuilder.AddQueryParam("resampleFreq", resampleFreq);
            urlBuilder.AddQueryParam("token", _apiKey);

            var result = await _httpClient.FetchJson<IEnumerable<TiingoPriceResponseModel>>(urlBuilder.ToString(), _rateLimiter);

            return new Response<IEnumerable<PricePoint>>
            {
                StatusCode = result.StatusCode,
                ErrorMessage = result.ErrorMessage,
                Result = result.Result?
                    .Where(price => price.Time >= from && price.Time <= to)
                    .Select(price => new PricePoint
                    {
                        CurrencyCode = "USD",
                        Price = price.Price,
                        Symbol = symbol,
                        Time = price.Time.ToUniversalTime()
                    })
            };
        }

        private async Task<Response<PricePoint>> GetTiingoIexTopOfBook(string symbol)
        {
            var urlBuilder = new QueryUrlBuilder($"{TIINGO_IEX_BASE_URL}/{symbol}");
            urlBuilder.AddQueryParam("token", _apiKey);

            var result = await _httpClient.FetchJson<IEnumerable<TiingoIexTopPriceResponseModel>>(urlBuilder.ToString(), _rateLimiter);

            return new Response<PricePoint>
            {
                StatusCode = result.StatusCode,
                ErrorMessage = result.ErrorMessage,
                Result = result.Result != null && result.Result.Any()
                ? 
                new PricePoint
                {
                    CurrencyCode = "USD",
                    Price = result.Result.First().Price,
                    Symbol = symbol,
                    Time = DateTime.UtcNow
                }
                : null
            };
        }

        private async Task<Response<PricePoint>> GetTiingoCryptoTopOfBook(string ticker, string currency)
        {
            var urlBuilder = new QueryUrlBuilder($"{TIINGO_CRYPTO_BASE_URL}/top");
            urlBuilder.AddQueryParam("tickers", GetCryptoTicker(ticker, currency));
            urlBuilder.AddQueryParam("token", _apiKey);

            var result = await _httpClient.FetchJson<IEnumerable<TiingoCryptoTopPriceResponseModel>>(urlBuilder.ToString(), _rateLimiter);

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

        private async Task<Response<IEnumerable<PricePoint>>> GetTiingoCryptoHistoricalPrices(string ticker, string currency,
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

            List<TiingoPriceResponseModel> prices = new List<TiingoPriceResponseModel>();
            var lastPriceTime = from;
            var anySuccessful = false;
            var anyUnexpectedError = false;

            while (true)
            {
                var urlBuilder = new QueryUrlBuilder($"{TIINGO_CRYPTO_BASE_URL}/prices");
                urlBuilder.AddQueryParam("tickers", GetCryptoTicker(ticker, currency));
                urlBuilder.AddQueryParam("startDate", lastPriceTime.ToString("yyyy-M-d"));
                urlBuilder.AddQueryParam("endDate", endDate);
                urlBuilder.AddQueryParam("resampleFreq", resampleFreq);
                urlBuilder.AddQueryParam("token", _apiKey);

                var result = await _httpClient.FetchJson<IEnumerable<TiingoCryptoPriceResponseModel>>(urlBuilder.ToString(), _rateLimiter);

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
                    if (result.StatusCode == StatusCode.OtherError)
                    {
                        anyUnexpectedError = true;
                        break;
                    }
                }
            }

            var resultStatusCode = StatusCode.Ok;
            if (!anySuccessful)
            {
                resultStatusCode = !anyUnexpectedError ? StatusCode.OtherError : StatusCode.ConnectionError;
            }

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
}
