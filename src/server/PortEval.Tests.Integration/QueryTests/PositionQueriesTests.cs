using Microsoft.Extensions.DependencyInjection;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Models.DTOs;
using PortEval.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Integration.QueryTests
{
    [Collection("Query test collection")]
    public class PositionQueriesTests
    {
        private readonly IPositionQueries _positionQueries;

        private readonly int _firstPortfolioId;
        private readonly int _secondPortfolioId;
        private readonly int _appleInstrumentId;
        private readonly int _applePositionId;
        private readonly int _btcInstrumentId;
        private readonly int _btcPositionId;

        public PositionQueriesTests(QueryTestFixture fixture)
        {
            using var scope = fixture.Factory.Services.CreateScope();
            _positionQueries = scope.ServiceProvider.GetRequiredService<IPositionQueries>();
            var context = scope.ServiceProvider.GetRequiredService<PortEvalDbContext>();

            _firstPortfolioId = context.Portfolios.First(p => p.Name == "Portfolio 1").Id;
            _secondPortfolioId = context.Portfolios.First(p => p.Name == "Portfolio 2").Id;

            _appleInstrumentId = context.Instruments.First(i => i.Symbol == "AAPL").Id;
            _applePositionId = context.Positions.First(p => p.InstrumentId == _appleInstrumentId).Id;

            _btcInstrumentId = context.Instruments.First(i => i.Symbol == "BTC").Id;
            _btcPositionId = context.Positions.First(p => p.InstrumentId == _btcInstrumentId).Id;
        }

        [Fact]
        public async Task GetAllPositions_ReturnsAllPositions()
        {
            var queryResult = await _positionQueries.GetAllPositionsAsync();

            Assert.Collection(queryResult, AssertIsAAPLPosition, AssertIsBTCPosition);
        }

        [Fact]
        public async Task GetPortfolioPositions_ReturnsOnlyPortfolioPositions()
        {
            var queryResult = await _positionQueries.GetPortfolioPositionsAsync(_firstPortfolioId);

            Assert.Collection(queryResult, AssertIsAAPLPosition);
        }

        [Fact]
        public async Task GetPosition_ReturnsCorrectPosition()
        {
            var queryResult = await _positionQueries.GetPositionAsync(_btcPositionId);

            AssertIsBTCPosition(queryResult);
        }

        [Fact]
        public async Task GetPosition_ReturnsNull_WhenPositionDoesNotExist()
        {
            var queryResult = await _positionQueries.GetPositionAsync(-1);

            Assert.Null(queryResult);
        }

        private void AssertIsAAPLPosition(PositionDto p)
        {
            Assert.Equal(_applePositionId, p.Id);
            Assert.Equal(_firstPortfolioId, p.PortfolioId);
            Assert.Equal(_appleInstrumentId, p.InstrumentId);
            Assert.Equal(1, p.PositionSize);
            Assert.Equal("", p.Note);
        }

        private void AssertIsBTCPosition(PositionDto p)
        {
            Assert.Equal(_btcPositionId, p.Id);
            Assert.Equal(_secondPortfolioId, p.PortfolioId);
            Assert.Equal(_btcInstrumentId, p.InstrumentId);
            Assert.Equal(5, p.PositionSize);
            Assert.Equal("bitcoin", p.Note);
        }
    }
}
