using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PortEval.Application.Services.Queries;
using PortEval.Application.Services.Queries.Interfaces;
using Xunit;

namespace PortEval.Tests.Functional.QueryTests
{
    [Collection("Integration test collection")]
    public class ExchangeQueriesTests
    {
        private readonly IExchangeQueries _exchangeQueries;

        public ExchangeQueriesTests(IntegrationTestFixture fixture)
        {
            var scope = fixture.Factory.Services.CreateScope();
            _exchangeQueries = scope.ServiceProvider.GetRequiredService<IExchangeQueries>();
        }

        [Fact]
        public async Task GetKnownExchanges_ReturnsExchangesFromDb()
        {
            var queryResult = await _exchangeQueries.GetKnownExchanges();

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            Assert.Collection(queryResult.Response, e =>
            {
                Assert.Equal("NASDAQ", e.Symbol);
                Assert.Equal("NASDAQ", e.Name);
            }, e =>
            {
                Assert.Equal("NYSE", e.Symbol);
                Assert.Equal("NYSE", e.Name);
            });
        }
    }
}
