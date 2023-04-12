using PortEval.Application.Models.FinancialDataFetcher;
using PortEval.DataFetcher;
using PortEval.DataFetcher.Models;
using PortEval.DataFetcher.Responses;
using PortEval.Infrastructure.FinancialDataFetcher.ExchangeRateHost.Models;
using PortEval.Infrastructure.FinancialDataFetcher.Extensions;
using PortEval.Infrastructure.FinancialDataFetcher.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Infrastructure.FinancialDataFetcher.ExchangeRateHost
{
    /// <summary>
    /// ExchangeRate.host API client supporting historical and latest exchange rates.
    /// </summary>
    public class ExchangeRateHostApi : DataSource
    {
        private const string _baseUrl = "https://api.exchangerate.host";

        [RequestProcessor(typeof(HistoricalDailyExchangeRatesRequest), typeof(IEnumerable<ExchangeRates>))]
        public async Task<Response<IEnumerable<ExchangeRates>>> ProcessAsync(HistoricalDailyExchangeRatesRequest request)
        {
            var tasks = GenerateHistoricalPricesTasks(request).ToList();
            await Task.WhenAll(tasks);

            var result = new List<ExchangeRates>();
            var anySuccessful = false;
            var anyUnexpectedError = false;

            foreach (var task in tasks)
            {
                if (task.Result.StatusCode == StatusCode.Ok) anySuccessful = true;
                else if (task.Result.StatusCode == StatusCode.OtherError) anyUnexpectedError = true;

                if (task.Result.Result != null) result.AddRange(ParseHistoricalDailyResponse(task.Result.Result));
            }

            var resultStatusCode = StatusCode.Ok;
            if (!anySuccessful)
            {
                resultStatusCode = !anyUnexpectedError ? StatusCode.OtherError : StatusCode.ConnectionError;
            }

            return new Response<IEnumerable<ExchangeRates>>
            {
                StatusCode = resultStatusCode,
                ErrorMessage = anySuccessful ? "" : "An error has occurred",
                Result = result
            };
        }

        [RequestProcessorAttribute(typeof(LatestExchangeRatesRequest), typeof(ExchangeRates))]
        public async Task<Response<ExchangeRates>> ProcessAsync(LatestExchangeRatesRequest request)
        {
            var queryUrl = $"{_baseUrl}/latest?base={request.CurrencyCode}";
            var response = await HttpClient.GetJsonAsync<ExchangeRatesLatestResponseModel>(queryUrl, Configuration?.RateLimiter);

            return new Response<ExchangeRates>
            {
                StatusCode = response.StatusCode,
                ErrorMessage = response.ErrorMessage,
                Result = response.Result != null
                    ? ParseLatestExchangeRatesResponse(response.Result)
                    : null
            };
        }

        private IEnumerable<Task<Response<ExchangeRatesTimeSeriesResponseModel>>> GenerateHistoricalPricesTasks(
            HistoricalDailyExchangeRatesRequest request)
        {
            var ranges = SplitRange(request.From, request.To);
            var tasks = new List<Task<Response<ExchangeRatesTimeSeriesResponseModel>>>();
            for (int i = 1; i < ranges.Count; i++)
            {
                var startDate = ranges[i - 1];
                var endDate = i == ranges.Count - 1 ? ranges[i] : ranges[i].AddDays(-1);
                var queryUrl =
                    $"{_baseUrl}/timeseries?start_date={startDate:yyyy-MM-dd}&end_date={endDate:yyyy-MM-dd}&base={request.CurrencyCode}";
                var task = Task.Run(async () => await HttpClient.GetJsonAsync<ExchangeRatesTimeSeriesResponseModel>(queryUrl, Configuration?.RateLimiter));
                tasks.Add(task);
            }

            return tasks;
        }

        private IEnumerable<ExchangeRates> ParseHistoricalDailyResponse(ExchangeRatesTimeSeriesResponseModel response)
        {
            var result = new List<ExchangeRates>();
            foreach (var (dayKey, exchangeRates) in response.Rates)
            {
                result.Add(new ExchangeRates
                {
                    Currency = response.Base,
                    Time = DateTime.SpecifyKind(DateTime.Parse(dayKey), DateTimeKind.Utc),
                    Rates = exchangeRates
                });
            }

            return result;
        }

        private ExchangeRates ParseLatestExchangeRatesResponse(ExchangeRatesLatestResponseModel response)
        {
            return new ExchangeRates
            {
                Time = DateTime.UtcNow,
                Currency = response.Base,
                Rates = response.Rates
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
