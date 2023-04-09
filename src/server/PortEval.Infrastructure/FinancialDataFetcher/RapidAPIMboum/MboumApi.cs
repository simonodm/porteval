using PortEval.DataFetcher;
using PortEval.DataFetcher.Responses;
using PortEval.Infrastructure.FinancialDataFetcher.RapidAPIMboum.Models;
using PortEval.Infrastructure.FinancialDataFetcher.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Models.FinancialDataFetcher;
using PortEval.Infrastructure.FinancialDataFetcher.Extensions;

namespace PortEval.Infrastructure.FinancialDataFetcher.RapidAPIMboum
{
    public class MboumApi : DataSource
    {
        private const string HISTORICAL_DATA_BASE_URL = @"https://mboum-finance.p.rapidapi.com/hi/history";
        private const string LATEST_DATA_BASE_URL = @"https://mboum-finance.p.rapidapi.com/qu/quote";

        [RequestProcessor(typeof(HistoricalDailyInstrumentPricesRequest), typeof(IEnumerable<PricePoint>))]
        public async Task<Response<IEnumerable<PricePoint>>> ProcessAsync(HistoricalDailyInstrumentPricesRequest request)
        {
            var response = await GetHistoricalDataAsync(request.Symbol, "1d");

            return new Response<IEnumerable<PricePoint>>
            {
                StatusCode = response.StatusCode,
                ErrorMessage = response.ErrorMessage,
                Result = response.Result?.Items?
                    .Where(kv =>
                    {
                        var time = DateTimeOffset.FromUnixTimeSeconds(kv.Value.DateUtc).UtcDateTime;
                        return time >= request.From && time <= request.To;
                    })
                    .Select(kv => ConvertHistoricalDataItemToPricePoint(kv.Value, request.Symbol, request.CurrencyCode))
            };
        }

        [RequestProcessor(typeof(IntradayInstrumentPricesRequest), typeof(IEnumerable<PricePoint>))]
        public async Task<Response<IEnumerable<PricePoint>>> ProcessAsync(IntradayInstrumentPricesRequest request)
        {
            var intervalStr = request.Interval == IntradayInterval.FiveMinutes ? "5m" : "1h";
            var response = await GetHistoricalDataAsync(request.Symbol, intervalStr);

            return new Response<IEnumerable<PricePoint>>
            {
                StatusCode = response.StatusCode,
                ErrorMessage = response.ErrorMessage,
                Result = response.Result?.Items?
                    .Where(kv =>
                    {
                        var time = DateTimeOffset.FromUnixTimeSeconds(kv.Value.DateUtc).UtcDateTime;
                        return time >= request.From && time <= request.To;
                    })
                    .Select(kv => ConvertHistoricalDataItemToPricePoint(kv.Value, request.Symbol, request.CurrencyCode))
            };
        }

        [RequestProcessor(typeof(LatestInstrumentPriceRequest), typeof(PricePoint))]
        public async Task<Response<PricePoint>> ProcessAsync(LatestInstrumentPriceRequest request)
        {
            var response = await GetQuoteData(request.Symbol);
            return new Response<PricePoint>
            {
                StatusCode = response.StatusCode,
                ErrorMessage = response.ErrorMessage,
                Result = response.Result != null && response.Result.Any() ? new PricePoint
                {
                    CurrencyCode = response.Result.First().Currency,
                    Price = response.Result.First().RegularMarketPrice,
                    Symbol = request.Symbol,
                    Time = DateTime.UtcNow
                } : null
            };
        }

        [RequestProcessor(typeof(InstrumentSplitsRequest), typeof(IEnumerable<InstrumentSplitData>))]
        public async Task<Response<IEnumerable<InstrumentSplitData>>> ProcessAsync(InstrumentSplitsRequest request)
        {
            var response = await GetHistoricalDataAsync(request.Symbol, "3mo", downloadSplits: true);
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

        private async Task<Response<MboumHistoricalDataResponse>> GetHistoricalDataAsync(string ticker, string interval, bool downloadSplits = false)
        {
            var urlBuilder = new QueryUrlBuilder(HISTORICAL_DATA_BASE_URL);
            urlBuilder.AddQueryParam("symbol", ticker);
            urlBuilder.AddQueryParam("interval", interval);
            urlBuilder.AddQueryParam("diffandsplits", downloadSplits.ToString().ToLower());

            var headers = new Dictionary<string, string>
            {
                { "X-RapidAPI-Key", Configuration.Credentials.Token },
                { "X-RapidAPI-Host", new Uri(HISTORICAL_DATA_BASE_URL).Host }
            };

            var result = await HttpClient.GetJsonAsync<MboumHistoricalDataResponse>(urlBuilder.ToString(), Configuration.RateLimiter, headers);

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
                { "X-RapidAPI-Key", Configuration.Credentials.Token },
                { "X-RapidAPI-Host", new Uri(LATEST_DATA_BASE_URL).Host }
            };

            var result =
                await HttpClient.GetJsonAsync<IEnumerable<MboumQuoteDataResponse>>(urlBuilder.ToString(), Configuration.RateLimiter, headers);

            return result;
        }
    }
}
