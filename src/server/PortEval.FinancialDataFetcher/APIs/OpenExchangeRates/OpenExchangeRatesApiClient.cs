using Newtonsoft.Json.Linq;
using PortEval.FinancialDataFetcher.APIs.Interfaces;
using PortEval.FinancialDataFetcher.APIs.OpenExchangeRates.Models;
using PortEval.FinancialDataFetcher.Models;
using PortEval.FinancialDataFetcher.Requests;
using PortEval.FinancialDataFetcher.Responses;
using System.Net.Http;
using System.Threading.Tasks;

namespace PortEval.FinancialDataFetcher.APIs.OpenExchangeRates
{
    /// <summary>
    /// OpenExchangeRates API client supporting latest exchange rates.
    /// </summary>
    internal class OpenExchangeRatesApiClient : ILatestExchangeRatesFinancialApiClient
    {
        private const string _baseUrl = "https://www.openexchangerates.org/api";
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly RateLimiter _rateLimiter;

        public OpenExchangeRatesApiClient(HttpClient httpClient, string apiKey, RateLimiter rateLimiter = null)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
            _rateLimiter = rateLimiter;
        }

        public async Task<Response<ExchangeRates>> Process(LatestExchangeRatesRequest request)
        {
            var queryUrl = $"{_baseUrl}/latest.json?app_id={_apiKey}&base={request.Symbol}";

            return await _httpClient.FetchJson(queryUrl, ParseLatestExchangeRates, _rateLimiter);
        }

        private Response<ExchangeRates> ParseLatestExchangeRates(JToken response)
        {
            var exchangeRatesResponse = response.ToObject<LatestExchangeRatesResponseModel>();
            if (exchangeRatesResponse != null)
            {
                var result = new ExchangeRates
                {
                    Currency = exchangeRatesResponse.Base,
                    Rates = exchangeRatesResponse.Rates,
                    Time = exchangeRatesResponse.Time
                };

                return new Response<ExchangeRates>
                {
                    StatusCode = StatusCode.Ok,
                    Result = result
                };
            }

            return new Response<ExchangeRates>
            {
                StatusCode = StatusCode.OtherError,
                ErrorMessage = "Invalid data received."
            };
        }
    }
}
