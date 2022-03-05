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
using Newtonsoft.Json.Linq;
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

            var urlBuilder = new QueryUrlBuilder($"{TIINGO_DAILY_BASE_URL}/{symbol}");
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
                        Time = price.Time
                    })
            };
        }

        private async Task<Response<IEnumerable<PricePoint>>> GetTiingoIexHistoricalPrices(string symbol, DateTime from,
            DateTime to, IntradayInterval? interval = null)
        {
            var startDate = from.ToString("yyyy-M-d");
            var endDate = to.ToString("yyyy-M-d");
            string resampleFreq = null;
            if (interval != null)
            {
                resampleFreq = interval == IntradayInterval.FiveMinutes ? "5min" : "60min";
            }

            var urlBuilder = new QueryUrlBuilder($"{TIINGO_IEX_BASE_URL}/{symbol}/prices");
            urlBuilder.AddQueryParam("token", _apiKey);
            urlBuilder.AddQueryParam("startDate", startDate);
            urlBuilder.AddQueryParam("endDate", endDate);
            urlBuilder.AddQueryParam("resampleFreq", resampleFreq);

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
                        Time = price.Time
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
                    Time = DateTime.Now
                }
                : null
            };
        }

        private async Task<Response<PricePoint>> GetTiingoCryptoTopOfBook(string ticker, string currency)
        {
            var urlBuilder = new QueryUrlBuilder($"{TIINGO_CRYPTO_BASE_URL}/top");
            urlBuilder.AddQueryParam("tickers", ticker + currency);

            var result = await _httpClient.FetchJson<TiingoCryptoTopPriceResponseModel>(urlBuilder.ToString(), _rateLimiter);

            return new Response<PricePoint>
            {
                StatusCode = result.StatusCode,
                ErrorMessage = result.ErrorMessage,
                Result = result.Result != null 
                    ? new PricePoint
                    {
                        CurrencyCode = "USD",
                        Price = result.Result.Data.LastPrice,
                        Symbol = ticker,
                        Time = DateTime.Now
                    }
                    : null
            };
        }

        private async Task<Response<IEnumerable<PricePoint>>> GetTiingoCryptoHistoricalPrices(string ticker, string currency,
            DateTime from, DateTime to, IntradayInterval? interval = null)
        {
            var startDate = from.ToString("yyyy-M-d");
            var endDate = to.ToString("yyyy-M-d");
            string resampleFreq = "1day";
            if (interval != null)
            {
                resampleFreq = interval == IntradayInterval.FiveMinutes ? "5min" : "60min";
            }

            var urlBuilder = new QueryUrlBuilder($"{TIINGO_CRYPTO_BASE_URL}/prices");
            urlBuilder.AddQueryParam("tickers", ticker + currency);
            urlBuilder.AddQueryParam("startDate", startDate);
            urlBuilder.AddQueryParam("endDate", endDate);
            urlBuilder.AddQueryParam("resampleFreq", resampleFreq);

            var result = await _httpClient.FetchJson<TiingoCryptoPriceResponseModel>(urlBuilder.ToString(), _rateLimiter);

            return new Response<IEnumerable<PricePoint>>
            {
                StatusCode = result.StatusCode,
                ErrorMessage = result.ErrorMessage,
                Result = result.Result?.Data
                    .Where(price => price.Time >= from && price.Time <= to)
                    .Select(price => new PricePoint
                    {
                        CurrencyCode = "USD",
                        Price = price.Price,
                        Symbol = ticker,
                        Time = price.Time
                    })
            };
        }
    }
}
