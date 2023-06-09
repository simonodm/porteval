﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.FinancialDataFetcher;
using PortEval.DataFetcher;
using PortEval.DataFetcher.Models;
using PortEval.Infrastructure.FinancialDataFetcher.Requests;
using PortEval.Infrastructure.FinancialDataFetcher.Tiingo;
using PortEval.Infrastructure.FinancialDataFetcher.Tiingo.Models;
using Xunit;

namespace PortEval.Tests.Integration.FinancialDataSourceTests;

public class TiingoTests
{
    [Fact]
    public async Task ProcessHistoricalDailyInstrumentPricesRequest_ReturnsDataFromTiingoEODEndpoint()
    {
        var priceTime = DateTime.Parse("2022-01-01T12:00:00Z").ToUniversalTime();
        var instrumentSymbol = "AAPL";

        var expectedRequestUri = @"https://api.tiingo.com/tiingo/daily/AAPL/prices";
        var apiMockResponse = new List<TiingoPriceResponseModel>
        {
            new()
            {
                Time = priceTime,
                Price = 100m
            }
        };

        var httpClient = Helpers.SetupMockHttpClientReturningResponse(expectedRequestUri, apiMockResponse);

        var request = new HistoricalDailyInstrumentPricesRequest
        {
            From = DateTime.Parse("2022-01-01"),
            To = DateTime.Parse("2022-01-03"),
            Symbol = instrumentSymbol
        };

        var sut = new DataFetcher.DataFetcher(httpClient);
        sut.RegisterDataSource<TiingoApi>(GetMockConfiguration());

        var response =
            await sut.ProcessRequestAsync<HistoricalDailyInstrumentPricesRequest, IEnumerable<PricePoint>>(request);

        Assert.Equal(StatusCode.Ok, response.StatusCode);
        Assert.Collection(response.Result, pricePoint =>
        {
            Assert.Equal(apiMockResponse[0].Time, pricePoint.Time);
            Assert.Equal(apiMockResponse[0].Price, pricePoint.Price);
            Assert.Equal("USD", pricePoint.CurrencyCode);
            Assert.Equal(instrumentSymbol, pricePoint.Symbol);
        });
    }

    [Fact]
    public async Task ProcessHistoricalDailyCryptoPricesRequest_ReturnsDataFromTiingoCryptoHistoricalEndpoint()
    {
        var priceTime = DateTime.Parse("2022-01-02T12:00:00Z").ToUniversalTime();
        var instrumentSymbol = "BTC";

        var expectedRequestUri = @"https://api.tiingo.com/tiingo/crypto/prices";
        var apiMockResponse = new List<TiingoCryptoPriceResponseModel>
        {
            new()
            {
                Ticker = "BTCUSD",
                BaseCurrency = "BTC",
                QuoteCurrency = "USD",
                Data = new List<TiingoCryptoPricePoint>
                {
                    new()
                    {
                        Time = priceTime,
                        Price = 100m
                    }
                }
            }
        };

        var httpClient = Helpers.SetupMockHttpClientReturningResponse(expectedRequestUri, apiMockResponse);

        var request = new HistoricalDailyCryptoPricesRequest
        {
            CurrencyCode = "USD",
            From = DateTime.Parse("2022-01-01"),
            To = DateTime.Parse("2022-01-03"),
            Symbol = instrumentSymbol
        };

        var sut = new DataFetcher.DataFetcher(httpClient);
        sut.RegisterDataSource<TiingoApi>(GetMockConfiguration());

        var response =
            await sut.ProcessRequestAsync<HistoricalDailyCryptoPricesRequest, IEnumerable<PricePoint>>(request);

        Assert.Equal(StatusCode.Ok, response.StatusCode);
        Assert.Collection(response.Result, pricePoint =>
        {
            Assert.Equal(apiMockResponse[0].Data[0].Time, pricePoint.Time);
            Assert.Equal(apiMockResponse[0].Data[0].Price, pricePoint.Price);
            Assert.Equal("USD", pricePoint.CurrencyCode);
            Assert.Equal(instrumentSymbol, pricePoint.Symbol);
        });
    }

