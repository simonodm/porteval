﻿using PortEval.Domain.Models.Enums;
using PortEval.FinancialDataFetcher.APIs.Tiingo;
using PortEval.FinancialDataFetcher.APIs.Tiingo.Models;
using PortEval.FinancialDataFetcher.Models;
using PortEval.FinancialDataFetcher.Requests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Integration.FinancialDataFetcherTests
{
    public class TiingoTests
    {
        [Fact]
        public async Task ProcessHistoricalDailyInstrumentPricesRequest_ReturnsDataFromTiingoEODEndpoint()
        {
            var priceTime = DateTime.Parse("2022-01-01T12:00:00Z").ToUniversalTime();
            var instrumentSymbol = "AAPL";
            var instrumentType = InstrumentType.Stock;

            var expectedRequestUri = @"https://api.tiingo.com/tiingo/daily/AAPL/prices";
            var apiMockResponse = new List<TiingoPriceResponseModel>
            {
                new TiingoPriceResponseModel
                {
                    Time = priceTime,
                    Price = 100m
                }
            };

            var httpClient = Helpers.SetupMockHttpClientReturningResponse(expectedRequestUri, apiMockResponse);

            var request = new HistoricalDailyInstrumentPricesRequest
            {
                CurrencyCode = "USD",
                From = DateTime.Parse("2022-01-01"),
                To = DateTime.Parse("2022-01-03"),
                Symbol = instrumentSymbol,
                Type = instrumentType
            };

            var sut = new TiingoApi(httpClient, "");

            var response = await sut.Process(request);

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
        public async Task ProcessHistoricalDailyInstrumentPricesRequest_ReturnsDataFromTiingoCryptoHistoricalEndpoint_WhenInstrumentTypeIsCrypto()
        {
            var priceTime = DateTime.Parse("2022-01-02T12:00:00Z").ToUniversalTime();
            var instrumentSymbol = "BTC";
            var instrumentType = InstrumentType.CryptoCurrency;

            var expectedRequestUri = @"https://api.tiingo.com/tiingo/crypto/prices";
            var apiMockResponse = new List<TiingoCryptoPriceResponseModel>
            {
                new TiingoCryptoPriceResponseModel
                {
                    Ticker = "BTCUSD",
                    BaseCurrency = "BTC",
                    QuoteCurrency = "USD",
                    Data = new List<TiingoPriceResponseModel>
                    {
                        new TiingoPriceResponseModel
                        {
                            Time = priceTime,
                            Price = 100m
                        }
                    }
                }
            };

            var httpClient = Helpers.SetupMockHttpClientReturningResponse(expectedRequestUri, apiMockResponse);

            var request = new HistoricalDailyInstrumentPricesRequest
            {
                CurrencyCode = "USD",
                From = DateTime.Parse("2022-01-01"),
                To = DateTime.Parse("2022-01-03"),
                Symbol = instrumentSymbol,
                Type = instrumentType
            };

            var sut = new TiingoApi(httpClient, "");

            var response = await sut.Process(request);

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
        public async Task ProcessIntradayPricesRequest_ReturnsDataFromTiingoIEXEndpoint()
        {
            var priceTime = DateTime.Parse("2022-01-01T12:00:00Z").ToUniversalTime();

            var instrumentSymbol = "AAPL";
            var instrumentType = InstrumentType.Stock;

            var expectedRequestUri = @"https://api.tiingo.com/iex/AAPL/prices";
            var apiMockResponse = new List<TiingoPriceResponseModel>
            {
                new TiingoPriceResponseModel
                {
                    Time = priceTime,
                    Price = 100m
                }
            };

            var httpClient = Helpers.SetupMockHttpClientReturningResponse(expectedRequestUri, apiMockResponse);

            var request = new IntradayPricesRequest
            {
                CurrencyCode = "USD",
                From = DateTime.Parse("2022-01-01"),
                To = DateTime.Parse("2022-01-03"),
                Symbol = instrumentSymbol,
                Interval = IntradayInterval.OneHour,
                Type = instrumentType
            };

            var sut = new TiingoApi(httpClient, "");

            var response = await sut.Process(request);

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
        public async Task ProcessIntradayPricesRequest_ReturnsDataFromTiingoCryptoHistoricalEndpoint_WhenInstrumentTypeIsCrypto()
        {
            var priceTime = DateTime.Parse("2022-01-02T12:00:00Z").ToUniversalTime();
            var instrumentSymbol = "BTC";
            var instrumentType = InstrumentType.CryptoCurrency;

            var expectedRequestUri = @"https://api.tiingo.com/tiingo/crypto/prices";
            var apiMockResponse = new List<TiingoCryptoPriceResponseModel>
            {
                new TiingoCryptoPriceResponseModel
                {
                    Ticker = "BTCUSD",
                    BaseCurrency = "BTC",
                    QuoteCurrency = "USD",
                    Data = new List<TiingoPriceResponseModel>
                    {
                        new TiingoPriceResponseModel
                        {
                            Time = priceTime,
                            Price = 100m
                        }
                    }
                }
            };

            var httpClient = Helpers.SetupMockHttpClientReturningResponse(expectedRequestUri, apiMockResponse);

            var request = new IntradayPricesRequest
            {
                CurrencyCode = "USD",
                From = DateTime.Parse("2022-01-01"),
                To = DateTime.Parse("2022-01-03"),
                Symbol = instrumentSymbol,
                Type = instrumentType,
                Interval = IntradayInterval.OneHour
            };

            var sut = new TiingoApi(httpClient, "");

            var response = await sut.Process(request);

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
                new TiingoIexTopPriceResponseModel
                {
                    Price = 100m
                }
            };

            var instrumentSymbol = "AAPL";
            var instrumentType = InstrumentType.Stock;

            var client = Helpers.SetupMockHttpClientReturningResponse(expectedRequestUri, apiMockResponse);

            var request = new LatestInstrumentPriceRequest
            {
                CurrencyCode = "USD",
                Symbol = instrumentSymbol,
                Type = instrumentType
            };

            var sut = new TiingoApi(client, "");

            var response = await sut.Process(request);

            Assert.Equal(StatusCode.Ok, response.StatusCode);
            Assert.Equal(apiMockResponse[0].Price, response.Result.Price);
            Assert.Equal("USD", response.Result.CurrencyCode);
            Assert.Equal(instrumentSymbol, response.Result.Symbol);
        }

        [Fact]
        public async Task ProcessLatestInstrumentPriceRequest_ReturnsDataFromTiingoCryptoTopOfBookEndpoint_WhenInstrumentTypeIsCrypto()
        {
            var priceTime = DateTime.Parse("2022-01-02T12:00:00Z").ToUniversalTime();
            var instrumentSymbol = "BTC";
            var instrumentType = InstrumentType.CryptoCurrency;

            var expectedRequestUri = @"https://api.tiingo.com/tiingo/crypto/top";
            var apiMockResponse = new List<TiingoCryptoTopPriceResponseModel>
            {
                new TiingoCryptoTopPriceResponseModel
                {
                    Ticker = "BTCUSD",
                    BaseCurrency = "BTC",
                    QuoteCurrency = "USD",
                    Data = new List<TiingoCryptoTopPriceDataResponseModel>
                    {
                        new TiingoCryptoTopPriceDataResponseModel
                        {
                            LastPrice = 100m,
                            AskPrice = 100m,
                            BidPrice = 100m
                        }
                    }
                }
            };

            var httpClient = Helpers.SetupMockHttpClientReturningResponse(expectedRequestUri, apiMockResponse);

            var request = new LatestInstrumentPriceRequest
            {
                CurrencyCode = "USD",
                Symbol = instrumentSymbol,
                Type = instrumentType,
            };

            var sut = new TiingoApi(httpClient, "");

            var response = await sut.Process(request);

            Assert.Equal(StatusCode.Ok, response.StatusCode);
            Assert.Equal(apiMockResponse[0].Data[0].LastPrice, response.Result.Price);
            Assert.Equal("USD", response.Result.CurrencyCode);
            Assert.Equal(instrumentSymbol, response.Result.Symbol);
        }
    }
}
