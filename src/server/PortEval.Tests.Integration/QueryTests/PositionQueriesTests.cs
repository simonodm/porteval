using Microsoft.Extensions.DependencyInjection;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain;
using PortEval.Domain.Models.Enums;
using PortEval.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Queries;
using Xunit;
using PortEval.Application.Core;

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
            var queryResult = await _positionQueries.GetAllPositions();

            Assert.Equal(OperationStatus.Ok, queryResult.Status);
            Assert.Collection(queryResult.Response, AssertIsAAPLPosition, AssertIsBTCPosition);
        }

        [Fact]
        public async Task GetPortfolioPositions_ReturnsOnlyPortfolioPositions()
        {
            var queryResult = await _positionQueries.GetPortfolioPositions(_firstPortfolioId);

            Assert.Equal(OperationStatus.Ok, queryResult.Status);
            Assert.Collection(queryResult.Response, AssertIsAAPLPosition);
        }

        [Fact]
        public async Task GetPortfolioPositions_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var queryResult = await _positionQueries.GetPortfolioPositions(-1);

            Assert.Equal(OperationStatus.NotFound, queryResult.Status);
        }

        [Fact]
        public async Task GetPosition_ReturnsCorrectPosition()
        {
            var queryResult = await _positionQueries.GetPosition(_btcPositionId);

            Assert.Equal(OperationStatus.Ok, queryResult.Status);
            AssertIsBTCPosition(queryResult.Response);
        }

        [Fact]
        public async Task GetPosition_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var queryResult = await _positionQueries.GetPosition(-1);

            Assert.Equal(OperationStatus.NotFound, queryResult.Status);
        }

        [Fact]
        public async Task GetPositionValue_ReturnsCorrectPositionValue()
        {
            var time = DateTime.UtcNow;

            var queryResult = await _positionQueries.GetPositionValue(_applePositionId, time);

            Assert.Equal(OperationStatus.Ok, queryResult.Status);
            Assert.Equal(150m, queryResult.Response.Value);
            Assert.Equal("USD", queryResult.Response.CurrencyCode);
            Assert.Equal(time, queryResult.Response.Time);
        }

        [Fact]
        public async Task GetPositionValue_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var queryResult = await _positionQueries.GetPositionValue(-1, DateTime.UtcNow);

            Assert.Equal(OperationStatus.NotFound, queryResult.Status);
        }

        [Fact]
        public async Task GetPositionProfit_ReturnsCorrectPositionProfit()
        {
            var queryResult = await _positionQueries.GetPositionProfit(_applePositionId, new DateRangeParams());

            Assert.Equal(OperationStatus.Ok, queryResult.Status);
            Assert.Equal(50m, queryResult.Response.Profit);
            Assert.Equal("USD", queryResult.Response.CurrencyCode);
            Assert.Equal(PortEvalConstants.FinancialDataStartTime, queryResult.Response.From);
            Assert.Equal(DateTime.UtcNow, queryResult.Response.To, TimeSpan.FromMinutes(1));
        }

        [Fact]
        public async Task GetPositionProfit_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var queryResult = await _positionQueries.GetPositionProfit(-1, new DateRangeParams());

            Assert.Equal(OperationStatus.NotFound, queryResult.Status);
        }

        [Fact]
        public async Task GetPositionPerformance_ReturnsCorrectPositionPerformance()
        {
            var queryResult = await _positionQueries.GetPositionPerformance(_applePositionId, new DateRangeParams());

            Assert.Equal(OperationStatus.Ok, queryResult.Status);
            Assert.Equal(0.5m, queryResult.Response.Performance);
            Assert.Equal(PortEvalConstants.FinancialDataStartTime, queryResult.Response.From);
            Assert.Equal(DateTime.UtcNow, queryResult.Response.To, TimeSpan.FromMinutes(1));
        }

        [Fact]
        public async Task GetPositionPerformance_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var queryResult = await _positionQueries.GetPositionPerformance(-1, new DateRangeParams());

            Assert.Equal(OperationStatus.NotFound, queryResult.Status);
        }

        [Fact]
        public async Task GetPositionBEP_ReturnsCorrectPositionBEP()
        {
            var time = DateTime.UtcNow;
            var queryResult = await _positionQueries.GetPositionBreakEvenPoint(_applePositionId, time);

            Assert.Equal(OperationStatus.Ok, queryResult.Status);
            Assert.Equal(100m, queryResult.Response.BreakEvenPoint);
            Assert.Equal(time, queryResult.Response.Time);
        }

        [Fact]
        public async Task GetPositionBEP_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var queryResult = await _positionQueries.GetPositionBreakEvenPoint(-1, DateTime.UtcNow);

            Assert.Equal(OperationStatus.NotFound, queryResult.Status);
        }

        [Fact]
        public async Task ChartPositionValue_ReturnsChartedPositionValue()
        {
            var queryResult = await _positionQueries.ChartPositionValue(_applePositionId, new DateRangeParams(),
                AggregationFrequency.Day);

            Assert.Equal(OperationStatus.Ok, queryResult.Status);
            Assert.Collection(queryResult.Response, p =>
            {
                Assert.Equal(130m, p.Value);
                Assert.Equal(DateTime.UtcNow.AddDays(-2), p.Time, TimeSpan.FromHours(1));
            }, p =>
            {
                Assert.Equal(130m, p.Value);
                Assert.Equal(DateTime.UtcNow.Date.AddDays(-1), p.Time, TimeSpan.FromHours(1));
            }, p =>
            {
                Assert.Equal(140m, p.Value);
                Assert.Equal(DateTime.UtcNow.Date, p.Time, TimeSpan.FromHours(1));
            }, p =>
            {
                Assert.Equal(150m, p.Value);
                Assert.Equal(DateTime.UtcNow, p.Time, TimeSpan.FromHours(1));
            });
        }

        [Fact]
        public async Task ChartPositionValue_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var queryResult = await _positionQueries.ChartPositionValue(-1, new DateRangeParams(), AggregationFrequency.Day);

            Assert.Equal(OperationStatus.NotFound, queryResult.Status);
        }

        [Fact]
        public async Task ChartPositionProfit_ReturnsChartedPositionProfit()
        {
            var queryResult = await _positionQueries.ChartPositionProfit(_applePositionId, new DateRangeParams(),
                AggregationFrequency.Day);

            Assert.Equal(OperationStatus.Ok, queryResult.Status);
            Assert.Collection(queryResult.Response, p =>
            {
                Assert.Equal(0m, p.Value);
                Assert.Equal(DateTime.UtcNow.AddDays(-2), p.Time, TimeSpan.FromHours(1));
            }, p =>
            {
                Assert.Equal(30m, p.Value);
                Assert.Equal(DateTime.UtcNow.Date.AddDays(-1), p.Time, TimeSpan.FromHours(1));
            }, p =>
            {
                Assert.Equal(40m, p.Value);
                Assert.Equal(DateTime.UtcNow.Date, p.Time, TimeSpan.FromHours(1));
            }, p =>
            {
                Assert.Equal(50m, p.Value);
                Assert.Equal(DateTime.UtcNow, p.Time, TimeSpan.FromHours(1));
            });
        }

        [Fact]
        public async Task ChartPositionProfit_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var queryResult = await _positionQueries.ChartPositionProfit(-1, new DateRangeParams(), AggregationFrequency.Day);

            Assert.Equal(OperationStatus.NotFound, queryResult.Status);
        }

        [Fact]
        public async Task ChartPositionPerformance_ReturnsChartedPositionPerformance()
        {
            var queryResult = await _positionQueries.ChartPositionPerformance(_applePositionId, new DateRangeParams(),
                AggregationFrequency.Day);

            Assert.Equal(OperationStatus.Ok, queryResult.Status);
            Assert.Collection(queryResult.Response, p =>
            {
                Assert.Equal(0m, p.Value, 2);
                Assert.Equal(DateTime.UtcNow.AddDays(-2), p.Time, TimeSpan.FromHours(1));
            }, p =>
            {
                Assert.Equal(0.3m, p.Value, 2);
                Assert.Equal(DateTime.UtcNow.Date.AddDays(-1), p.Time, TimeSpan.FromHours(1));
            }, p =>
            {
                Assert.Equal(0.4m, p.Value, 2);
                Assert.Equal(DateTime.UtcNow.Date, p.Time, TimeSpan.FromHours(1));
            }, p =>
            {
                Assert.Equal(0.5m, p.Value, 2);
                Assert.Equal(DateTime.UtcNow, p.Time, TimeSpan.FromHours(1));
            });
        }

        [Fact]
        public async Task ChartPositionPerformance_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var queryResult = await _positionQueries.ChartPositionPerformance(-1, new DateRangeParams(), AggregationFrequency.Day);

            Assert.Equal(OperationStatus.NotFound, queryResult.Status);
        }

        [Fact]
        public async Task ChartPositionProfitAggregated_ReturnsChartedPositionAggregatedProfit()
        {
            var queryResult = await _positionQueries.ChartPositionProfitAggregated(_applePositionId, new DateRangeParams(),
                AggregationFrequency.Day);

            Assert.Equal(OperationStatus.Ok, queryResult.Status);
            Assert.Collection(queryResult.Response, p =>
            {
                Assert.Equal(30m, p.Value);
                Assert.Equal(DateTime.UtcNow.Date.AddDays(-1), p.Time, TimeSpan.FromHours(1));
            }, p =>
            {
                Assert.Equal(10m, p.Value);
                Assert.Equal(DateTime.UtcNow.Date, p.Time, TimeSpan.FromHours(1));
            }, p =>
            {
                Assert.Equal(10m, p.Value);
                Assert.Equal(DateTime.UtcNow, p.Time, TimeSpan.FromHours(1));
            });
        }

        [Fact]
        public async Task ChartPositionProfitAggregated_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var queryResult = await _positionQueries.ChartPositionProfitAggregated(-1, new DateRangeParams(), AggregationFrequency.Day);

            Assert.Equal(OperationStatus.NotFound, queryResult.Status);
        }

        [Fact]
        public async Task ChartPositionPerformanceAggregated_ReturnsChartedPositionAggregatedPerformance()
        {
            var queryResult = await _positionQueries.ChartPositionPerformanceAggregated(_applePositionId, new DateRangeParams(),
                AggregationFrequency.Day);

            Assert.Equal(OperationStatus.Ok, queryResult.Status);
            Assert.Collection(queryResult.Response, p =>
            {
                Assert.Equal(0.3m, p.Value);
                Assert.Equal(DateTime.UtcNow.Date.AddDays(-1), p.Time, TimeSpan.FromHours(1));
            }, p =>
            {
                var expectedPerformance = 0.0769m;
                Assert.InRange(p.Value, expectedPerformance - 0.01m, expectedPerformance + 0.01m);
                Assert.Equal(DateTime.UtcNow.Date, p.Time, TimeSpan.FromHours(1));
            }, p =>
            {
                var expectedPerformance = 0.0714m;
                Assert.InRange(p.Value, expectedPerformance - 0.01m, expectedPerformance + 0.01m);
                Assert.Equal(DateTime.UtcNow, p.Time, TimeSpan.FromHours(1));
            });
        }

        [Fact]
        public async Task ChartPositionPerformanceAggregated_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var queryResult = await _positionQueries.ChartPositionPerformanceAggregated(-1, new DateRangeParams(), AggregationFrequency.Day);

            Assert.Equal(OperationStatus.NotFound, queryResult.Status);
        }

        [Fact]
        public async Task GetPortfolioPositionStatistics_ReturnsCorrectPortfolioPositionStatistics()
        {
            var queryResult = await _positionQueries.GetPortfolioPositionsStatistics(_firstPortfolioId);

            Assert.Equal(OperationStatus.Ok, queryResult.Status);
            Assert.Collection(queryResult.Response, AssertIsAAPLPerformanceStatistics);
        }

        [Fact]
        public async Task GetPositionStatistics_ReturnsCorrectPositionStatistics()
        {
            var queryResult = await _positionQueries.GetPositionStatistics(_applePositionId);

            Assert.Equal(OperationStatus.Ok, queryResult.Status);
            AssertIsAAPLPerformanceStatistics(queryResult.Response);
        }

        [Fact]
        public async Task GetPositionStatistics_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var queryResult = await _positionQueries.GetPositionStatistics(-1);

            Assert.Equal(OperationStatus.NotFound, queryResult.Status);
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

        private void AssertIsAAPLPerformanceStatistics(PositionStatisticsDto s)
        {
            var expectedPerformance = 0.5m;
            var expectedLastDayPerformance = 0.0714m;

            var expectedProfit = 50m;
            var expectedLastDayProfit = 10m;

            var expectedBep = 100m;

            Assert.Equal(expectedPerformance, s.TotalPerformance);
            Assert.Equal(expectedPerformance, s.LastMonthPerformance);
            Assert.Equal(expectedPerformance, s.LastWeekPerformance);
            Assert.InRange(s.LastDayPerformance, expectedLastDayPerformance - 0.01m, expectedLastDayPerformance + 0.01m);

            Assert.Equal(expectedProfit, s.TotalProfit);
            Assert.Equal(expectedProfit, s.LastMonthProfit);
            Assert.Equal(expectedProfit, s.LastWeekProfit);
            Assert.Equal(expectedLastDayProfit, s.LastDayProfit);

            Assert.Equal(expectedBep, s.BreakEvenPoint);
        }
    }
}
