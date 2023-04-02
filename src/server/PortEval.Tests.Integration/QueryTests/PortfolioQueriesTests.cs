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
using PortEval.Application.Core.Queries;
using Xunit;

namespace PortEval.Tests.Integration.QueryTests
{
    [Collection("Query test collection")]
    public class PortfolioQueriesTests
    {
        private readonly IPortfolioQueries _portfolioQueries;

        private readonly int _firstPortfolioId;
        private readonly int _secondPortfolioId;

        public PortfolioQueriesTests(QueryTestFixture fixture)
        {
            using var scope = fixture.Factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PortEvalDbContext>();
            _portfolioQueries = scope.ServiceProvider.GetRequiredService<IPortfolioQueries>();

            _firstPortfolioId = context.Portfolios.First(p => p.Name == "Portfolio 1").Id;
            _secondPortfolioId = context.Portfolios.First(p => p.Name == "Portfolio 2").Id;
        }

        [Fact]
        public async Task GetPortfolios_ReturnsAllPortfolios()
        {
            var queryResult = await _portfolioQueries.GetPortfolios();

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            Assert.Collection(queryResult.Response, AssertIsFirstPortfolio, AssertIsSecondPortfolio);
        }

        [Fact]
        public async Task GetPortfolio_ReturnsCorrectPortfolio_WhenPortfolioExists()
        {
            var queryResult = await _portfolioQueries.GetPortfolio(_firstPortfolioId);

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            AssertIsFirstPortfolio(queryResult.Response);
        }

        [Fact]
        public async Task GetPortfolio_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var queryResult = await _portfolioQueries.GetPortfolio(-1);

            Assert.Equal(QueryStatus.NotFound, queryResult.Status);
        }

        [Fact]
        public async Task GetPortfolioValue_ReturnsCorrectValue()
        {
            var time = DateTime.UtcNow;

            var queryResult = await _portfolioQueries.GetPortfolioValue(_firstPortfolioId, time);

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            Assert.Equal(150m, queryResult.Response.Value);
            Assert.Equal("USD", queryResult.Response.CurrencyCode);
            Assert.Equal(time, queryResult.Response.Time);
        }

        [Fact]
        public async Task GetPortfolioValue_ReturnsCorrectValue_WhenCurrencyConversionTakesPlace()
        {
            var time = DateTime.UtcNow;

            var queryResult = await _portfolioQueries.GetPortfolioValue(_secondPortfolioId, time);

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            Assert.Equal(5050m, queryResult.Response.Value);
            Assert.Equal("EUR", queryResult.Response.CurrencyCode);
            Assert.Equal(time, queryResult.Response.Time);
        }

        [Fact]
        public async Task GetPortfolioValue_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var queryResult = await _portfolioQueries.GetPortfolioValue(-1, DateTime.UtcNow);

