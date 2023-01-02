using PortEval.FinancialDataFetcher.APIs.RapidAPIMboum.Models;
using PortEval.FinancialDataFetcher.Interfaces.APIs;
using PortEval.FinancialDataFetcher.Models;
using PortEval.FinancialDataFetcher.Requests;
using PortEval.FinancialDataFetcher.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PortEval.FinancialDataFetcher.APIs.RapidAPIMboum
{
    internal class MboumApi : IHistoricalDailyInstrumentPricesFinancialApi, IIntradayFinancialApi, ILatestInstrumentPriceFinancialApi,
        IInstrumentSplitFinancialApi
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
            var response = await GetHistoricalData(request.Symbol, "1d");

            return new Response<IEnumerable<PricePoint>>
            {
                StatusCode = response.StatusCode,
                ErrorMessage = response.ErrorMessage,
                Result = response.Result.Items?
                    .Where(kv =>
                    {
                        var time = DateTimeOffset.FromUnixTimeSeconds(kv.Value.DateUtc).UtcDateTime;
                        return time >= request.From && time <= request.To;
                    })
                    .Select(kv => ConvertHistoricalDataItemToPricePoint(kv.Value, request.Symbol, request.CurrencyCode))
            };
        }

        public async Task<Response<IEnumerable<PricePoint>>> Process(IntradayInstrumentPricesRequest request)
        {
            var intervalStr = request.Interval == IntradayInterval.FiveMinutes ? "5m" : "1h";
            var response = await GetHistoricalData(request.Symbol, intervalStr);

            return new Response<IEnumerable<PricePoint>>
            {
                StatusCode = response.StatusCode,
                ErrorMessage = response.ErrorMessage,
                Result = response.Result.Items?
                    .Where(kv =>
                    {
                        var time = DateTimeOffset.FromUnixTimeSeconds(kv.Value.DateUtc).UtcDateTime;
                        return time >= request.From && time <= request.To;
                    })
                    .Select(kv => ConvertHistoricalDataItemToPricePoint(kv.Value, request.Symbol, request.CurrencyCode))
            };
        }

        public async Task<Response<PricePoint>> Process(LatestInstrumentPriceRequest request)
        {
            var response = await GetQuoteData(request.Symbol);
            return new Response<PricePoint>
            {
                StatusCode = response.StatusCode,
                ErrorMessage = response.ErrorMessage,
                Result = response.Result.Any() ? new PricePoint
                {
                    CurrencyCode = response.Result.First().Currency,
                    Price = response.Result.First().RegularMarketPrice,
                    Symbol = request.Symbol,
                    Time = DateTime.UtcNow
                } : null
            };
        }

        public async Task<Response<IEnumerable<InstrumentSplitData>>> Process(InstrumentSplitsRequest request)
        {
            var response = await GetHistoricalData(request.Symbol, "3mo", downloadSplits: true);
            return new Response<IEnumerable<InstrumentSplitData>>
            {
                StatusCode = response.StatusCode,
                ErrorMessage = response.ErrorMessage,
                Result = response.Result?.Events?.Splits?
                    .Where(kv =>
                    {
                        var time = DateTimeOffset.FromUnixTimeSeconds(kv.Value.Date);
                        return time.DateTime > request.From && time.DateTime <= request.To;
                    })
                    .Select(kv => ConvertHistoricalSplitDataItemToInstrumentSplitData(kv.Value))
            };
        }

        private async Task<Response<MboumHistoricalDataResponse>> GetHistoricalData(string ticker, string interval, bool downloadSplits = false)
        {
            var urlBuilder = new QueryUrlBuilder(HISTORICAL_DATA_BASE_URL);
            urlBuilder.AddQueryParam("symbol", ticker);
            urlBuilder.AddQueryParam("interval", interval);
            urlBuilder.AddQueryParam("diffandsplits", downloadSplits.ToString().ToLower());

            var headers = new Dictionary<string, string>
            {
                { "X-RapidAPI-Key", _apiKey },
                { "X-RapidAPI-Host", new Uri(HISTORICAL_DATA_BASE_URL).Host }
            };

            var result = await _httpClient.GetJson<MboumHistoricalDataResponse>(urlBuilder.ToString(), _rateLimiter,
                headers);

            return result;
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

        private InstrumentSplitData ConvertHistoricalSplitDataItemToInstrumentSplitData(MboumSplit split)
        {
            if (split == null)
            {
                return null;
            }

            return new InstrumentSplitData
            {
                Time = DateTimeOffset.FromUnixTimeSeconds(split.Date).UtcDateTime,
                Numerator = split.Numerator,
                Denominator = split.Denominator
            };
        }

        private async Task<Response<IEnumerable<MboumQuoteDataResponse>>> GetQuoteData(string ticker)
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

            return result;
        }
    }
}
