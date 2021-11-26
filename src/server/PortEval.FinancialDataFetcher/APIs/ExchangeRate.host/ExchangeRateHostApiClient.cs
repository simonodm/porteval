using Newtonsoft.Json.Linq;
using PortEval.FinancialDataFetcher.APIs.ExchangeRate.host.Models;
using PortEval.FinancialDataFetcher.APIs.Interfaces;
using PortEval.FinancialDataFetcher.Models;
using PortEval.FinancialDataFetcher.Requests;
using PortEval.FinancialDataFetcher.Responses;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ExchangeRates = PortEval.FinancialDataFetcher.Models.ExchangeRates;

namespace PortEval.FinancialDataFetcher.APIs.ExchangeRate.host
{
    /// <summary>
    /// ExchangeRate.host API client supporting historical and latest exchange rates.
    /// </summary>
    internal class ExchangeRateHostApiClient : IHistoricalDailyExchangeRatesFinancialApiClient, ILatestExchangeRatesFinancialApiClient
    {
        private const string _baseUrl = "https://api.exchangerate.host";
        private readonly HttpClient _httpClient;
        private readonly RateLimiter _rateLimiter;

        public ExchangeRateHostApiClient(HttpClient httpClient, RateLimiter rateLimiter = null)
        {
            _httpClient = httpClient;
            _rateLimiter = rateLimiter;
        }

        public async Task<Response<IEnumerable<ExchangeRates>>> Process(HistoricalDailyExchangeRatesRequest request)
        {
            var ranges = SplitRange(request.From, request.To);
            var tasks = new List<Task<Response<IEnumerable<ExchangeRates>>>>();
            for (int i = 1; i < ranges.Count; i++)
            {
                var startDate = ranges[i - 1];
                var endDate = i == ranges.Count - 1 ? ranges[i] : ranges[i].AddDays(-1);
                var queryUrl =
                    $"{_baseUrl}/timeseries?start_date={startDate:yyyy-MM-dd}&end_date={endDate:yyyy-MM-dd}&base={request.Symbol}";
                var task = Task.Run(async () => await _httpClient.FetchJson(queryUrl, ParseHistoricalDailyResponse, _rateLimiter));
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            var result = new List<ExchangeRates>();
            var anySuccessful = false;

            foreach (var task in tasks)
            {
                if (task.Result.StatusCode == StatusCode.Ok) anySuccessful = true;
                if (task.Result.Result != null) result.AddRange(task.Result.Result);
            }

            return new Response<IEnumerable<ExchangeRates>>
            {
                StatusCode = anySuccessful ? StatusCode.Ok : StatusCode.ConnectionError,
                ErrorMessage = anySuccessful ? "" : "An error has occurred",
                Result = result
            };
        }

        public async Task<Response<ExchangeRates>> Process(LatestExchangeRatesRequest request)
        {
            var queryUrl = $"{_baseUrl}/latest?base={request.Symbol}";
            return await _httpClient.FetchJson(queryUrl, ParseLatestExchangeRatesResponse, _rateLimiter);
        }

        private Response<IEnumerable<ExchangeRates>> ParseHistoricalDailyResponse(JToken response)
        {
            var responseModel = response.ToObject<ExchangeRatesTimeSeriesResponseModel>();

            var result = new List<ExchangeRates>();
            foreach (var (dayKey, exchangeRates) in responseModel.Rates)
            {
                result.Add(new ExchangeRates
                {
                    Currency = responseModel.Base,
                    Time = TimeZoneInfo.ConvertTimeToUtc(DateTime.Parse(dayKey)),
                    Rates = exchangeRates
                });
            }

            return new Response<IEnumerable<ExchangeRates>>
            {
                StatusCode = StatusCode.Ok,
                Result = result
            };
        }

        private Response<ExchangeRates> ParseLatestExchangeRatesResponse(JToken response)
        {
            var responseModel = response.ToObject<ExchangeRatesLatestResponseModel>();
            var resultRates = new ExchangeRates
            {
                Time = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now),
                Currency = responseModel.Base,
                Rates = responseModel.Rates
            };

            return new Response<ExchangeRates>
            {
                StatusCode = StatusCode.Ok,
                Result = resultRates
            };
        }

        private List<DateTime> SplitRange(DateTime from, DateTime to)
        {
            var dateTimes = new List<DateTime>();
            var current = from;
            while (current < to)
            {
                dateTimes.Add(current);
                current = current.AddDays(365);
            }

            dateTimes.Add(to);
            return dateTimes;
        }
    }
}
