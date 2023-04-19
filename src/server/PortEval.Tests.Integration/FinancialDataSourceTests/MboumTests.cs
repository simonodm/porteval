using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.FinancialDataFetcher;
using PortEval.DataFetcher;
using PortEval.DataFetcher.Models;
using PortEval.Infrastructure.FinancialDataFetcher.RapidAPIMboum;
using PortEval.Infrastructure.FinancialDataFetcher.RapidAPIMboum.Models;
using PortEval.Infrastructure.FinancialDataFetcher.Requests;
using Xunit;

namespace PortEval.Tests.Integration.FinancialDataSourceTests;

public class MboumTests
{
    [Fact]
    public async Task ProcessHistoricalDailyInstrumentPricesRequest_ReturnsDataFromMboumHistoryEndpoint()
    {
        var priceTime = DateTime.Parse("2022-01-01T00:00:00Z").ToUniversalTime();
        var priceTimeStamp = new DateTimeOffset(priceTime).ToUnixTimeSeconds();
        var instrumentSymbol = "AAPL";

        var expectedRequestUri =
            @$"https://mboum-finance.p.rapidapi.com/hi/history?symbol={instrumentSymbol}&interval=1d";
        var price = new MboumHistoricalDataItem
        {
            Open = 100m,
            Close = 100m,
            Date = priceTime.ToString("yyyy-MM-dd"),
            DateUtc = priceTimeStamp
        };

        var apiMockResponse = new MboumHistoricalDataResponse
        {
            Items = new Dictionary<long, MboumHistoricalDataItem>
            {
                {
                    priceTimeStamp,
                    price
                }
            }
        };

        var httpClient = Helpers.SetupMockHttpClientReturningResponse(expectedRequestUri, apiMockResponse);

        var request = new HistoricalDailyInstrumentPricesRequest
        {
            From = DateTime.Parse("2022-01-01"),
            To = DateTime.Parse("2022-01-03"),
            Symbol = instrumentSymbol,
            CurrencyCode = "USD"
        };

        var sut = new DataFetcher.DataFetcher(httpClient);
        sut.RegisterDataSource<MboumApi>(GetMockConfiguration());

        var response =
            await sut.ProcessRequestAsync<HistoricalDailyInstrumentPricesRequest, IEnumerable<PricePoint>>(request);

        Assert.Equal(StatusCode.Ok, response.StatusCode);
        Assert.Collection(response.Result, pricePoint =>
        {
            Assert.Equal(priceTime, pricePoint.Time);
            Assert.Equal(price.Open, pricePoint.Price);
            Assert.Equal("USD", pricePoint.CurrencyCode);
            Assert.Equal(instrumentSymbol, pricePoint.Symbol);
        });
    }

    [Fact]
    public async Task ProcessIntradayInstrumentPricesRequest_ReturnsDataFromMboumHistoryEndpoint()
    {
        var priceTime = DateTime.Parse("2022-01-01T00:00:00Z").ToUniversalTime();
        var priceTimeStamp = new DateTimeOffset(priceTime).ToUnixTimeSeconds();
        var instrumentSymbol = "AAPL";

        var expectedRequestUri =
            @$"https://mboum-finance.p.rapidapi.com/hi/history?symbol={instrumentSymbol}&interval=1h";
        var price = new MboumHistoricalDataItem
        {
            Open = 100m,
            Close = 100m,
            Date = priceTime.ToString("yyyy-MM-dd"),
            DateUtc = priceTimeStamp
        };

        var apiMockResponse = new MboumHistoricalDataResponse
        {
            Items = new Dictionary<long, MboumHistoricalDataItem>
            {
                {
                    priceTimeStamp,
                    price
                }
            }
        };

        var httpClient = Helpers.SetupMockHttpClientReturningResponse(expectedRequestUri, apiMockResponse);

        var request = new IntradayInstrumentPricesRequest
        {
            From = DateTime.Parse("2022-01-01"),
            To = DateTime.Parse("2022-01-03"),
            Symbol = instrumentSymbol,
            CurrencyCode = "USD",
            Interval = IntradayInterval.OneHour
        };

        var sut = new DataFetcher.DataFetcher(httpClient);
        sut.RegisterDataSource<MboumApi>(GetMockConfiguration());

        var response = await sut.ProcessRequestAsync<IntradayInstrumentPricesRequest, IEnumerable<PricePoint>>(request);

        Assert.Equal(StatusCode.Ok, response.StatusCode);
        Assert.Collection(response.Result, pricePoint =>
        {
            Assert.Equal(priceTime, pricePoint.Time);
            Assert.Equal(price.Open, pricePoint.Price);
            Assert.Equal("USD", pricePoint.CurrencyCode);
            Assert.Equal(instrumentSymbol, pricePoint.Symbol);
        });
    }

