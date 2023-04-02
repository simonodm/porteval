using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.FinancialDataFetcher;
using PortEval.DataFetcher.Models;
using PortEval.Infrastructure.FinancialDataFetcher.ExchangeRateHost;
using PortEval.Infrastructure.FinancialDataFetcher.ExchangeRateHost.Models;
using PortEval.Infrastructure.FinancialDataFetcher.Requests;
using Xunit;

namespace PortEval.Tests.Integration.DataFetcherTests
{
    public class ExchangeRateHostTests
    {
        [Fact]
        public async Task ProcessHistoricalDailyExchangeRatesRequest_ReturnsDataFromERHTimeseriesEndpoint()
        {
            var expectedRequestUri = @"https://api.exchangerate.host/timeseries";
            var apiMockResponse = new ExchangeRatesTimeSeriesResponseModel
            {
                Base = "USD",
                Rates = new Dictionary<string, Dictionary<string, decimal>>
                {
                    {
                        "2022-01-02", new Dictionary<string, decimal>
                        {
                            { "EUR", 1m }
                        }
                    }
                }
            };

            var httpClient = Helpers.SetupMockHttpClientReturningResponse(expectedRequestUri, apiMockResponse);

            var request = new HistoricalDailyExchangeRatesRequest
            {
                CurrencyCode = "USD",
                From = DateTime.Parse("2022-01-01"),
                To = DateTime.Parse("2022-01-03")
            };

            var sut = new DataFetcher.DataFetcher(httpClient);
            sut.RegisterDataSource<ExchangeRateHostApi>();

            var response = await sut.ProcessRequest<HistoricalDailyExchangeRatesRequest, IEnumerable<ExchangeRates>>(request);

            Assert.Equal(StatusCode.Ok, response.StatusCode);
            Assert.Collection(response.Result, er =>
            {
                Assert.Equal(apiMockResponse.Base, er.Currency);
                Assert.Equal(apiMockResponse.Rates["2022-01-02"]["EUR"], er.Rates["EUR"]);
                Assert.Equal(DateTime.Parse("2022-01-02"), er.Time);
            });
        }

        [Fact]
        public async Task ProcessLatestExchangeRatesRequest_ReturnsDataFromERHLatestEndpoint()
        {
            var expectedRequestUri = @"https://api.exchangerate.host/latest";
            var apiMockResponse = new ExchangeRatesLatestResponseModel
            {
                Base = "USD",
                Rates = new Dictionary<string, decimal>
                {
                    { "EUR", 1m }
                }
            };

            var httpClient = Helpers.SetupMockHttpClientReturningResponse(expectedRequestUri, apiMockResponse);

            var request = new LatestExchangeRatesRequest
            {
                CurrencyCode = "USD"
            };

            var sut = new DataFetcher.DataFetcher(httpClient);
            sut.RegisterDataSource<ExchangeRateHostApi>();

            var response = await sut.ProcessRequest<LatestExchangeRatesRequest, ExchangeRates>(request);

            Assert.Equal(StatusCode.Ok, response.StatusCode);
            Assert.Equal(apiMockResponse.Base, response.Result.Currency);
            Assert.Equal(apiMockResponse.Rates["EUR"], response.Result.Rates["EUR"]);
        }
    }
}
