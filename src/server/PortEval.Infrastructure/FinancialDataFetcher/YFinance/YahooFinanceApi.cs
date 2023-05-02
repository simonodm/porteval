using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Models.FinancialDataFetcher;
using PortEval.DataFetcher;
using PortEval.DataFetcher.Models;
using PortEval.DataFetcher.Responses;
using PortEval.Infrastructure.FinancialDataFetcher.Extensions;
using PortEval.Infrastructure.FinancialDataFetcher.Requests;
using PortEval.Infrastructure.FinancialDataFetcher.YFinance.Models;

namespace PortEval.Infrastructure.FinancialDataFetcher.YFinance;

/// <summary>
///     Yahoo Finance API client supporting retrieval of instrument price and split history.
/// </summary>
public class YahooFinanceApi : DataSource
{
    private const string BaseUrlV7 = "https://query2.finance.yahoo.com/v7/finance";
    private const string BaseUrlV8 = "https://query1.finance.yahoo.com/v8/finance";

    [RequestProcessor(typeof(HistoricalDailyInstrumentPricesRequest), typeof(IEnumerable<PricePoint>))]
    public async Task<Response<IEnumerable<PricePoint>>> ProcessAsync(HistoricalDailyInstrumentPricesRequest request)
    {
        var response = await GetHistoricalDataFromChartEndpoint(request.Symbol, request.From, request.To, "1d");
        return ExtractPrices(response.Result, request.From, request.To);
    }

    [RequestProcessor(typeof(IntradayInstrumentPricesRequest), typeof(IEnumerable<PricePoint>))]
    public async Task<Response<IEnumerable<PricePoint>>> ProcessAsync(IntradayInstrumentPricesRequest request)
    {
        var interval = request.Interval == IntradayInterval.OneHour ? "60m" : "5m";
        var response = await GetHistoricalDataFromChartEndpoint(request.Symbol, request.From, request.To, interval);
        return ExtractPrices(response.Result, request.From, request.To);
    }

    [RequestProcessor(typeof(LatestInstrumentPriceRequest), typeof(PricePoint))]
    public async Task<Response<PricePoint>> ProcessAsync(LatestInstrumentPriceRequest request)
    {
        return await GetCurrentPrice(request.Symbol);
    }

    [RequestProcessor(typeof(InstrumentSplitsRequest), typeof(IEnumerable<InstrumentSplitData>))]
    public async Task<Response<IEnumerable<InstrumentSplitData>>> ProcessAsync(InstrumentSplitsRequest request)
    {
        var response = await GetHistoricalDataFromChartEndpoint(request.Symbol, request.From, request.To);
        return ExtractSplits(response.Result, request.From, request.To);
    }

    [RequestProcessor(typeof(HistoricalDailyCryptoPricesRequest), typeof(IEnumerable<PricePoint>))]
    public async Task<Response<IEnumerable<PricePoint>>> ProcessAsync(HistoricalDailyCryptoPricesRequest request)
    {
        var yahooFinanceTicker = GetYahooCryptoTicker(request.Symbol, request.CurrencyCode);
        var response = await GetHistoricalDataFromChartEndpoint(yahooFinanceTicker, request.From, request.To);
        return ExtractPrices(response.Result, request.From, request.To);
    }

    [RequestProcessor(typeof(IntradayCryptoPricesRequest), typeof(IEnumerable<PricePoint>))]
    public async Task<Response<IEnumerable<PricePoint>>> ProcessAsync(IntradayCryptoPricesRequest request)
    {
        var yahooFinanceTicker = GetYahooCryptoTicker(request.Symbol, request.CurrencyCode);
        var interval = request.Interval == IntradayInterval.OneHour ? "60m" : "5m";
        var response = await GetHistoricalDataFromChartEndpoint(yahooFinanceTicker, request.From, request.To, interval);
        return ExtractPrices(response.Result, request.From, request.To);
    }

    [RequestProcessor(typeof(LatestCryptoPriceRequest), typeof(PricePoint))]
    public async Task<Response<PricePoint>> ProcessAsync(LatestCryptoPriceRequest request)
    {
        var yahooFinanceTicker = GetYahooCryptoTicker(request.Symbol, request.CurrencyCode);
        return await GetCurrentPrice(yahooFinanceTicker);
    }

    private async Task<Response<PricePoint>> GetCurrentPrice(string symbol)
    {
        var urlBuilder = new QueryUrlBuilder($"{BaseUrlV7}/quote");
        urlBuilder.AddQueryParam("symbols", symbol);
        urlBuilder.AddQueryParam("lang", "en-US");
        urlBuilder.AddQueryParam("region", "US");
        urlBuilder.AddQueryParam("corsDomain", "finance.yahoo.com");

        var url = urlBuilder.ToString();

        var result = await HttpClient.GetJsonAsync<QuoteEndpointResponse>(url, Configuration.RateLimiter);
        if (result.StatusCode != StatusCode.Ok)
        {
            return new Response<PricePoint>
            {
                StatusCode = result.StatusCode,
                ErrorMessage = result.ErrorMessage
            };
        }

        var response = result.Result.QuoteSummary.Result.FirstOrDefault();
        if (response == null)
        {
            return new Response<PricePoint>
            {
                StatusCode = StatusCode.OtherError,
                ErrorMessage = "Invalid data received."
            };
        }

        var pricePoint = new PricePoint
        {
            CurrencyCode = response.Currency,
            Symbol = symbol,
            Price = response.Price,
            Time = DateTime.UtcNow
        };

        return new Response<PricePoint>
        {
            StatusCode = StatusCode.Ok,
            Result = pricePoint
        };
    }

