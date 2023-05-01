using System.Threading.Tasks;
using PortEval.Application.Models.FinancialDataFetcher;
using PortEval.DataFetcher;
using PortEval.DataFetcher.Responses;
using PortEval.Infrastructure.FinancialDataFetcher.Extensions;
using PortEval.Infrastructure.FinancialDataFetcher.OpenExchangeRates.Models;
using PortEval.Infrastructure.FinancialDataFetcher.Requests;

namespace PortEval.Infrastructure.FinancialDataFetcher.OpenExchangeRates;

/// <summary>
///     OpenExchangeRates API client supporting latest exchange rates.
/// </summary>
public class OpenExchangeRatesApi : DataSource
{
    private const string BaseUrl = "https://www.openexchangerates.org/api";

    [RequestProcessor(typeof(LatestExchangeRatesRequest), typeof(ExchangeRates))]
    public async Task<Response<ExchangeRates>> ProcessAsync(LatestExchangeRatesRequest request)
    {
        var queryUrlBuilder = new QueryUrlBuilder($"{BaseUrl}/latest.json");
        queryUrlBuilder.AddQueryParam("app_id", Configuration.Credentials.Token);
        queryUrlBuilder.AddQueryParam("base", request.CurrencyCode);
        
        var queryUrl = queryUrlBuilder.ToString();

        var response =
            await HttpClient.GetJsonAsync<LatestExchangeRatesResponseModel>(queryUrl, Configuration.RateLimiter);

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