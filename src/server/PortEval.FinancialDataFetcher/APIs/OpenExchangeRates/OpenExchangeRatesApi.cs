using PortEval.FinancialDataFetcher.APIs.OpenExchangeRates.Models;
using PortEval.FinancialDataFetcher.Interfaces.APIs;
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
    internal class OpenExchangeRatesApi : ILatestExchangeRatesFinancialApi
    {
        private const string _baseUrl = "https://www.openexchangerates.org/api";
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly RateLimiter _rateLimiter;

        public OpenExchangeRatesApi(HttpClient httpClient, string apiKey, RateLimiter rateLimiter = null)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
            _rateLimiter = rateLimiter;
        }

        public async Task<Response<ExchangeRates>> Process(LatestExchangeRatesRequest request)
        {
            var queryUrl = $"{_baseUrl}/latest.json?app_id={_apiKey}&base={request.CurrencyCode}";

            var response = await _httpClient.GetJson<LatestExchangeRatesResponseModel>(queryUrl, _rateLimiter);

            return new Response<ExchangeRates>
            {
                StatusCode = response.StatusCode,
                ErrorMessage = response.ErrorMessage,
                Result = response.Result != null
                    ? ParseLatestExchangeRatesResponse(response.Result)
                    : null
            };
        }

        private ExchangeRates ParseLatestExchangeRatesResponse(LatestExchangeRatesResponseModel response)
        {
            return new ExchangeRates
            {
                Currency = response.Base,
                Rates = response.Rates,
                Time = response.Time.ToUniversalTime()
            };
        }
    }
}
