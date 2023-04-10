using Microsoft.Extensions.DependencyInjection;
using PortEval.Application.Core.Interfaces.Queries;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Integration.QueryTests
{
    [Collection("Query test collection")]
    public class ExchangeQueriesTests
    {
        private readonly IExchangeQueries _exchangeQueries;

        public ExchangeQueriesTests(QueryTestFixture fixture)
        {
            var scope = fixture.Factory.Services.CreateScope();
            _exchangeQueries = scope.ServiceProvider.GetRequiredService<IExchangeQueries>();
        }

        [Fact]
        public async Task GetKnownExchanges_ReturnsExchangesFromDb()
        {
            var queryResult = await _exchangeQueries.GetKnownExchangesAsync();
            
            Assert.Collection(queryResult, e =>
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