    [Fact]
    public async Task ProcessLatestInstrumentPriceRequest_ReturnsDataFromMboumQuoteEndpoint()
    {
        var instrumentSymbol = "AAPL";

        var expectedRequestUri = @$"https://mboum-finance.p.rapidapi.com/qu/quote?symbol={instrumentSymbol}";

        var quote = new MboumQuoteDataResponse
        {
            Currency = "USD",
            Exchange = "NASDAQ",
            RegularMarketPrice = 100m
        };
        var apiMockResponse = new List<MboumQuoteDataResponse>
        {
            quote
        };

        var httpClient = Helpers.SetupMockHttpClientReturningResponse(expectedRequestUri, apiMockResponse);

        var request = new LatestInstrumentPriceRequest
        {
            Symbol = instrumentSymbol,
            CurrencyCode = "USD"
        };

        var sut = new DataFetcher.DataFetcher(httpClient);
        sut.RegisterDataSource<MboumApi>(GetMockConfiguration());

        var response = await sut.ProcessRequestAsync<LatestInstrumentPriceRequest, PricePoint>(request);

        Assert.Equal(StatusCode.Ok, response.StatusCode);
        Assert.Equal(quote.RegularMarketPrice, response.Result.Price);
        Assert.Equal(quote.Currency, response.Result.CurrencyCode);
        Assert.Equal(instrumentSymbol, response.Result.Symbol);
    }

    [Fact]
    public async Task ProcessInstrumentSplitsRequest_ReturnsDataFromMboumHistoryEndpoint()
    {
        var splitTime = DateTime.Parse("2022-01-01T00:00:00Z").ToUniversalTime();
        var splitTimeStamp = new DateTimeOffset(splitTime).ToUnixTimeSeconds();

        var instrumentSymbol = "AAPL";

        var expectedRequestUri = @$"https://mboum-finance.p.rapidapi.com/hi/history?symbol={instrumentSymbol}";

        var split = new MboumSplit
        {
            Date = splitTimeStamp,
            Numerator = 3,
            Denominator = 2
        };

        var apiMockResponse = new MboumHistoricalDataResponse
        {
            Events = new MboumEvents
            {
                Splits = new Dictionary<long, MboumSplit>
                {
                    {
                        splitTimeStamp,
                        split
                    }
                }
            }
        };

        var httpClient = Helpers.SetupMockHttpClientReturningResponse(expectedRequestUri, apiMockResponse);

        var request = new InstrumentSplitsRequest
        {
            Symbol = instrumentSymbol,
            From = DateTime.Parse("2021-01-01"),
            To = DateTime.Parse("2023-01-01")
        };

        var sut = new DataFetcher.DataFetcher(httpClient);
        sut.RegisterDataSource<MboumApi>(GetMockConfiguration());

        var response =
            await sut.ProcessRequestAsync<InstrumentSplitsRequest, IEnumerable<InstrumentSplitData>>(request);

        Assert.Equal(StatusCode.Ok, response.StatusCode);
        Assert.Collection(response.Result, splitData =>
        {
            Assert.Equal(splitTime, splitData.Time);
            Assert.Equal(split.Numerator, splitData.Numerator);
            Assert.Equal(split.Denominator, splitData.Denominator);
        });
    }

    private DataSourceConfiguration GetMockConfiguration()
    {
        return new DataSourceConfiguration
        {
            Credentials = new DataSourceCredentials
            {
                Token = "test-token"
            }
        };
    }
}