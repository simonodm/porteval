using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Core;
using PortEval.Application.Core.Common;
using PortEval.Application.Core.Interfaces.Calculators;
using PortEval.Application.Core.Interfaces.ChartDataGenerators;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Core.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.Services
{
    public class PortfolioServiceTests
    {
        [Fact]
        public async Task GetAllPortfoliosAsync_ReturnsAllPortfolios()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolios = fixture.CreateMany<PortfolioDto>();

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(q => q.GetAllPortfoliosAsync())
                .ReturnsAsync(portfolios);

            var sut = fixture.Create<PortfolioService>();
            var result = await sut.GetAllPortfoliosAsync();

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(portfolios, result.Response);
        }

        [Fact]
        public async Task GetPortfolioAsync_ReturnsPortfolio_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolio = fixture.Create<PortfolioDto>();
            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.GetPortfolioAsync(portfolio.Id))
                .ReturnsAsync(portfolio);

            var sut = fixture.Create<PortfolioService>();
            var result = await sut.GetPortfolioAsync(portfolio.Id);

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(portfolio, result.Response);
        }

        [Fact]
        public async Task GetPortfolioAsync_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolio = fixture.Create<PortfolioDto>();

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.GetPortfolioAsync(portfolio.Id))
                .ReturnsAsync((PortfolioDto) null);

            var sut = fixture.Create<PortfolioService>();
            var result = await sut.GetPortfolioAsync(portfolio.Id);

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetPortfolioValueAsync_ReturnsCorrectValue_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var value = fixture.Create<decimal>();
            var time = fixture.Create<DateTime>();

            var valueCalculator = fixture.Freeze<Mock<IPositionValueCalculator>>();
            valueCalculator
                .Setup(c => c.CalculateValue(It.IsAny<IEnumerable<PositionPriceRangeData>>(), time))
                .Returns(value);

            var sut = fixture.Create<PortfolioService>();
            var result = await sut.GetPortfolioValueAsync(fixture.Create<int>(), time);

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(value, result.Response.Value);
            Assert.Equal(time, result.Response.Time);
        }

        [Fact]
        public async Task GetPortfolioValueAsync_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.GetPortfolioAsync(It.IsAny<int>()))
                .ReturnsAsync((PortfolioDto) null);

            var sut = fixture.Create<PortfolioService>();
            var result = await sut.GetPortfolioValueAsync(fixture.Create<int>(), fixture.Create<DateTime>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetPortfolioProfitAsync_ReturnsCorrectProfit_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var profit = fixture.Create<decimal>();
            var dateRange = fixture.Create<DateRangeParams>();

            var profitCalculator = fixture.Freeze<Mock<IPositionProfitCalculator>>();
            profitCalculator
                .Setup(c => c.CalculateProfit(It.IsAny<IEnumerable<PositionPriceRangeData>>(), dateRange.From, dateRange.To))
                .Returns(profit);

            var sut = fixture.Create<PortfolioService>();
            var result = await sut.GetPortfolioProfitAsync(fixture.Create<int>(), dateRange);

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(profit, result.Response.Profit);
            Assert.Equal(dateRange.From, result.Response.From);
            Assert.Equal(dateRange.To, result.Response.To);
        }

        [Fact]
        public async Task GetPortfolioProfitAsync_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.GetPortfolioAsync(It.IsAny<int>()))
                .ReturnsAsync((PortfolioDto) null);

            var sut = fixture.Create<PortfolioService>();
            var result = await sut.GetPortfolioProfitAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetPortfolioPerformanceAsync_ReturnsCorrectPerformance_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var performance = fixture.Create<decimal>();
            var dateRange = fixture.Create<DateRangeParams>();

            var performanceCalculator = fixture.Freeze<Mock<IPositionPerformanceCalculator>>();
            performanceCalculator
                .Setup(c => c.CalculatePerformance(It.IsAny<IEnumerable<PositionPriceRangeData>>(), dateRange.From, dateRange.To))
                .Returns(performance);

            var sut = fixture.Create<PortfolioService>();
            var result = await sut.GetPortfolioPerformanceAsync(fixture.Create<int>(), dateRange);

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(performance, result.Response.Performance);
            Assert.Equal(dateRange.From, result.Response.From);
            Assert.Equal(dateRange.To, result.Response.To);
        }

        [Fact]
        public async Task GetPortfolioPerformanceAsync_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.GetPortfolioAsync(It.IsAny<int>()))
                .ReturnsAsync((PortfolioDto) null);

            var sut = fixture.Create<PortfolioService>();
            var result = await sut.GetPortfolioPerformanceAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task ChartPortfolioValueAsync_ReturnsCorrectValueChart_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var valueChart = fixture.CreateMany<EntityChartPointDto>();

            var chartGenerator = fixture.Freeze<Mock<IPositionChartDataGenerator>>();
            chartGenerator
                .Setup(c => c.ChartValue(It.IsAny<PositionPriceListData>(), It.IsAny<DateRangeParams>(), It.IsAny<AggregationFrequency>()))
                .Returns(valueChart);

            var sut = fixture.Create<PortfolioService>();
            var result = await sut.ChartPortfolioValueAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>(), fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(valueChart, result.Response);
        }

        [Fact]
        public async Task ChartPortfolioValueAsync_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.GetPortfolioAsync(It.IsAny<int>()))
                .ReturnsAsync((PortfolioDto) null);

            var sut = fixture.Create<PortfolioService>();
            var result = await sut.ChartPortfolioValueAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>(), fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task ChartPortfolioProfitAsync_ReturnsCorrectProfitChart_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var profitChart = fixture.CreateMany<EntityChartPointDto>();

            var chartGenerator = fixture.Freeze<Mock<IPositionChartDataGenerator>>();
            chartGenerator
                .Setup(c => c.ChartProfit(It.IsAny<PositionPriceListData>(), It.IsAny<DateRangeParams>(), It.IsAny<AggregationFrequency>()))
                .Returns(profitChart);

            var sut = fixture.Create<PortfolioService>();
            var result = await sut.ChartPortfolioProfitAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>(), fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(profitChart, result.Response);
        }

        [Fact]
        public async Task ChartPortfolioProfitAsync_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.GetPortfolioAsync(It.IsAny<int>()))
                .ReturnsAsync((PortfolioDto) null);

            var sut = fixture.Create<PortfolioService>();
            var result = await sut.ChartPortfolioProfitAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>(), fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task ChartPortfolioPerformanceAsync_ReturnsCorrectPerformanceChart_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var performanceChart = fixture.CreateMany<EntityChartPointDto>();

            var chartGenerator = fixture.Freeze<Mock<IPositionChartDataGenerator>>();
            chartGenerator
                .Setup(c => c.ChartPerformance(It.IsAny<PositionPriceListData>(), It.IsAny<DateRangeParams>(), It.IsAny<AggregationFrequency>()))
                .Returns(performanceChart);

            var sut = fixture.Create<PortfolioService>();
            var result = await sut.ChartPortfolioPerformanceAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>(), fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(performanceChart, result.Response);
        }

        [Fact]
        public async Task ChartPortfolioPerformanceAsync_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.GetPortfolioAsync(It.IsAny<int>()))
                .ReturnsAsync((PortfolioDto) null);

            var sut = fixture.Create<PortfolioService>();
            var result = await sut.ChartPortfolioPerformanceAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>(), fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task ChartPortfolioAggregatedProfitAsync_ReturnsCorrectAggregatedProfitChart_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var aggregatedProfitChart = fixture.CreateMany<EntityChartPointDto>();

            var chartGenerator = fixture.Freeze<Mock<IPositionChartDataGenerator>>();
            chartGenerator
                .Setup(c => c.ChartAggregatedProfit(It.IsAny<PositionPriceListData>(), It.IsAny<DateRangeParams>(), It.IsAny<AggregationFrequency>()))
                .Returns(aggregatedProfitChart);

            var sut = fixture.Create<PortfolioService>();
            var result = await sut.ChartPortfolioAggregatedProfitAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>(), fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(aggregatedProfitChart, result.Response);
        }

        [Fact]
        public async Task ChartPortfolioAggregatedProfitAsync_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.GetPortfolioAsync(It.IsAny<int>()))
                .ReturnsAsync((PortfolioDto) null);

            var sut = fixture.Create<PortfolioService>();
            var result = await sut.ChartPortfolioAggregatedProfitAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>(), fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task ChartPortfolioAggregatedPerformanceAsync_ReturnsCorrectAggregatedPerformanceChart_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var aggregatedPerformanceChart = fixture.CreateMany<EntityChartPointDto>();

            var chartGenerator = fixture.Freeze<Mock<IPositionChartDataGenerator>>();
            chartGenerator
                .Setup(c => c.ChartAggregatedPerformance(It.IsAny<PositionPriceListData>(), It.IsAny<DateRangeParams>(), It.IsAny<AggregationFrequency>()))
                .Returns(aggregatedPerformanceChart);

            var sut = fixture.Create<PortfolioService>();
            var result = await sut.ChartPortfolioAggregatedPerformanceAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>(), fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(aggregatedPerformanceChart, result.Response);
        }

        [Fact]
        public async Task ChartPortfolioAggregatedPerformanceAsync_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.GetPortfolioAsync(It.IsAny<int>()))
                .ReturnsAsync((PortfolioDto) null);

            var sut = fixture.Create<PortfolioService>();
            var result = await sut.ChartPortfolioAggregatedPerformanceAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>(), fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetAllPortfoliosStatisticsAsync_ReturnsAllPortfoliosStatistics()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolios = fixture.CreateMany<PortfolioDto>(2);
            var statisticsDto = fixture.Create<EntityStatisticsDto>();

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.GetAllPortfoliosAsync())
                .ReturnsAsync(portfolios);

            var statisticsCalculator = fixture.Freeze<Mock<IPortfolioStatisticsCalculator>>();
            statisticsCalculator
                .Setup(m => m.CalculateStatistics(It.IsAny<PortfolioPositionsPriceListData>(), It.IsAny<DateTime>()))
                .Returns(statisticsDto);

            var sut = fixture.Create<PortfolioService>();
            var result = await sut.GetAllPortfoliosStatisticsAsync();

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Collection(result.Response, s => Assert.Equal(statisticsDto, s), s => Assert.Equal(statisticsDto, s));
        }

        [Fact]
        public async Task GetPortfolioStatisticsDto_ReturnsPortfolioStatistics_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolio = fixture.Create<PortfolioDto>();
            var statisticsDto = fixture.Create<EntityStatisticsDto>();

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.GetPortfolioAsync(It.IsAny<int>()))
                .ReturnsAsync(portfolio);

            var statisticsCalculator = fixture.Freeze<Mock<IPortfolioStatisticsCalculator>>();
            statisticsCalculator
                .Setup(m => m.CalculateStatistics(It.IsAny<PortfolioPositionsPriceListData>(), It.IsAny<DateTime>()))
                .Returns(statisticsDto);

            var sut = fixture.Create<PortfolioService>();
            var result = await sut.GetPortfolioStatisticsAsync(fixture.Create<int>());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(statisticsDto, result.Response);
        }

        [Fact]
        public async Task CreatingPortfolio_AddsPortfolioToRepository_WhenWellFormed()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioDto = fixture.Create<PortfolioDto>();

            var portfolioRepository = fixture.Freeze<Mock<IPortfolioRepository>>();
            fixture.Freeze<Mock<ICurrencyRepository>>();

            var sut = fixture.Create<PortfolioService>();

            await sut.CreatePortfolioAsync(portfolioDto);

            portfolioRepository.Verify(r => r.Add(It.Is<Portfolio>(p =>
                p.Name == portfolioDto.Name &&
                p.Note == portfolioDto.Note &&
                p.CurrencyCode == portfolioDto.CurrencyCode
            )), Times.Once());
        }

        [Fact]
        public async Task CreatingPortfolio_ThrowsException_WhenCurrencyDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioDto = fixture.Create<PortfolioDto>();

            var currencyRepository = fixture.Freeze<Mock<ICurrencyRepository>>();
            currencyRepository
                .Setup(r => r.ExistsAsync(portfolioDto.CurrencyCode))
                .Returns(Task.FromResult(false));

            var sut = fixture.Create<PortfolioService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await sut.CreatePortfolioAsync(portfolioDto));
        }

        [Fact]
        public async Task CreatingPortfolio_ReturnsNewPortfolio_WhenCreatedSuccessfully()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioDto = fixture.Create<PortfolioDto>();

            fixture.Freeze<Mock<IPortfolioRepository>>();
            fixture.Freeze<Mock<ICurrencyRepository>>();

            var sut = fixture.Create<PortfolioService>();

            var result = await sut.CreatePortfolioAsync(portfolioDto);

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(portfolioDto.Name, result.Response.Name);
            Assert.Equal(portfolioDto.CurrencyCode, result.Response.CurrencyCode);
            Assert.Equal(portfolioDto.Note, result.Response.Note);
        }

        [Fact]
        public async Task UpdatingPortfolio_UpdatesPortfolio_WhenWellFormed()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioDto = fixture.Create<PortfolioDto>();

            var portfolioRepository = fixture.Freeze<Mock<IPortfolioRepository>>();
            fixture.Freeze<Mock<ICurrencyRepository>>();

            var sut = fixture.Create<PortfolioService>();

            await sut.UpdatePortfolioAsync(portfolioDto);

            portfolioRepository.Verify(r => r.Update(It.Is<Portfolio>(p =>
                p.Name == portfolioDto.Name &&
                p.Note == portfolioDto.Note &&
                p.CurrencyCode == portfolioDto.CurrencyCode
            )), Times.Once());
        }

        [Fact]
        public async Task UpdatingPortfolio_ReturnsError_WhenCurrencyDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioDto = fixture.Create<PortfolioDto>();

            fixture.Freeze<Mock<IPortfolioRepository>>();
            var currencyRepository = fixture.Freeze<Mock<ICurrencyRepository>>();
            currencyRepository
                .Setup(r => r.ExistsAsync(portfolioDto.CurrencyCode))
                .Returns(Task.FromResult(false));

            var sut = fixture.Create<PortfolioService>();
            var result = await sut.UpdatePortfolioAsync(portfolioDto);

            Assert.Equal(OperationStatus.Error, result.Status);
        }

        [Fact]
        public async Task UpdatingPortfolio_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioDto = fixture.Create<PortfolioDto>();

            var portfolioRepository = fixture.Freeze<Mock<IPortfolioRepository>>();
            portfolioRepository
                .Setup(r => r.ExistsAsync(portfolioDto.Id))
                .Returns(Task.FromResult(false));
            portfolioRepository
                .Setup(r => r.FindAsync(portfolioDto.Id))
                .Returns(Task.FromResult<Portfolio>(null));
            fixture.Freeze<Mock<ICurrencyRepository>>();

            var sut = fixture.Create<PortfolioService>();
            var result = await sut.UpdatePortfolioAsync(portfolioDto);

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task UpdatingPortfolio_ReturnsNewPortfolio_WhenUpdatedSuccessfully()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioDto = fixture.Create<PortfolioDto>();

            fixture.Freeze<Mock<IPortfolioRepository>>();
            fixture.Freeze<Mock<ICurrencyRepository>>();

            var sut = fixture.Create<PortfolioService>();

            var result = await sut.UpdatePortfolioAsync(portfolioDto);

            Assert.Equal(portfolioDto.Name, result.Response.Name);
            Assert.Equal(portfolioDto.CurrencyCode, result.Response.CurrencyCode);
            Assert.Equal(portfolioDto.Note, result.Response.Note);
        }

        [Fact]
        public async Task DeletingPortfolio_DeletesPortfolio_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();

            var portfolioRepository = fixture.Freeze<Mock<IPortfolioRepository>>();
            fixture.Freeze<Mock<ICurrencyRepository>>();

            var sut = fixture.Create<PortfolioService>();

            await sut.DeletePortfolioAsync(portfolioId);

            portfolioRepository.Verify(r => r.DeleteAsync(portfolioId), Times.Once());
        }

        [Fact]
        public async Task DeletingPortfolio_ThrowsException_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();

            var portfolioRepository = fixture.Freeze<Mock<IPortfolioRepository>>();
            portfolioRepository
                .Setup(r => r.ExistsAsync(portfolioId))
                .Returns(Task.FromResult(false));
            fixture.Freeze<Mock<ICurrencyRepository>>();

            var sut = fixture.Create<PortfolioService>();
            var result = await sut.DeletePortfolioAsync(portfolioId);

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }
    }
}