    private async Task<Response<ChartEndpointResponse>> GetHistoricalDataFromChartEndpoint(string symbol, DateTime from,
        DateTime to, string interval = "1d")
    {
        var queryUrlBuilder = new QueryUrlBuilder($"{BaseUrlV8}/chart/{symbol}");
        queryUrlBuilder.AddQueryParam("period1", new DateTimeOffset(from).ToUnixTimeSeconds().ToString());
        queryUrlBuilder.AddQueryParam("period2", new DateTimeOffset(to).ToUnixTimeSeconds().ToString());
        queryUrlBuilder.AddQueryParam("interval", interval);
        queryUrlBuilder.AddQueryParam("events", "splits");

        var url = queryUrlBuilder.ToString();
        return await HttpClient.GetJsonAsync<ChartEndpointResponse>(url, Configuration.RateLimiter);
    }

    private Response<IEnumerable<PricePoint>> ExtractPrices(ChartEndpointResponse response, DateTime from, DateTime to)
    {
        var timestamps = response?.Chart?.Result?.FirstOrDefault()?.Timestamps;
        var prices = response?.Chart?.Result?.FirstOrDefault()?.Indicators?.QuoteIndicators?.FirstOrDefault()
            ?.Prices;
        var meta = response?.Chart?.Result?.FirstOrDefault()?.Meta;

        if (meta == null || timestamps == null || prices == null || timestamps.Count != prices.Count)
        {
            return new Response<IEnumerable<PricePoint>>
            {
                StatusCode = StatusCode.OtherError,
                ErrorMessage = "Invalid data retrieved."
            };
        }

        var result = new List<PricePoint>();
        var currencyCode = meta.Currency;
        var symbol = meta.Symbol;

        for (var i = 0; i < timestamps.Count; i++)
        {
            var timestamp = timestamps[i];
            var price = prices[i];

            var priceDate = DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
            if (priceDate < from || priceDate > to || price == null)
            {
                continue;
            }

            var pricePoint = new PricePoint
            {
                Time = priceDate,
                CurrencyCode = currencyCode,
                Symbol = symbol,
                Price = price.Value
            };
            result.Add(pricePoint);
        }

        return new Response<IEnumerable<PricePoint>>
        {
            StatusCode = StatusCode.Ok,
            Result = result
        };
    }

    private Response<IEnumerable<InstrumentSplitData>> ExtractSplits(ChartEndpointResponse response, DateTime from,
        DateTime to)
    {
        var meta = response?.Chart?.Result?.FirstOrDefault()?.Meta;
        var splits = response?.Chart?.Result?.FirstOrDefault()?.Events?.Splits;
        if (meta == null || splits == null)
        {
            return new Response<IEnumerable<InstrumentSplitData>>
            {
                StatusCode = StatusCode.OtherError,
                ErrorMessage = "Invalid data received."
            };
        }

        var result = new List<InstrumentSplitData>();
        foreach (var (_, split) in splits)
        {
            var splitDate = DateTimeOffset.FromUnixTimeSeconds(split.Timestamp).DateTime;
            if (splitDate < from || splitDate > to)
            {
                continue;
            }

            // Convert fractions to have integer values in numerator and denominator, e.g. 2.5/1 -> 5/2
            var (intNumerator, intDenominator) = ConvertDecimalFractionToInt(split.Numerator, split.Denominator);

            var splitResult = new InstrumentSplitData
            {
                Time = splitDate,
                Numerator = intNumerator,
                Denominator = intDenominator
            };
            result.Add(splitResult);
        }

        return new Response<IEnumerable<InstrumentSplitData>>
        {
            StatusCode = StatusCode.Ok,
            Result = result
        };
    }

    private static (int, int) ConvertDecimalFractionToInt(decimal numerator, decimal denominator)
    {
        var precision = 10000;
        var gcd = FindGCD(Convert.ToInt32(numerator * precision), Convert.ToInt32(denominator * precision));

        var newNumerator = Convert.ToInt32(numerator * precision) / gcd;
        var newDenominator = Convert.ToInt32(denominator * precision) / gcd;

        return (newNumerator, newDenominator);
    }

    private static int FindGCD(int a, int b)
    {
        while (b != 0)
        {
            var remainder = a % b;
            a = b;
            b = remainder;
        }

        return a;
    }

    private static string GetYahooCryptoTicker(string symbol, string currencyCode)
    {
        var adjustedSymbol = symbol;
        if (symbol.ToLower().EndsWith(currencyCode.ToLower()))
        {
            adjustedSymbol = symbol.Substring(0, adjustedSymbol.Length - currencyCode.Length);
        }

        return adjustedSymbol + '-' + currencyCode;
    }
}