    [Fact]
    public async Task ProcessIntradayCryptoPricesRequest_ReturnsDataFromTiingoCryptoHistoricalEndpoint()
    {
        var priceTime = DateTime.Parse("2022-01-02T12:00:00Z").ToUniversalTime();
        var instrumentSymbol = "BTC";

        var expectedRequestUri = @"https://api.tiingo.com/tiingo/crypto/prices";
        var apiMockResponse = new List<TiingoCryptoPriceResponseModel>
        {
            new()
            {
                Ticker = "BTCUSD",
                BaseCurrency = "BTC",
                QuoteCurrency = "USD",
                Data = new List<TiingoCryptoPricePoint>
                {
                    new()
                    {
                        Time = priceTime,
                        Price = 100m
                    }
                }
            }
        };

        var httpClient = Helpers.SetupMockHttpClientReturningResponse(expectedRequestUri, apiMockResponse);

        var request = new IntradayCryptoPricesRequest
        {
            CurrencyCode = "USD",
            From = DateTime.Parse("2022-01-01"),
            To = DateTime.Parse("2022-01-03"),
            Symbol = instrumentSymbol,
            Interval = IntradayInterval.OneHour
        };

        var sut = new DataFetcher.DataFetcher(httpClient);
        sut.RegisterDataSource<TiingoApi>(GetMockConfiguration());

        var response = await sut.ProcessRequestAsync<IntradayCryptoPricesRequest, IEnumerable<PricePoint>>(request);

        Assert.Equal(StatusCode.Ok, response.StatusCode);
        Assert.Collection(response.Result, pricePoint =>
        {
            Assert.Equal(apiMockResponse[0].Data[0].Time, pricePoint.Time);
            Assert.Equal(apiMockResponse[0].Data[0].Price, pricePoint.Price);
            Assert.Equal("USD", pricePoint.CurrencyCode);
            Assert.Equal(instrumentSymbol, pricePoint.Symbol);
        });
    }

    [Fact]
    public async Task ProcessLatestInstrumentPriceRequest_ReturnsDataFromTiingoIEXEndpoint()
    {
        var expectedRequestUri = @"https://api.tiingo.com/iex/AAPL";
        var apiMockResponse = new List<TiingoIexTopPriceResponseModel>
        {
            new()
            {
                Price = 100m
            }
        };

        var instrumentSymbol = "AAPL";

        var httpClient = Helpers.SetupMockHttpClientReturningResponse(expectedRequestUri, apiMockResponse);

        var request = new LatestInstrumentPriceRequest
        {
            Symbol = instrumentSymbol
        };

        var sut = new DataFetcher.DataFetcher(httpClient);
        sut.RegisterDataSource<TiingoApi>(GetMockConfiguration());

        var response = await sut.ProcessRequestAsync<LatestInstrumentPriceRequest, PricePoint>(request);

        Assert.Equal(StatusCode.Ok, response.StatusCode);
        Assert.Equal(apiMockResponse[0].Price, response.Result.Price);
        Assert.Equal("USD", response.Result.CurrencyCode);
        Assert.Equal(instrumentSymbol, response.Result.Symbol);
    }

    [Fact]
    public async Task ProcessLatestCryptoPriceRequest_ReturnsDataFromTiingoCryptoTopOfBookEndpoint()
    {
        var priceTime = DateTime.Parse("2022-01-02T12:00:00Z").ToUniversalTime();
        var instrumentSymbol = "BTC";

        var expectedRequestUri = @"https://api.tiingo.com/tiingo/crypto/top";
        var apiMockResponse = new List<TiingoCryptoTopPriceResponseModel>
        {
            new()
            {
                Ticker = "BTCUSD",
                BaseCurrency = "BTC",
                QuoteCurrency = "USD",
                Data = new List<TiingoCryptoTopPriceDataResponseModel>
                {
                    new()
                    {
                        LastPrice = 100m,
                        AskPrice = 100m,
                        BidPrice = 100m
                    }
                }
            }
        };

        var httpClient = Helpers.SetupMockHttpClientReturningResponse(expectedRequestUri, apiMockResponse);

        var request = new LatestCryptoPriceRequest
        {
            CurrencyCode = "USD",
            Symbol = instrumentSymbol
        };

        var sut = new DataFetcher.DataFetcher(httpClient);
        sut.RegisterDataSource<TiingoApi>(GetMockConfiguration());

        var response = await sut.ProcessRequestAsync<LatestCryptoPriceRequest, PricePoint>(request);

        Assert.Equal(StatusCode.Ok, response.StatusCode);
        Assert.Equal(apiMockResponse[0].Data[0].LastPrice, response.Result.Price);
        Assert.Equal("USD", response.Result.CurrencyCode);
        Assert.Equal(instrumentSymbol, response.Result.Symbol);
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