            Assert.Equal(QueryStatus.NotFound, queryResult.Status);
        }

        [Fact]
        public async Task GetPortfolioProfit_ReturnsCorrectValue()
        {
            var queryResult = await _portfolioQueries.GetPortfolioProfit(_firstPortfolioId, new DateRangeParams());

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            Assert.Equal(50m, queryResult.Response.Profit);
            Assert.Equal("USD", queryResult.Response.CurrencyCode);
            Assert.Equal(DateTime.UtcNow, queryResult.Response.To, TimeSpan.FromMinutes(1));
            Assert.Equal(PortEvalConstants.FinancialDataStartTime, queryResult.Response.From);
        }

        [Fact]
        public async Task GetPortfolioProfit_ReturnsCorrectProfit_WhenCurrencyConversionTakesPlace()
        {
            var queryResult = await _portfolioQueries.GetPortfolioProfit(_secondPortfolioId, new DateRangeParams());

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            Assert.Equal(-19950m, queryResult.Response.Profit);
            Assert.Equal("EUR", queryResult.Response.CurrencyCode);
            Assert.Equal(DateTime.UtcNow, queryResult.Response.To, TimeSpan.FromMinutes(1));
            Assert.Equal(PortEvalConstants.FinancialDataStartTime, queryResult.Response.From);
        }

        [Fact]
        public async Task GetPortfolioProfit_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var queryResult = await _portfolioQueries.GetPortfolioProfit(-1, new DateRangeParams());

            Assert.Equal(QueryStatus.NotFound, queryResult.Status);
        }

        [Fact]
        public async Task GetPortfolioPerformance_ReturnsCorrectPerformance()
        {
            var queryResult = await _portfolioQueries.GetPortfolioPerformance(_firstPortfolioId, new DateRangeParams());

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            Assert.Equal(0.5m, queryResult.Response.Performance);
            Assert.Equal(DateTime.UtcNow, queryResult.Response.To, TimeSpan.FromMinutes(1));
            Assert.Equal(PortEvalConstants.FinancialDataStartTime, queryResult.Response.From);
        }

        [Fact]
        public async Task GetPortfolioPerformance_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var queryResult = await _portfolioQueries.GetPortfolioPerformance(-1, new DateRangeParams());

            Assert.Equal(QueryStatus.NotFound, queryResult.Status);
        }

        [Fact]
        public async Task ChartPortfolioValue_ReturnsChartedPortfolioValue()
        {
            var queryResult = await _portfolioQueries.ChartPortfolioValue(_firstPortfolioId, new DateRangeParams(),
                AggregationFrequency.Day);

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
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
        public async Task ChartPortfolioValue_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var queryResult = await _portfolioQueries.ChartPortfolioValue(-1, new DateRangeParams(), AggregationFrequency.Day);

            Assert.Equal(QueryStatus.NotFound, queryResult.Status);
        }

        [Fact]
        public async Task ChartPortfolioProfit_ReturnsChartedPortfolioProfit()
        {
            var queryResult = await _portfolioQueries.ChartPortfolioProfit(_firstPortfolioId, new DateRangeParams(),
                AggregationFrequency.Day);

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
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
        public async Task ChartPortfolioProfit_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var queryResult = await _portfolioQueries.ChartPortfolioProfit(-1, new DateRangeParams(), AggregationFrequency.Day);

            Assert.Equal(QueryStatus.NotFound, queryResult.Status);
        }

        [Fact]
        public async Task ChartPortfolioPerformance_ReturnsChartedPortfolioPerformance()
        {
            var queryResult = await _portfolioQueries.ChartPortfolioPerformance(_firstPortfolioId, new DateRangeParams(),
                AggregationFrequency.Day);

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
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
        public async Task ChartPortfolioPerformance_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var queryResult = await _portfolioQueries.ChartPortfolioPerformance(-1, new DateRangeParams(), AggregationFrequency.Day);

            Assert.Equal(QueryStatus.NotFound, queryResult.Status);
        }

        [Fact]
        public async Task ChartPortfolioProfitAggregated_ReturnsChartedPortfolioAggregatedProfit()
        {
            var queryResult = await _portfolioQueries.ChartPortfolioProfitAggregated(_firstPortfolioId, new DateRangeParams(),
                AggregationFrequency.Day);

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
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
        public async Task ChartPortfolioProfitAggregated_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var queryResult = await _portfolioQueries.ChartPortfolioProfitAggregated(-1, new DateRangeParams(), AggregationFrequency.Day);

            Assert.Equal(QueryStatus.NotFound, queryResult.Status);
        }

        [Fact]
        public async Task ChartPortfolioPerformanceAggregated_ReturnsChartedPortfolioAggregatedPerformance()
        {
            var queryResult = await _portfolioQueries.ChartPortfolioPerformanceAggregated(_firstPortfolioId, new DateRangeParams(),
                AggregationFrequency.Day);

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
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
        public async Task ChartPortfolioPerformanceAggregated_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var queryResult = await _portfolioQueries.ChartPortfolioPerformanceAggregated(-1, new DateRangeParams(), AggregationFrequency.Day);

            Assert.Equal(QueryStatus.NotFound, queryResult.Status);
        }

        [Fact]
        public async Task GetAllPortfolioStatistics_ReturnsCorrectStatistics()
        {
            var queryResult = await _portfolioQueries.GetAllPortfoliosStatistics();

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            Assert.Collection(queryResult.Response, AssertIsFirstPortfolioPerformanceStatistics, AssertIsSecondPortfolioPerformanceStatistics);
        }

        [Fact]
        public async Task GetPortfolioStatistics_ReturnsCorrectStatistics()
        {
            var queryResult = await _portfolioQueries.GetPortfolioStatistics(_firstPortfolioId);

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            AssertIsFirstPortfolioPerformanceStatistics(queryResult.Response);
        }

        [Fact]
        public async Task GetPortfolioStatistics_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var queryResult = await _portfolioQueries.GetPortfolioStatistics(-1);

            Assert.Equal(QueryStatus.NotFound, queryResult.Status);
        }

        private void AssertIsFirstPortfolio(PortfolioDto p)
        {
            Assert.Equal(_firstPortfolioId, p.Id);
            Assert.Equal("Portfolio 1", p.Name);
            Assert.Equal("USD", p.CurrencyCode);
            Assert.Equal("Test note 1", p.Note);
        }

        private void AssertIsSecondPortfolio(PortfolioDto p)
        {
            Assert.Equal(_secondPortfolioId, p.Id);
            Assert.Equal("Portfolio 2", p.Name);
            Assert.Equal("EUR", p.CurrencyCode);
            Assert.Equal("Test note 2", p.Note);
        }

        private void AssertIsFirstPortfolioPerformanceStatistics(EntityStatisticsDto s)
        {
            var expectedPerformance = 0.5m;
            var expectedLastDayPerformance = 0.0714m;

            var expectedProfit = 50m;
            var expectedLastDayProfit = 10m;

            Assert.Equal(expectedPerformance, s.TotalPerformance);
            Assert.Equal(expectedPerformance, s.LastMonthPerformance);
            Assert.Equal(expectedPerformance, s.LastWeekPerformance);
            Assert.InRange(s.LastDayPerformance, expectedLastDayPerformance - 0.01m, expectedLastDayPerformance + 0.01m);

            Assert.Equal(expectedProfit, s.TotalProfit);
            Assert.Equal(expectedProfit, s.LastMonthProfit);
            Assert.Equal(expectedProfit, s.LastWeekProfit);
            Assert.Equal(expectedLastDayProfit, s.LastDayProfit);
        }

        private void AssertIsSecondPortfolioPerformanceStatistics(EntityStatisticsDto s)
        {
            var expectedPerformance = -0.796m;
            var expectedLastDayPerformance = -0.505m;

            var expectedProfit = -19950m;
            var expectedLastDayProfit = -4950m;

            Assert.InRange(s.TotalPerformance, expectedPerformance - 0.01m, expectedPerformance + 0.01m);
            Assert.InRange(s.LastMonthPerformance, expectedPerformance - 0.01m, expectedPerformance + 0.01m);
            Assert.InRange(s.LastWeekPerformance, expectedPerformance - 0.01m, expectedPerformance + 0.01m);
            Assert.InRange(s.LastDayPerformance, expectedLastDayPerformance - 0.01m, expectedLastDayPerformance + 0.01m);

            Assert.Equal(expectedProfit, s.TotalProfit);
            Assert.Equal(expectedProfit, s.LastMonthProfit);
            Assert.Equal(expectedProfit, s.LastWeekProfit);
            Assert.Equal(expectedLastDayProfit, s.LastDayProfit);
        }
    }
}
