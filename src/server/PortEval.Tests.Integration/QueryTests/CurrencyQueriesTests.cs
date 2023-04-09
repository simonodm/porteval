using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Queries;
using Xunit;
using PortEval.Application.Core;

namespace PortEval.Tests.Integration.QueryTests
{
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
        public async Task GetAllCurrencies_ReturnsKeyCurrenciesFromDb()
        {
            var queryResult = await _currencyQueries.GetAllCurrencies();

            Assert.Equal(OperationStatus.Ok, queryResult.Status);
            Assert.Contains(queryResult.Response, c => c.Code == "USD" && c.Name == "United States dollar" && c.Symbol == "US$");
            Assert.Contains(queryResult.Response, c => c.Code == "EUR" && c.Name == "European Euro" && c.Symbol == "€");
            Assert.Contains(queryResult.Response, c => c.Code == "CZK" && c.Name == "Czech koruna" && c.Symbol == "Kč");
        }

        [Fact]
        public async Task GetCurrency_ReturnsCurrencyFromDb()
        {
            var queryResult = await _currencyQueries.GetCurrency("USD");

            Assert.Equal(OperationStatus.Ok, queryResult.Status);
            Assert.Equal("USD", queryResult.Response.Code);
            Assert.Equal("United States dollar", queryResult.Response.Name);
            Assert.Equal("US$", queryResult.Response.Symbol);
            Assert.True(queryResult.Response.IsDefault);
        }

        [Fact]
        public async Task GetCurrency_ReturnsNotFound_WhenCurrencyDoesNotExist()
        {
            var queryResult = await _currencyQueries.GetCurrency("AAA");

            Assert.Equal(OperationStatus.NotFound, queryResult.Status);
            Assert.Null(queryResult.Response);
        }
    }
}
