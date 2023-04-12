using Microsoft.Extensions.DependencyInjection;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.DTOs.Enums;
using PortEval.Domain.Models.Enums;
using PortEval.Infrastructure;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Integration.QueryTests
{
    [Collection("Query test collection")]
    public class ChartQueriesTests
    {
        private readonly IChartQueries _chartQueries;

        private readonly int _portfolioChartId;
        private readonly int _positionInstrumentChartId;
        private readonly int _firstPortfolioId;
        private readonly int _appleInstrumentId;
        private readonly int _applePositionId;

        public ChartQueriesTests(QueryTestFixture fixture)
        {
            var scope = fixture.Factory.Services.CreateScope();
            _chartQueries = scope.ServiceProvider.GetRequiredService<IChartQueries>();
            var context = scope.ServiceProvider.GetRequiredService<PortEvalDbContext>();

            _portfolioChartId = context.Charts
                .Where(c => c.TypeConfiguration.Type == ChartType.Price)
                .Select(c => c.Id)
                .FirstOrDefault();
            _positionInstrumentChartId = context.Charts
                .Where(c => c.TypeConfiguration.Type == ChartType.AggregatedPerformance)
                .Select(c => c.Id)
                .FirstOrDefault();
            _firstPortfolioId = context.Portfolios
                .Where(p => p.Name == "Portfolio 1")
                .Select(p => p.Id)
                .FirstOrDefault();
            _appleInstrumentId = context.Instruments
                .Where(i => i.Symbol == "AAPL")
                .Select(i => i.Id)
                .FirstOrDefault();
            _applePositionId = context.Positions
                .Where(p => p.InstrumentId == _appleInstrumentId)
                .Select(p => p.Id)
                .FirstOrDefault();
        }

        [Fact]
        public async Task GetCharts_ReturnsAllChartsFromDb()
        {
            var queryResult = await _chartQueries.GetChartsAsync();

            Assert.Collection(queryResult, AssertIsTestPortfolioChart, AssertIsTestPositionInstrumentChart);
        }

        [Fact]
        public async Task GetChart_ReturnsCorrectChart_WhenChartExists()
        {
            var queryResult = await _chartQueries.GetChartAsync(_portfolioChartId);

            AssertIsTestPortfolioChart(queryResult);
        }

        [Fact]
        public async Task GetChart_ReturnsNull_WhenChartDoesNotExist()
        {
            var queryResult = await _chartQueries.GetChartAsync(-1);

            Assert.Null(queryResult);
        }

        private void AssertIsTestPortfolioChart(ChartDto c)
        {
            Assert.Equal(_portfolioChartId, c.Id);
            Assert.Equal("Portfolio chart", c.Name);
            Assert.Equal(ChartType.Price, c.Type);
            Assert.Equal("USD", c.CurrencyCode);
            Assert.True(c.IsToDate);
            Assert.Equal(DateRangeUnit.MONTH, c.ToDateRange.Unit);
            Assert.Equal(1, c.ToDateRange.Value);
            Assert.Null(c.DateRangeStart);
            Assert.Null(c.DateRangeEnd);
            Assert.Null(c.Frequency);
            Assert.Collection(c.Lines, line =>
            {
                Assert.Equal(ChartLineType.Portfolio, line.Type);
                Assert.Equal(_firstPortfolioId, line.PortfolioId);
                Assert.NotEqual(0, line.PortfolioId);
                Assert.Equal("Portfolio 1", line.Name);
                Assert.Equal(1, line.Width);
                Assert.Equal(LineDashType.Solid, line.Dash);
                Assert.Equal(Color.Red.ToArgb(), line.Color.ToArgb());
                Assert.Null(line.InstrumentId);
                Assert.Null(line.PositionId);
            });
        }

        private void AssertIsTestPositionInstrumentChart(ChartDto c)
        {
            Assert.Equal(_positionInstrumentChartId, c.Id);
            Assert.Equal("Position/instrument chart", c.Name);
            Assert.Equal(ChartType.AggregatedPerformance, c.Type);
            Assert.Equal(AggregationFrequency.Day, c.Frequency);
            Assert.True(c.IsToDate == null || !(bool)c.IsToDate);
            Assert.Equal(DateTime.Parse("2022-01-01"), c.DateRangeStart);
            Assert.Equal(DateTime.Parse("2022-01-15"), c.DateRangeEnd);
            Assert.Null(c.ToDateRange);
            Assert.Null(c.CurrencyCode);
            Assert.Collection(c.Lines, line =>
            {
                Assert.Equal(ChartLineType.Instrument, line.Type);
                Assert.Equal(_appleInstrumentId, line.InstrumentId);
                Assert.NotEqual(0, line.InstrumentId);
                Assert.Equal("Apple Inc.", line.Name);
                Assert.Equal(2, line.Width);
                Assert.Equal(LineDashType.Dotted, line.Dash);
                Assert.Equal(Color.Cyan.ToArgb(), line.Color.ToArgb());
                Assert.Null(line.PortfolioId);
                Assert.Null(line.PositionId);
            }, line =>
            {
                Assert.Equal(ChartLineType.Position, line.Type);
                Assert.Equal(_applePositionId, line.PositionId);
                Assert.NotEqual(0, line.PositionId);
                Assert.Equal("Apple Inc.", line.Name);
                Assert.Equal(1, line.Width);
                Assert.Equal(LineDashType.Dashed, line.Dash);
                Assert.Equal(Color.Blue.ToArgb(), line.Color.ToArgb());
                Assert.Null(line.InstrumentId);
                Assert.Null(line.PortfolioId);
            });
        }
    }
}
