using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Features.Interfaces.Queries;
using PortEval.Application.Features.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Tests.Unit.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.ControllerTests
{
    public class PortfolioControllerTests
    {
        [Fact]
        public async Task GetPortfolios_ReturnsPortfolios()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolios = fixture.CreateMany<PortfolioDto>();

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.GetPortfolios())
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(portfolios));
            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolios();

            portfolioQueries.Verify(m => m.GetPortfolios(), Times.Once());
            Assert.Equal(portfolios, result.Value);
        }

        [Fact]
        public async Task GetPortfolio_ReturnsCorrectPortfolio_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolio = fixture.Create<PortfolioDto>();

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.GetPortfolio(portfolio.Id))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(portfolio));

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolio(portfolio.Id);

            portfolioQueries.Verify(m => m.GetPortfolio(portfolio.Id), Times.Once());
            Assert.Equal(portfolio, result.Value);
        }

        [Fact]
        public async Task GetPortfolio_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.GetPortfolio(It.IsAny<int>()))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<PortfolioDto>());

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolio(portfolioId);

            portfolioQueries.Verify(m => m.GetPortfolio(portfolioId), Times.Once());
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPositions_ReturnsPositions_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();
            var positions = fixture.CreateMany<PositionDto>();

            var positionQueries = fixture.Freeze<Mock<IPositionQueries>>();
            positionQueries
                .Setup(m => m.GetPortfolioPositions(portfolioId))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(positions));

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPositions(portfolioId);

            positionQueries.Verify(m => m.GetPortfolioPositions(portfolioId));
            Assert.Equal(positions, result.Value);
        }

        [Fact]
        public async Task GetPositions_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();

            var positionQueries = fixture.Freeze<Mock<IPositionQueries>>();
            positionQueries
                .Setup(m => m.GetPortfolioPositions(portfolioId))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<IEnumerable<PositionDto>>());

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPositions(portfolioId);

            positionQueries.Verify(m => m.GetPortfolioPositions(portfolioId));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPositionsStatistics_ReturnsPortfolioPositionsStatistics_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();
            var statistics = fixture.CreateMany<PositionStatisticsDto>();

            var positionQueries = fixture.Freeze<Mock<IPositionQueries>>();
            positionQueries
                .Setup(m => m.GetPortfolioPositionsStatistics(portfolioId))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(statistics));

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionsStatistics(portfolioId);

            positionQueries.Verify(m => m.GetPortfolioPositionsStatistics(portfolioId));
            Assert.Equal(statistics, result.Value);
        }

        [Fact]
        public async Task GetPositionStatistics_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();

            var positionQueries = fixture.Freeze<Mock<IPositionQueries>>();
            positionQueries
                .Setup(m => m.GetPortfolioPositionsStatistics(portfolioId))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<IEnumerable<PositionStatisticsDto>>());

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionsStatistics(portfolioId);

            positionQueries.Verify(m => m.GetPortfolioPositionsStatistics(portfolioId));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPortfolioValue_ReturnsPortfolioValue_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();
            var value = fixture.Create<EntityValueDto>();

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.GetPortfolioValue(portfolioId, value.Time))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(value));

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioValue(portfolioId, value.Time);

            portfolioQueries.Verify(m => m.GetPortfolioValue(portfolioId, value.Time));
            Assert.Equal(value, result.Value);
        }

        [Fact]
        public async Task GetPortfolioValue_ReturnsCurrentPortfolioValue_WhenTimeParameterIsNotProvided()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var now = DateTime.UtcNow;
            var portfolioId = fixture.Create<int>();
            var value = fixture.Build<EntityValueDto>().With(v => v.Time, now).Create();

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.GetPortfolioValue(portfolioId, It.Is<DateTime>(d => d >= now)))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(value));

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioValue(portfolioId, null);

            portfolioQueries.Verify(m => m.GetPortfolioValue(portfolioId, It.Is<DateTime>(d => d >= now)));
            Assert.Equal(value, result.Value);
        }

        [Fact]
        public async Task GetPortfolioValue_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.GetPortfolioValue(portfolioId, It.IsAny<DateTime>()))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<EntityValueDto>());

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioValue(portfolioId, DateTime.UtcNow);

            portfolioQueries.Verify(m => m.GetPortfolioValue(portfolioId, It.IsAny<DateTime>()));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPortfolioProfit_ReturnsProfit_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();
            var profit = fixture.Create<EntityProfitDto>();
            var dateRange = fixture.Create<DateRangeParams>();

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.GetPortfolioProfit(portfolioId, dateRange))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(profit));

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioProfit(portfolioId, dateRange);

            portfolioQueries.Verify(m => m.GetPortfolioProfit(portfolioId, dateRange));
            Assert.Equal(profit, result.Value);
        }

        [Fact]
        public async Task GetPortfolioProfit_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();
            var dateRange = fixture.Create<DateRangeParams>();

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.GetPortfolioProfit(portfolioId, dateRange))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<EntityProfitDto>());

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioProfit(portfolioId, dateRange);

            portfolioQueries.Verify(m => m.GetPortfolioProfit(portfolioId, dateRange));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPortfolioPerformance_ReturnsPortfolioPerformance_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();
            var performance = fixture.Create<EntityPerformanceDto>();
            var dateRange = fixture.Create<DateRangeParams>();

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.GetPortfolioPerformance(portfolioId, dateRange))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(performance));

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioPerformance(portfolioId, dateRange);

            portfolioQueries.Verify(m => m.GetPortfolioPerformance(portfolioId, dateRange));
            Assert.Equal(performance, result.Value);
        }

        [Fact]
        public async Task GetPortfolioPerformance_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();
            var dateRange = fixture.Create<DateRangeParams>();

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.GetPortfolioPerformance(portfolioId, dateRange))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<EntityPerformanceDto>());

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioPerformance(portfolioId, dateRange);

            portfolioQueries.Verify(m => m.GetPortfolioPerformance(portfolioId, dateRange));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPortfolioChartedValue_ReturnsChartedValue_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();
            var chartedValue = fixture.CreateMany<EntityChartPointDto>();
            var dateRange = fixture.Create<DateRangeParams>();
            var aggregationFrequency = fixture.Create<AggregationFrequency>();
            var currencyCode = fixture.Create<string>();

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.ChartPortfolioValue(portfolioId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(chartedValue));

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioChartedValue(portfolioId, dateRange, aggregationFrequency, currencyCode);

            portfolioQueries.Verify(m => m.ChartPortfolioValue(portfolioId, dateRange, aggregationFrequency, currencyCode));
            Assert.Equal(chartedValue, result.Value);
        }

        [Fact]
        public async Task GetPortfolioChartedValue_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();
            var dateRange = fixture.Create<DateRangeParams>();
            var aggregationFrequency = fixture.Create<AggregationFrequency>();
            var currencyCode = fixture.Create<string>();

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.ChartPortfolioValue(portfolioId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<IEnumerable<EntityChartPointDto>>());

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioChartedValue(portfolioId, dateRange, aggregationFrequency, currencyCode);

            portfolioQueries.Verify(m => m.ChartPortfolioValue(portfolioId, dateRange, aggregationFrequency, currencyCode));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPortfolioChartedProfit_ReturnsChartedProfit_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();
            var chartedProfit = fixture.CreateMany<EntityChartPointDto>();
            var dateRange = fixture.Create<DateRangeParams>();
            var aggregationFrequency = fixture.Create<AggregationFrequency>();
            var currencyCode = fixture.Create<string>();

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.ChartPortfolioProfit(portfolioId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(chartedProfit));

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioChartedProfit(portfolioId, dateRange, aggregationFrequency, currencyCode);

            portfolioQueries.Verify(m => m.ChartPortfolioProfit(portfolioId, dateRange, aggregationFrequency, currencyCode));
            Assert.Equal(chartedProfit, result.Value);
        }

        [Fact]
        public async Task GetPortfolioChartedProfit_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();
            var dateRange = fixture.Create<DateRangeParams>();
            var aggregationFrequency = fixture.Create<AggregationFrequency>();
            var currencyCode = fixture.Create<string>();

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.ChartPortfolioProfit(portfolioId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<IEnumerable<EntityChartPointDto>>());

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioChartedProfit(portfolioId, dateRange, aggregationFrequency, currencyCode);

            portfolioQueries.Verify(m => m.ChartPortfolioProfit(portfolioId, dateRange, aggregationFrequency, currencyCode));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPortfolioChartedPerformance_ReturnsChartedPerformance_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();
            var chartedPerformance = fixture.CreateMany<EntityChartPointDto>();
            var dateRange = fixture.Create<DateRangeParams>();
            var aggregationFrequency = fixture.Create<AggregationFrequency>();

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.ChartPortfolioPerformance(portfolioId, dateRange, aggregationFrequency))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(chartedPerformance));

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioChartedPerformance(portfolioId, dateRange, aggregationFrequency);

            portfolioQueries.Verify(m => m.ChartPortfolioPerformance(portfolioId, dateRange, aggregationFrequency));
            Assert.Equal(chartedPerformance, result.Value);
        }

        [Fact]
        public async Task GetPortfolioChartedPerformance_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();
            var dateRange = fixture.Create<DateRangeParams>();
            var aggregationFrequency = fixture.Create<AggregationFrequency>();

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.ChartPortfolioPerformance(portfolioId, dateRange, aggregationFrequency))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<IEnumerable<EntityChartPointDto>>());

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioChartedPerformance(portfolioId, dateRange, aggregationFrequency);

            portfolioQueries.Verify(m => m.ChartPortfolioPerformance(portfolioId, dateRange, aggregationFrequency));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPortfolioAggregatedPerformance_ReturnsAggregatedPerformance_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();
            var aggregatedPerformance = fixture.CreateMany<EntityChartPointDto>();
            var dateRange = fixture.Create<DateRangeParams>();
            var aggregationFrequency = fixture.Create<AggregationFrequency>();

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.ChartPortfolioPerformanceAggregated(portfolioId, dateRange, aggregationFrequency))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(aggregatedPerformance));

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioAggregatedPerformance(portfolioId, dateRange, aggregationFrequency);

            portfolioQueries.Verify(m => m.ChartPortfolioPerformanceAggregated(portfolioId, dateRange, aggregationFrequency));
            Assert.Equal(aggregatedPerformance, result.Value);
        }

        [Fact]
        public async Task GetPortfolioAggregatedPerformance_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();
            var dateRange = fixture.Create<DateRangeParams>();
            var aggregationFrequency = fixture.Create<AggregationFrequency>();

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.ChartPortfolioPerformanceAggregated(portfolioId, dateRange, aggregationFrequency))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<IEnumerable<EntityChartPointDto>>());

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioAggregatedPerformance(portfolioId, dateRange, aggregationFrequency);

            portfolioQueries.Verify(m => m.ChartPortfolioPerformanceAggregated(portfolioId, dateRange, aggregationFrequency));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPortfolioAggregatedProfit_ReturnsAggregatedProfit_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();
            var aggregatedProfit = fixture.CreateMany<EntityChartPointDto>();
            var dateRange = fixture.Create<DateRangeParams>();
            var aggregationFrequency = fixture.Create<AggregationFrequency>();
            var currencyCode = fixture.Create<string>();

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.ChartPortfolioProfitAggregated(portfolioId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(aggregatedProfit));

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioAggregatedProfit(portfolioId, dateRange, aggregationFrequency, currencyCode);

            portfolioQueries.Verify(m => m.ChartPortfolioProfitAggregated(portfolioId, dateRange, aggregationFrequency, currencyCode));
            Assert.Equal(aggregatedProfit, result.Value);
        }

        [Fact]
        public async Task GetPortfolioAggregatedProfit_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();
            var dateRange = fixture.Create<DateRangeParams>();
            var aggregationFrequency = fixture.Create<AggregationFrequency>();
            var currencyCode = fixture.Create<string>();

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.ChartPortfolioProfitAggregated(portfolioId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<IEnumerable<EntityChartPointDto>>());

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioAggregatedProfit(portfolioId, dateRange, aggregationFrequency, currencyCode);

            portfolioQueries.Verify(m => m.ChartPortfolioProfitAggregated(portfolioId, dateRange, aggregationFrequency, currencyCode));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetAllPortfoliosStatistics_ReturnsStatistics()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var statistics = fixture.CreateMany<EntityStatisticsDto>();

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.GetAllPortfoliosStatistics())
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(statistics));

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetAllPortfoliosStatistics();

            portfolioQueries.Verify(m => m.GetAllPortfoliosStatistics(), Times.Once());
            Assert.Equal(statistics, result.Value);
        }

        [Fact]
        public async Task GetPortfolioStatistics_ReturnsPortfolioStatistics_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();
            var statistics = fixture.Create<EntityStatisticsDto>();

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.GetPortfolioStatistics(portfolioId))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(statistics));

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioStatistics(portfolioId);

            portfolioQueries.Verify(m => m.GetPortfolioStatistics(portfolioId), Times.Once());
            Assert.Equal(statistics, result.Value);
        }

        [Fact]
        public async Task GetPortfolioStatistics_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.GetPortfolioStatistics(portfolioId))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<EntityStatisticsDto>());

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioStatistics(portfolioId);

            portfolioQueries.Verify(m => m.GetPortfolioStatistics(portfolioId), Times.Once());
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task PostPortfolio_CreatesPortfolio()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolio = fixture.Create<PortfolioDto>();

            var portfolioService = fixture.Freeze<Mock<IPortfolioService>>();
            portfolioService
                .Setup(m => m.CreatePortfolioAsync(portfolio))
                .ReturnsAsync(fixture.Create<Portfolio>());
            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            await sut.PostPortfolio(portfolio);

            portfolioService.Verify(m => m.CreatePortfolioAsync(portfolio), Times.Once());
        }

        [Fact]
        public async Task PutPortfolio_UpdatesPortfolio()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolio = fixture.Create<PortfolioDto>();

            var portfolioService = fixture.Freeze<Mock<IPortfolioService>>();
            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            await sut.PutPortfolio(portfolio.Id, portfolio);

            portfolioService.Verify(m => m.UpdatePortfolioAsync(portfolio), Times.Once());
        }

        [Fact]
        public async Task PutPortfolio_ReturnsBadRequest_WhenQueryParameterIdAndBodyIdDontMatch()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolio = fixture.Create<PortfolioDto>();

            var portfolioService = fixture.Freeze<Mock<IPortfolioService>>();
            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.PutPortfolio(portfolio.Id + 1, portfolio);

            portfolioService.Verify(m => m.UpdatePortfolioAsync(portfolio), Times.Never());
            Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeletePortfolio_DeletesPortfolio()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();

            var portfolioService = fixture.Freeze<Mock<IPortfolioService>>();
            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            await sut.DeletePortfolio(portfolioId);

            portfolioService.Verify(m => m.DeletePortfolioAsync(portfolioId), Times.Once());
        }
    }
}
