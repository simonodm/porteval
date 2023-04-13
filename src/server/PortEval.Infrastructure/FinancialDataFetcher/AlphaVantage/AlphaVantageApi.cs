using PortEval.Application.Models.FinancialDataFetcher;
using PortEval.DataFetcher;
using PortEval.DataFetcher.Models;
using PortEval.DataFetcher.Responses;
using PortEval.Infrastructure.FinancialDataFetcher.AlphaVantage.Models;
using PortEval.Infrastructure.FinancialDataFetcher.Extensions;
using PortEval.Infrastructure.FinancialDataFetcher.Requests;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PortEval.Infrastructure.FinancialDataFetcher.AlphaVantage
{
    /// <summary>
    /// AlphaVantage API client supporting historical daily, intraday and latest instrument prices.
    /// </summary>
    public class AlphaVantageApi: DataSource
    {
        private const string _baseUrl = "https://www.alphavantage.co";

        [RequestProcessor(typeof(HistoricalDailyInstrumentPricesRequest), typeof(IEnumerable<PricePoint>))]
        public async Task<Response<IEnumerable<PricePoint>>> Process(HistoricalDailyInstrumentPricesRequest request)
        {
            ValidateRange(request.From, request.To);

            var queryUrl = $"{_baseUrl}/query?function=TIME_SERIES_DAILY_ADJUSTED&symbol={request.Symbol}&outputsize=full&apikey={Configuration.Credentials.Token}";

            var response = await HttpClient.GetJsonAsync<TimeSeriesDailyResponseModel>(queryUrl, Configuration.RateLimiter);
            if (response.Result == null)
            {
                return new Response<IEnumerable<PricePoint>>
                {
                    StatusCode = StatusCode.OtherError,
                    ErrorMessage = "Invalid data received."
                };
            }

            var result = new List<PricePoint>();
            var currentSplitFactor = 1m;
            foreach (var (timeKey, priceData) in response.Result.Prices)
            {
                var time = DateTime.Parse(timeKey);
                time = DateTime.SpecifyKind(time, DateTimeKind.Utc);

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
                    Time = time,
                    Price = price / currentSplitFactor,
                    CurrencyCode = request.CurrencyCode
                };

                result.Add(pricePoint);

                currentSplitFactor *= priceData.SplitCoefficient;
            }

            return new Response<IEnumerable<PricePoint>>
            {
                StatusCode = StatusCode.Ok,
                Result = result
            };
        }

        [RequestProcessor(typeof(LatestInstrumentPriceRequest), typeof(PricePoint))]
        public async Task<Response<PricePoint>> Process(LatestInstrumentPriceRequest request)
        {
            var queryUrl = $"{_baseUrl}/query?function=GLOBAL_QUOTE&symbol={request.Symbol}&apikey={Configuration.Credentials.Token}";

            var response = await HttpClient.GetJsonAsync<GlobalQuoteResponseModel>(queryUrl, Configuration.RateLimiter);

            if (response.StatusCode != StatusCode.Ok)
            {
                return new Response<PricePoint>
                {
                    StatusCode = response.StatusCode,
                    ErrorMessage = response.ErrorMessage
                };
            }

            if (response.Result == null)
            {
                return new Response<PricePoint>
                {
                    StatusCode = StatusCode.OtherError,
                    ErrorMessage = "Invalid data received."
                };
            }

            var pricePoint = new PricePoint
            {
                Symbol = request.Symbol,
                Price = response.Result.PriceData.Price,
                Time = DateTime.UtcNow,
                CurrencyCode = request.CurrencyCode
            };

            return new Response<PricePoint>
            {
                StatusCode = StatusCode.Ok,
                Result = pricePoint
            };
        }

        private void ValidateRange(DateTime from, DateTime to)
        {
            if (to < from)
            {
                throw new ArgumentException($"Argument {nameof(from)} cannot be later than argument {nameof(to)}");
            }
        }
    }
}
