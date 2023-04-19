using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PortEval.Application.Core.Interfaces.Queries;
using Xunit;

namespace PortEval.Tests.Integration.QueryTests;

[Collection("Query test collection")]
public class CurrencyQueriesTests
{
    private readonly ICurrencyQueries _currencyQueries;

    public CurrencyQueriesTests(QueryTestFixture fixture)
    {
        var scope = fixture.Factory.Services.CreateScope();
        _currencyQueries = scope.ServiceProvider.GetService<ICurrencyQueries>();
    }

    [Fact]
    public async Task GetAllCurrenciesAsync_ReturnsKeyCurrenciesFromDb()
    {
        var queryResult = await _currencyQueries.GetAllCurrenciesAsync();

        Assert.Contains(queryResult, c => c.Code == "USD" && c.Name == "United States dollar" && c.Symbol == "US$");
        Assert.Contains(queryResult, c => c.Code == "EUR" && c.Name == "European Euro" && c.Symbol == "€");
        Assert.Contains(queryResult, c => c.Code == "CZK" && c.Name == "Czech koruna" && c.Symbol == "Kč");
    }

    [Fact]
    public async Task GetCurrencyAsync_ReturnsCurrencyFromDb()
    {
        var queryResult = await _currencyQueries.GetCurrencyAsync("USD");

        Assert.Equal("USD", queryResult.Code);
        Assert.Equal("United States dollar", queryResult.Name);
        Assert.Equal("US$", queryResult.Symbol);
        Assert.True(queryResult.IsDefault);
    }

    [Fact]
    public async Task GetCurrencyAsync_ReturnsNull_WhenCurrencyDoesNotExist()
    {
        var queryResult = await _currencyQueries.GetCurrencyAsync("AAA");

        Assert.Null(queryResult);
    }

    [Fact]
    public async Task GetExchangeRates_ReturnsCorrectExchangeRatesFromDb()
    {
        var result = await _currencyQueries.GetDirectExchangeRatesAsync("USD", DateTime.UtcNow);

        var rates = result.ToList();

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
            await _currencyQueries.GetCurrencyExchangeRateAsync("USD", "EUR",
                DateTime.UtcNow.AddHours(1).AddDays(-1));

        Assert.Equal("USD", result.CurrencyFromCode);
        Assert.Equal("EUR", result.CurrencyToCode);
        Assert.Equal(1.00m, result.ExchangeRate);
        Assert.Equal(DateTime.UtcNow.AddDays(-1), result.Time, TimeSpan.FromHours(2));
    }

    [Fact]
    public async Task GetExchangeRateAt_ReturnsNull_WhenCurrencyIsNotFound()
    {
        var result = await _currencyQueries.GetCurrencyExchangeRateAsync("AAA", "USD", DateTime.UtcNow);

        Assert.Null(result);
    }
}