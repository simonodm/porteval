using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PortEval.Application.Services.Queries;
using PortEval.Application.Services.Queries.Interfaces;
using Xunit;

namespace PortEval.Tests.Integration.QueryTests
{
    [Collection("Query test collection")]
    public class CurrencyExchangeRateQueriesTests
    {
        private readonly ICurrencyExchangeRateQueries _currencyExchangeRateQueries;

        public CurrencyExchangeRateQueriesTests(QueryTestFixture fixture)
        {
            var scope = fixture.Factory.Services.CreateScope();
            _currencyExchangeRateQueries = scope.ServiceProvider.GetRequiredService<ICurrencyExchangeRateQueries>();
        }

        [Fact]
        public async Task GetExchangeRates_ReturnsCorrectExchangeRatesFromDb()
        {
            var result = await _currencyExchangeRateQueries.GetExchangeRates("USD", DateTime.UtcNow);

            var rates = result.Response.ToList();

            Assert.Equal(QueryStatus.Ok, result.Status);
            Assert.Collection(rates,
                r =>
                {
                    Assert.Equal("USD", r.CurrencyFromCode);
                    Assert.Equal("CZK", r.CurrencyToCode);
                    Assert.Equal(25m, r.ExchangeRate);
                    Assert.Equal(DateTime.UtcNow, r.Time, TimeSpan.FromHours(1));
                },
                r =>
                {
                    Assert.Equal("USD", r.CurrencyFromCode);
                    Assert.Equal("EUR", r.CurrencyToCode);
                    Assert.Equal(1.01m, r.ExchangeRate);
                    Assert.Equal(DateTime.UtcNow, r.Time, TimeSpan.FromHours(1));
                }
            );
        }

        [Fact]
        public async Task GetExchangeRateAt_ReturnsCorrectExchangeRateAtSpecifiedTime()
        {
            var result =
                await _currencyExchangeRateQueries.GetExchangeRateAt("USD", "EUR",
                    DateTime.UtcNow.AddHours(1).AddDays(-1));

            var exchangeRate = result.Response;

            Assert.Equal(QueryStatus.Ok, result.Status);
            Assert.Equal("USD", exchangeRate.CurrencyFromCode);
            Assert.Equal("EUR", exchangeRate.CurrencyToCode);
            Assert.Equal(1.00m, exchangeRate.ExchangeRate);
            Assert.Equal(DateTime.UtcNow.AddDays(-1), exchangeRate.Time, TimeSpan.FromHours(2));
        }

        [Fact]
        public async Task GetExchangeRateAt_ReturnsNotFound_WhenBaseCurrencyDoesNotExist()
        {
            var result = await _currencyExchangeRateQueries.GetExchangeRateAt("AAA", "USD", DateTime.UtcNow);

            Assert.Equal(QueryStatus.NotFound, result.Status);
            Assert.Null(result.Response);
        }

        [Fact]
        public async Task GetExchangeRateAt_ReturnsNotFound_WhenTargetCurrencyDoesNotExist()
        {
            var result = await _currencyExchangeRateQueries.GetExchangeRateAt("USD", "AAA", DateTime.UtcNow);

            Assert.Equal(QueryStatus.NotFound, result.Status);
            Assert.Null(result.Response);
        }

        [Fact]
        public async Task GetExchangeRateAt_ReturnsNotFound_WhenNoExchangeRateExists()
        {
            var result = await _currencyExchangeRateQueries.GetExchangeRateAt("USD", "EUR", DateTime.UtcNow.AddMonths(-1));

            Assert.Equal(QueryStatus.NotFound, result.Status);
            Assert.Null(result.Response);
        }

        [Fact]
        public async Task Convert_ReturnsInverselyConvertedPrice_WhenOnlyInverseExchangeRateIsPresent()
        {
            var result = await _currencyExchangeRateQueries.Convert("EUR", "USD", 100, DateTime.UtcNow);

            Assert.Equal(100 * (1m / 1.01m), result);
        }

        [Fact]
        public async Task Convert_ReturnsConvertedPrice_WhenExactExchangeRateIsPresent()
        {
            var result = await _currencyExchangeRateQueries.Convert("USD", "EUR", 100, DateTime.UtcNow);

            Assert.Equal(101m, result);
        }

        [Fact]
        public async Task Convert_ReturnsIndirectlyConvertedPrice_WhenConvertingBetweenCurrenciesWithoutExchangeRate()
        {
            var result = await _currencyExchangeRateQueries.Convert("CZK", "EUR", 1000, DateTime.UtcNow);

            Assert.Equal(1000 * 1m / 25m * 1.01m, result);
        }
    }
}
