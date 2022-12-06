using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using PortEval.FinancialDataFetcher.APIs.Interfaces;
using PortEval.FinancialDataFetcher.APIs.RapidAPIMboum.Models;
using PortEval.FinancialDataFetcher.Models;
using PortEval.FinancialDataFetcher.Requests;
using PortEval.FinancialDataFetcher.Responses;

namespace PortEval.FinancialDataFetcher.APIs.RapidAPIMboum
{
    internal class MboumApi : IHistoricalDailyFinancialApi, IIntradayFinancialApi, ILatestPriceFinancialApi
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private readonly RateLimiter _rateLimiter;

        private const string HISTORICAL_DATA_BASE_URL = @"https://mboum-finance.p.rapidapi.com/hi/history";
        private const string LATEST_DATA_BASE_URL = @"https://mboum-finance.p.rapidapi.com/qu/quote";

        public MboumApi(HttpClient httpClient, string apiKey, RateLimiter rateLimiter = null)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
            _rateLimiter = rateLimiter;
        }

        public async Task<Response<IEnumerable<PricePoint>>> Process(HistoricalDailyInstrumentPricesRequest request)
        {
            return await GetHistoricalData(request.Symbol, "1d", request.CurrencyCode, request.From, request.To);
        }

        public async Task<Response<IEnumerable<PricePoint>>> Process(IntradayPricesRequest request)
        {
            var intervalStr = request.Interval == IntradayInterval.FiveMinutes ? "5m" : "1h";
            return await GetHistoricalData(request.Symbol, intervalStr, request.CurrencyCode, request.From, request.To);
        }

        public async Task<Response<PricePoint>> Process(LatestInstrumentPriceRequest request)
        {
            return await GetLatestPrice(request.Symbol);
        }

        private async Task<Response<IEnumerable<PricePoint>>> GetHistoricalData(string ticker, string interval, string currency, DateTime from, DateTime to)
        {
            var urlBuilder = new QueryUrlBuilder(HISTORICAL_DATA_BASE_URL);
            urlBuilder.AddQueryParam("symbol", ticker);
            urlBuilder.AddQueryParam("interval", interval);
            urlBuilder.AddQueryParam("diffandsplits", "false");

            var headers = new Dictionary<string, string>
            {
                { "X-RapidAPI-Key", _apiKey },
                { "X-RapidAPI-Host", new Uri(HISTORICAL_DATA_BASE_URL).Host }
            };

            var result = await _httpClient.GetJson<MboumHistoricalDataResponse>(urlBuilder.ToString(), _rateLimiter,
                headers);

            return new Response<IEnumerable<PricePoint>>
            {
                StatusCode = result.StatusCode,
                ErrorMessage = result.ErrorMessage,
                Result = result.Result.Items?
                    .Where(kv =>
                    {
                        var time = DateTimeOffset.FromUnixTimeSeconds(kv.Value.DateUtc).UtcDateTime;
                        return time >= from && time <= to;
                    })
                    .Select(kv => ConvertHistoricalDataItemToPricePoint(kv.Value, ticker, currency))
            };
        }

        private PricePoint ConvertHistoricalDataItemToPricePoint(MboumHistoricalDataItem item, string ticker, string currency)
        {
            return new PricePoint
            {
                CurrencyCode = currency,
                Price = item.Close,
                Symbol = ticker,
                Time = DateTimeOffset.FromUnixTimeSeconds(item.DateUtc).UtcDateTime
            };
        }

        private async Task<Response<PricePoint>> GetLatestPrice(string ticker)
        {
            var urlBuilder = new QueryUrlBuilder(LATEST_DATA_BASE_URL);
            urlBuilder.AddQueryParam("symbol", ticker);

            var headers = new Dictionary<string, string>
            {
                { "X-RapidAPI-Key", _apiKey },
                { "X-RapidAPI-Host", new Uri(LATEST_DATA_BASE_URL).Host }
            };

            var result =
                await _httpClient.GetJson<IEnumerable<MboumQuoteDataResponse>>(urlBuilder.ToString(), _rateLimiter, headers);

            return new Response<PricePoint>
            {
                StatusCode = result.StatusCode,
                ErrorMessage = result.ErrorMessage,
                Result = result.Result.Any() ? new PricePoint
                {
                    CurrencyCode = result.Result.First().Currency,
                    Price = result.Result.First().RegularMarketPrice,
                    Symbol = ticker,
                    Time = DateTime.UtcNow
                } : null
            };
        }
    }
}
