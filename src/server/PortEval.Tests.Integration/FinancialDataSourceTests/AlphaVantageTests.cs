using PortEval.Application.Models.FinancialDataFetcher;
using PortEval.DataFetcher;
using PortEval.DataFetcher.Models;
using PortEval.Infrastructure.FinancialDataFetcher.AlphaVantage;
using PortEval.Infrastructure.FinancialDataFetcher.AlphaVantage.Models;
using PortEval.Infrastructure.FinancialDataFetcher.Requests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Integration.FinancialDataSourceTests
{
    public class AlphaVantageTests
    {
        [Fact]
        public async Task ProcessHistoricalDailyInstrumentPricesRequest_ReturnsDataFromAlphaVantageDailyAdjustedEndpoint()
        {
            var priceTime = DateTime.Parse("2022-01-01T00:00:00Z").ToUniversalTime();
            var instrumentSymbol = "AAPL";

            var expectedRequestUri = @$"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY_ADJUSTED&symbol={instrumentSymbol}";
            var price = new TimeSeriesPriceDataModel
            {
                Price = 100m,
                SplitCoefficient = 1m
            };
            var apiMockResponse = new TimeSeriesDailyResponseModel()
            {
                Prices = new Dictionary<string, TimeSeriesPriceDataModel>
                {
                    [priceTime.ToString("yyyy-MM-dd")] = price
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
            sut.RegisterDataSource<AlphaVantageApi>(GetMockConfiguration());

            var response = await sut.ProcessRequestAsync<HistoricalDailyInstrumentPricesRequest, IEnumerable<PricePoint>>(request);

            Assert.Equal(StatusCode.Ok, response.StatusCode);
            Assert.Collection(response.Result, pricePoint =>
            {
                Assert.Equal(priceTime, pricePoint.Time);
                Assert.Equal(price.Price, pricePoint.Price);
                Assert.Equal("USD", pricePoint.CurrencyCode);
                Assert.Equal(instrumentSymbol, pricePoint.Symbol);
            });
        }

        [Fact]
        public async Task ProcessIntradayInstrumentPricesRequest_ReturnsDataFromAlphaVantageIntradayAdjustedEndpoint()
        {
            var priceTime = DateTime.Parse("2022-01-01T00:00:00Z").ToUniversalTime();
            var instrumentSymbol = "AAPL";

            var expectedRequestUri = @$"https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol={instrumentSymbol}&interval=60min&outputsize=full";
            var price = new TimeSeriesPriceDataModel
            {
                Price = 100m,
                SplitCoefficient = 1m
            };
            var apiMockResponse = new TimeSeriesIntradayResponseModel()
            {
                Prices = new Dictionary<string, TimeSeriesPriceDataModel>
                {
                    [priceTime.ToString("yyyy-MM-dd")] = price
                }
            };

            var httpClient = Helpers.SetupMockHttpClientReturningResponse(expectedRequestUri, apiMockResponse);

            var request = new IntradayInstrumentPricesRequest()
            {
                From = DateTime.Parse("2022-01-01"),
                To = DateTime.Parse("2022-01-03"),
                Symbol = instrumentSymbol,
                CurrencyCode = "USD",
                Interval = IntradayInterval.OneHour
            };

            var sut = new DataFetcher.DataFetcher(httpClient);
            sut.RegisterDataSource<AlphaVantageApi>(GetMockConfiguration());

            var response = await sut.ProcessRequestAsync<IntradayInstrumentPricesRequest, IEnumerable<PricePoint>>(request);

            Assert.Equal(StatusCode.Ok, response.StatusCode);
            Assert.Collection(response.Result, pricePoint =>
            {
                Assert.Equal(priceTime, pricePoint.Time);
                Assert.Equal(price.Price, pricePoint.Price);
                Assert.Equal("USD", pricePoint.CurrencyCode);
                Assert.Equal(instrumentSymbol, pricePoint.Symbol);
            });
        }

        [Fact]
        public async Task ProcessLatestInstrumentPriceRequest_ReturnsDataFromAlphaVantageQuoteEndpoint()
        {
            var price = new GlobalQuotePriceDataModel
            {
                Price = 100m
            };
            var apiMockResponse = new GlobalQuoteResponseModel
            {
                PriceData = price
            };

            var instrumentSymbol = "AAPL";

            var expectedRequestUri = @$"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={instrumentSymbol}";

            var httpClient = Helpers.SetupMockHttpClientReturningResponse(expectedRequestUri, apiMockResponse);

            var request = new LatestInstrumentPriceRequest
            {
                Symbol = instrumentSymbol,
                CurrencyCode = "USD"
            };

            var sut = new DataFetcher.DataFetcher(httpClient);
            sut.RegisterDataSource<AlphaVantageApi>(GetMockConfiguration());

            var response = await sut.ProcessRequestAsync<LatestInstrumentPriceRequest, PricePoint>(request);

            Assert.Equal(StatusCode.Ok, response.StatusCode);
            Assert.Equal(price.Price, response.Result.Price);
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
}
