using System.Net.Http;
using PortEval.DataFetcher;
using PortEval.DataFetcher.Responses;
using PortEval.Infrastructure.FinancialDataFetcher.OpenExchangeRates.Models;
using PortEval.Infrastructure.FinancialDataFetcher.Requests;
using System.Threading.Tasks;
using PortEval.Application.Models.FinancialDataFetcher;
using PortEval.Infrastructure.FinancialDataFetcher.Extensions;

namespace PortEval.Infrastructure.FinancialDataFetcher.OpenExchangeRates
{
    /// <summary>
    /// OpenExchangeRates API client supporting latest exchange rates.
    /// </summary>
    public class OpenExchangeRatesApi : DataSource
    {
        private const string _baseUrl = "https://www.openexchangerates.org/api";

        [RequestProcessor(typeof(LatestExchangeRatesRequest), typeof(ExchangeRates))]
        public async Task<Response<ExchangeRates>> ProcessAsync(LatestExchangeRatesRequest request)
        {
            var queryUrl = $"{_baseUrl}/latest.json?app_id={Configuration.Credentials.Token}&base={request.CurrencyCode}";

            var response = await HttpClient.GetJsonAsync<LatestExchangeRatesResponseModel>(queryUrl, Configuration.RateLimiter);

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
