using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Queries;
using Xunit;
using PortEval.Application.Core;

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
            var queryResult = await _exchangeQueries.GetKnownExchanges();

            Assert.Equal(OperationStatus.Ok, queryResult.Status);
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
