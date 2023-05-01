using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.FinancialDataFetcher;
using PortEval.DataFetcher;
using PortEval.DataFetcher.Models;
using PortEval.DataFetcher.Responses;
using PortEval.Infrastructure.FinancialDataFetcher.AlphaVantage.Models;
using PortEval.Infrastructure.FinancialDataFetcher.Extensions;
using PortEval.Infrastructure.FinancialDataFetcher.Requests;

namespace PortEval.Infrastructure.FinancialDataFetcher.AlphaVantage;

/// <summary>
///     AlphaVantage API client supporting historical daily, intraday and latest instrument prices.
/// </summary>
public class AlphaVantageApi : DataSource
{
    private const string BaseUrl = "https://www.alphavantage.co";

    [RequestProcessor(typeof(HistoricalDailyInstrumentPricesRequest), typeof(IEnumerable<PricePoint>))]
    public async Task<Response<IEnumerable<PricePoint>>> ProcessAsync(HistoricalDailyInstrumentPricesRequest request)
    {
        ValidateRange(request.From, request.To);

        var queryUrlBuilder = new QueryUrlBuilder($"{BaseUrl}/query");
        queryUrlBuilder.AddQueryParam("function", "TIME_SERIES_DAILY_ADJUSTED");
        queryUrlBuilder.AddQueryParam("symbol", request.Symbol);
        queryUrlBuilder.AddQueryParam("outputsize", "full");
        queryUrlBuilder.AddQueryParam("apikey", Configuration.Credentials.Token);

        var queryUrl = queryUrlBuilder.ToString();

        var response = await HttpClient.GetJsonAsync<TimeSeriesDailyResponseModel>(queryUrl, Configuration.RateLimiter);
        return ProcessTimeSeriesResponse(request, response?.Result?.Prices);
    }

    [RequestProcessor(typeof(IntradayInstrumentPricesRequest), typeof(IEnumerable<PricePoint>))]
    public async Task<Response<IEnumerable<PricePoint>>> ProcessAsync(IntradayInstrumentPricesRequest request)
    {
        var interval = request.Interval == IntradayInterval.FiveMinutes ? "5min" : "60min";

        var queryUrlBuilder = new QueryUrlBuilder($"{BaseUrl}/query");
        queryUrlBuilder.AddQueryParam("function", "TIME_SERIES_INTRADAY");
        queryUrlBuilder.AddQueryParam("symbol", request.Symbol);
        queryUrlBuilder.AddQueryParam("interval", interval);
        queryUrlBuilder.AddQueryParam("outputsize", "full");
        queryUrlBuilder.AddQueryParam("apikey", Configuration.Credentials.Token);

        var queryUrl = queryUrlBuilder.ToString();

        var response =
            await HttpClient.GetJsonAsync<TimeSeriesIntradayResponseModel>(queryUrl, Configuration.RateLimiter);

        return ProcessTimeSeriesResponse(request, response?.Result?.Prices);
    }

    [RequestProcessor(typeof(LatestInstrumentPriceRequest), typeof(PricePoint))]
    public async Task<Response<PricePoint>> ProcessAsync(LatestInstrumentPriceRequest request)
    {
        var queryUrlBuilder = new QueryUrlBuilder($"{BaseUrl}/query");
        queryUrlBuilder.AddQueryParam("function", "GLOBAL_QUOTE");
        queryUrlBuilder.AddQueryParam("symbol", request.Symbol);
        queryUrlBuilder.AddQueryParam("apikey", Configuration.Credentials.Token);

        var queryUrl = queryUrlBuilder.ToString();

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

    private Response<IEnumerable<PricePoint>> ProcessTimeSeriesResponse(IInstrumentTimeRangeRequest request,
        Dictionary<string, TimeSeriesPriceDataModel> prices)
    {
        var result = new List<PricePoint>();
        if (prices == null)
        {
            return new Response<IEnumerable<PricePoint>>
            {
                StatusCode = StatusCode.OtherError,
                ErrorMessage = "Invalid data received."
            };
        }

        var splitCoefficient = 1m;

        foreach (var (timeKey, priceData) in prices)
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
                Price = price / splitCoefficient,
                CurrencyCode = request.CurrencyCode
            };

            result.Add(pricePoint);

            if (priceData.SplitCoefficient != 0)
            {
                splitCoefficient *= priceData.SplitCoefficient;
            }
        }

        return new Response<IEnumerable<PricePoint>>
        {
            StatusCode = StatusCode.Ok,
            Result = result
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