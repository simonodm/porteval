using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.FinancialDataFetcher;
using PortEval.DataFetcher;
using PortEval.DataFetcher.Models;
using PortEval.Infrastructure.FinancialDataFetcher.Requests;
using PortEval.Infrastructure.FinancialDataFetcher.YFinance;
using PortEval.Infrastructure.FinancialDataFetcher.YFinance.Models;
using Xunit;

namespace PortEval.Tests.Integration.FinancialDataSourceTests;

public class YahooFinanceTests
{
    [Fact]
    public async Task ProcessHistoricalDailyInstrumentPricesRequest_ReturnsDataFromYFinanceChartEndpoint()
    {
        var priceTime = DateTime.Parse("2022-01-01T00:00:00Z").ToUniversalTime();
        var instrumentSymbol = "AAPL";

        var from = DateTime.Parse("2022-01-01");
        var to = DateTime.Parse("2022-01-03");

        var period1 = new DateTimeOffset(from).ToUnixTimeSeconds();
        var period2 = new DateTimeOffset(to).ToUnixTimeSeconds();
        
        var expectedRequestUri =
            @$"https://query1.finance.yahoo.com/v8/finance/chart/{instrumentSymbol}?period1={period1}&period2={period2}&interval=1d";
        var price = 100m;
        var timestamp = new DateTimeOffset(priceTime).ToUnixTimeSeconds();
        var apiMockResponse = new ChartEndpointResponse
        {
            Chart = new YahooFinanceResponse<List<ChartEndpointResult>>
            {
                Result = new List<ChartEndpointResult>
                {
                    new()
                    {
                        Indicators = new ChartIndicators
                        {
                            QuoteIndicators = new List<QuoteIndicator>
                            {
                                new()
                                {
                                    Prices = new List<decimal?> { price }
                                }
                            }
                        },
                        Meta = new TickerMeta
                        {
                            Symbol = instrumentSymbol,
                            Currency = "USD"
                        },
                        Timestamps = new List<long> { timestamp }
                    }
                }
            }
        };

        var httpClient = Helpers.SetupMockHttpClientReturningResponse(expectedRequestUri, apiMockResponse);

        var request = new HistoricalDailyInstrumentPricesRequest
        {
            From = from,
            To = to,
            Symbol = instrumentSymbol,
            CurrencyCode = "USD"
        };

        var sut = new DataFetcher.DataFetcher(httpClient);
        sut.RegisterDataSource<YahooFinanceApi>(new DataSourceConfiguration());

        var response =
            await sut.ProcessRequestAsync<HistoricalDailyInstrumentPricesRequest, IEnumerable<PricePoint>>(request);

        Assert.Equal(StatusCode.Ok, response.StatusCode);
        Assert.Collection(response.Result, pricePoint =>
        {
            Assert.Equal(priceTime, pricePoint.Time);
            Assert.Equal(price, pricePoint.Price);
            Assert.Equal("USD", pricePoint.CurrencyCode);
            Assert.Equal(instrumentSymbol, pricePoint.Symbol);
        });
    }

    [Fact]
    public async Task ProcessIntradayInstrumentPricesRequest_ReturnsDataFromYFinanceChartEndpoint()
    {
        var priceTime = DateTime.Parse("2022-01-01T00:00:00Z").ToUniversalTime();
        var instrumentSymbol = "AAPL";

        var from = DateTime.Parse("2022-01-01");
        var to = DateTime.Parse("2022-01-03");

        var period1 = new DateTimeOffset(from).ToUnixTimeSeconds();
        var period2 = new DateTimeOffset(to).ToUnixTimeSeconds();
        
        var expectedRequestUri =
            @$"https://query1.finance.yahoo.com/v8/finance/chart/{instrumentSymbol}?period1={period1}&period2={period2}&interval=60m";
        var price = 100m;
        var timestamp = new DateTimeOffset(priceTime).ToUnixTimeSeconds();
        var apiMockResponse = new ChartEndpointResponse
        {
            Chart = new YahooFinanceResponse<List<ChartEndpointResult>>
            {
                Result = new List<ChartEndpointResult>
                {
                    new()
                    {
                        Indicators = new ChartIndicators
                        {
                            QuoteIndicators = new List<QuoteIndicator>
                            {
                                new()
                                {
                                    Prices = new List<decimal?> { price }
                                }
                            }
                        },
                        Meta = new TickerMeta
                        {
                            Symbol = instrumentSymbol,
                            Currency = "USD"
                        },
                        Timestamps = new List<long> { timestamp }
                    }
                }
            }
        };

        var httpClient = Helpers.SetupMockHttpClientReturningResponse(expectedRequestUri, apiMockResponse);

        var request = new IntradayInstrumentPricesRequest
        {
            From = from,
            To = to,
            Symbol = instrumentSymbol,
            CurrencyCode = "USD",
            Interval = IntradayInterval.OneHour
        };

        var sut = new DataFetcher.DataFetcher(httpClient);
        sut.RegisterDataSource<YahooFinanceApi>(new DataSourceConfiguration());

        var response = await sut.ProcessRequestAsync<IntradayInstrumentPricesRequest, IEnumerable<PricePoint>>(request);

        Assert.Equal(StatusCode.Ok, response.StatusCode);
        Assert.Collection(response.Result, pricePoint =>
        {
            Assert.Equal(priceTime, pricePoint.Time);
            Assert.Equal(price, pricePoint.Price);
            Assert.Equal("USD", pricePoint.CurrencyCode);
            Assert.Equal(instrumentSymbol, pricePoint.Symbol);
        });
    }

