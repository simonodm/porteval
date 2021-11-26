using PortEval.FinancialDataFetcher.APIs.Interfaces;
using PortEval.FinancialDataFetcher.APIs.Tiingo.Models;
using PortEval.FinancialDataFetcher.Models;
using PortEval.FinancialDataFetcher.Requests;
using PortEval.FinancialDataFetcher.Responses;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PortEval.FinancialDataFetcher.APIs.Tiingo
{
    /// <summary>
    /// Tiingo API client supporting historical, intraday and latest instrument prices.
    /// </summary>
    internal class TiingoApiClient : IHistoricalDailyFinancialApiClient, IIntradayFinancialApiClient, ILatestPriceFinancialApiClient
    {
        private const string _baseUrl = "https://api.tiingo.com/";
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private readonly RateLimiter _rateLimiter;

        public TiingoApiClient(HttpClient httpClient, string apiKey, RateLimiter rateLimiter = null)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
            _rateLimiter = rateLimiter;
        }

        public async Task<Response<IEnumerable<PricePoint>>> Process(HistoricalDailyInstrumentPricesRequest request)
        {
            var startTime = (request.From + TimeSpan.FromDays(1)).ToString("yyyy-M-d");
            var endTime = request.To.ToString("yyyy-M-d");
            var queryUrl =
                $"{_baseUrl}/tiingo/daily/{request.Symbol}/prices?startDate={startTime}&endDate={endTime}&columns=date,open&token={_apiKey}";

            var fetchResponse = await _httpClient.FetchJson(queryUrl, response =>
            {
                var prices = response.ToObject<List<TiingoPriceResponseModel>>();
                var result = new List<PricePoint>();
                if (prices != null)
                {
                    foreach (var price in prices)
                    {
                        if (price.Time > request.To) continue;
                        if (price.Time < request.From) break;

                        result.Add(new PricePoint
                        {
                            Time = price.Time,
                            Price = price.Price,
                            Symbol = request.Symbol,
                            CurrencyCode = "USD"
                        });
                    }

                    return new Response<IEnumerable<PricePoint>>
                    {
                        StatusCode = StatusCode.Ok,
                        Result = result
                    };
                }

                return new Response<IEnumerable<PricePoint>>
                {
                    StatusCode = StatusCode.OtherError,
                    ErrorMessage = "Invalid data received."
                };
            }, _rateLimiter);

            return fetchResponse;
        }

        public async Task<Response<IEnumerable<PricePoint>>> Process(IntradayPricesRequest request)
        {
            var startDate = request.From.ToString("yyyy-M-d");
            var endDate = request.To.ToString("yyyy-M-d");
            var interval = request.Interval == IntradayInterval.FiveMinutes ? "5min" : "60min";
            var queryUrl =
                $"{_baseUrl}/iex/{request.Symbol}/prices?startDate={startDate}&endDate={endDate}&resampleFreq={interval}&token={_apiKey}";

            var fetchResponse = await _httpClient.FetchJson(queryUrl, response =>
            {
                var responseModel = response.ToObject<List<TiingoPriceResponseModel>>();
                var result = new List<PricePoint>();
                if (responseModel != null)
                {
                    foreach (var price in responseModel)
                    {
                        if (price.Time < request.From) continue;
                        if (price.Time > request.To) break;

                        result.Add(new PricePoint
                        {
                            Symbol = request.Symbol,
                            Price = price.Price,
                            Time = price.Time,
                            CurrencyCode = "USD"
                        });
                    }
                    return new Response<IEnumerable<PricePoint>>
                    {
                        StatusCode = StatusCode.Ok,
                        Result = result
                    };
                }

                return new Response<IEnumerable<PricePoint>>
                {
                    StatusCode = StatusCode.OtherError,
                    ErrorMessage = "Invalid data received."
                };
            }, _rateLimiter);

            return fetchResponse;
        }

        public async Task<Response<PricePoint>> Process(LatestInstrumentPriceRequest request)
        {
            var queryUrl = $"{_baseUrl}/iex/{request.Symbol}?token={_apiKey}";

            var fetchResponse = await _httpClient.FetchJson(queryUrl, response =>
            {
                var responseModel = response.ToObject<List<TiingoLatestPriceResponseModel>>();
                if (responseModel != null && responseModel.Count > 0)
                {
                    var pricePoint = new PricePoint
                    {
                        Symbol = request.Symbol,
                        Price = responseModel[0].Price,
                        Time = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now),
                        CurrencyCode = "USD"
                    };
                    return new Response<PricePoint>
                    {
                        StatusCode = StatusCode.Ok,
                        Result = pricePoint
                    };
                }

                return new Response<PricePoint>
                {
                    StatusCode = StatusCode.OtherError,
                    ErrorMessage = "Invalid data received."
                };
            }, _rateLimiter);

            return fetchResponse;
        }
    }
}
