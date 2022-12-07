﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortEval.FinancialDataFetcher.APIs.ExchangeRateHost;
using PortEval.FinancialDataFetcher.APIs.ExchangeRateHost.Models;
using PortEval.FinancialDataFetcher.Models;
using PortEval.FinancialDataFetcher.Requests;
using Xunit;

namespace PortEval.Tests.Integration.FinancialDataFetcherTests
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

            var sut = new ExchangeRateHostApi(httpClient);

            var response = await sut.Process(request);

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

            var sut = new ExchangeRateHostApi(httpClient);

            var response = await sut.Process(request);

            Assert.Equal(StatusCode.Ok, response.StatusCode);
            Assert.Equal(apiMockResponse.Base, response.Result.Currency);
            Assert.Equal(apiMockResponse.Rates["EUR"], response.Result.Rates["EUR"]);
        }
    }
}