    [Fact]
    public async Task ProcessLatestInstrumentPriceRequest_ReturnsDataFromYFinanceQuoteEndpoint()
    {
        var instrumentSymbol = "AAPL";
        var price = 100m;

        var apiMockResponse = new QuoteEndpointResponse
        {
            QuoteSummary = new YahooFinanceResponse<List<Quote>>
            {
                Result = new List<Quote>
                {
                    new()
                    {
                        Currency = "USD",
                        Price = price
                    }
                }
            }
        };

        var expectedRequestUri = @$"https://query2.finance.yahoo.com/v7/finance/quote?symbols={instrumentSymbol}";

        var httpClient = Helpers.SetupMockHttpClientReturningResponse(expectedRequestUri, apiMockResponse);

        var request = new LatestInstrumentPriceRequest
        {
            Symbol = instrumentSymbol,
            CurrencyCode = "USD"
        };

        var sut = new DataFetcher.DataFetcher(httpClient);
        sut.RegisterDataSource<YahooFinanceApi>(new DataSourceConfiguration());

        var response = await sut.ProcessRequestAsync<LatestInstrumentPriceRequest, PricePoint>(request);

        Assert.Equal(StatusCode.Ok, response.StatusCode);
        Assert.Equal(price, response.Result.Price);
        Assert.Equal("USD", response.Result.CurrencyCode);
        Assert.Equal(instrumentSymbol, response.Result.Symbol);
    }

    [Fact]
    public async Task ProcessInstrumentSplitsRequest_ReturnsDataFromYFinanceChartEndpointEndpoint()
    {
        var splitTime = DateTime.Parse("2022-01-01T00:00:00Z").ToUniversalTime();
        var splitTimeStamp = new DateTimeOffset(splitTime).ToUnixTimeSeconds();

        var instrumentSymbol = "AAPL";

        var from = DateTime.Parse("2022-01-01");
        var to = DateTime.Parse("2022-01-03");

        var period1 = new DateTimeOffset(from).ToUnixTimeSeconds();
        var period2 = new DateTimeOffset(to).ToUnixTimeSeconds();
        
        var expectedRequestUri =
            @$"https://query1.finance.yahoo.com/v8/finance/chart/{instrumentSymbol}?period1={period1}&period2={period2}&interval=1d&events=splits";

        var split = new ChartEndpointSplit
        {
            Timestamp = splitTimeStamp,
            Numerator = 2.5m,
            Denominator = 1m
        };

        var apiMockResponse = new ChartEndpointResponse
        {
            Chart = new YahooFinanceResponse<List<ChartEndpointResult>>
            {
                Result = new List<ChartEndpointResult>
                {
                    new()
                    {
                        Meta = new TickerMeta
                        {
                            Currency = "USD",
                            Symbol = instrumentSymbol
                        },
                        Events = new ChartEndpointEvents
                        {
                            Splits = new Dictionary<string, ChartEndpointSplit>
                            {
                                { splitTimeStamp.ToString(), split }
                            }
                        }
                    }
                }
            }
        };

        var httpClient = Helpers.SetupMockHttpClientReturningResponse(expectedRequestUri, apiMockResponse);

        var request = new InstrumentSplitsRequest
        {
            Symbol = instrumentSymbol,
            From = from,
            To = to
        };

        var sut = new DataFetcher.DataFetcher(httpClient);
        sut.RegisterDataSource<YahooFinanceApi>(new DataSourceConfiguration());

        var response =
            await sut.ProcessRequestAsync<InstrumentSplitsRequest, IEnumerable<InstrumentSplitData>>(request);

        Assert.Equal(StatusCode.Ok, response.StatusCode);
        Assert.Collection(response.Result, splitData =>
        {
            Assert.Equal(splitTime, splitData.Time);
            Assert.Equal(split.Numerator * 2, splitData.Numerator);
            Assert.Equal(split.Denominator * 2, splitData.Denominator);
        });
    }
}