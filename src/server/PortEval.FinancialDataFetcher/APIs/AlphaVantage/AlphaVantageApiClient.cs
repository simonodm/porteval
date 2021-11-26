using PortEval.FinancialDataFetcher.APIs.AlphaVantage.Models;
using PortEval.FinancialDataFetcher.APIs.Interfaces;
using PortEval.FinancialDataFetcher.Models;
using PortEval.FinancialDataFetcher.Requests;
using PortEval.FinancialDataFetcher.Responses;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PortEval.FinancialDataFetcher.APIs.AlphaVantage
{
    /// <summary>
    /// AlphaVantage API client supporting historical daily, intraday and latest instrument prices.
    /// </summary>
    internal class AlphaVantageApiClient : IHistoricalDailyFinancialApiClient, IIntradayFinancialApiClient, ILatestPriceFinancialApiClient
    {
        private const string _baseUrl = "https://www.alphavantage.co/";
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly RateLimiter _rateLimiter;

        public AlphaVantageApiClient(HttpClient httpClient, string apiKey, RateLimiter rateLimiter = null)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
            _rateLimiter = rateLimiter;
        }

        public async Task<Response<IEnumerable<PricePoint>>> Process(HistoricalDailyInstrumentPricesRequest request)
        {
            ValidateRange(request.From, request.To);

            var queryUrl = $"{_baseUrl}/query?function=TIME_SERIES_DAILY&symbol={request.Symbol}&outputsize=full&apikey={_apiKey}";

            return await _httpClient.FetchJson(queryUrl,
                response => ProcessPrices(request, response?.ToObject<TimeSeriesDailyResponseModel>()?.Prices), _rateLimiter);
        }

        public async Task<Response<IEnumerable<PricePoint>>> Process(IntradayPricesRequest request)
        {
            ValidateRange(request.From, request.To);

            var intervalString = request.Interval == IntradayInterval.FiveMinutes ? "5min" : "60min";
            var queryUrl =
                $"{_baseUrl}/query?function=TIME_SERIES_INTRADAY&symbol={request.Symbol}&interval={intervalString}&outputsize=full&apikey={_apiKey}";

            return await _httpClient.FetchJson(queryUrl,
                response => ProcessPrices(request, response?.ToObject<TimeSeriesIntradayResponseModel>()?.Prices), _rateLimiter);
        }

        public async Task<Response<PricePoint>> Process(LatestInstrumentPriceRequest request)
        {
            var queryUrl = $"{_baseUrl}/query?function=GLOBAL_QUOTE&symbol={request.Symbol}&apikey={_apiKey}";

            return await _httpClient.FetchJson(queryUrl, response =>
            {
                var currentPriceData = response.ToObject<GlobalQuoteResponseModel>();
                if (currentPriceData != null)
                {
                    var pricePoint = new PricePoint
                    {
                        Symbol = request.Symbol,
                        Price = currentPriceData.PriceData.Price,
                        Time = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)
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
        }

        private void ValidateRange(DateTime from, DateTime to)
        {
            if (to < from)
            {
                throw new ArgumentException($"Argument {nameof(from)} cannot be later than argument {nameof(to)}");
            }
        }

        private Response<IEnumerable<PricePoint>> ProcessPrices(TimeRangeRequest request, Dictionary<string, TimeSeriesPriceDataModel> prices)
        {
            if (prices == null)
            {
                return new Response<IEnumerable<PricePoint>>
                {
                    StatusCode = StatusCode.OtherError,
                    ErrorMessage = "Invalid data received"
                };
            }

            var result = new List<PricePoint>();
            foreach (var (timeKey, priceData) in prices)
            {
                var time = DateTime.Parse(timeKey);

                if (time < request.From)
                {
                    break;
                }
                if (time > request.To)
                {
                    continue;
                }

                var price = priceData.Price;
                var pricePoint = new PricePoint
                {
                    Symbol = request.Symbol,
                    Time = TimeZoneInfo.ConvertTimeToUtc(time),
                    Price = price
                };

                result.Add(pricePoint);
            }

            return new Response<IEnumerable<PricePoint>>
            {
                StatusCode = StatusCode.Ok,
                Result = result
            };
        }
    }
}
