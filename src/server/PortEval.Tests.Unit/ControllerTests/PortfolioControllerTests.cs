using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Tests.Unit.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Services;
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

            var portfolioService = fixture.Freeze<Mock<IPortfolioService>>();
            portfolioService
                .Setup(m => m.GetAllPortfoliosAsync())
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(portfolios));
            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolios();

            portfolioService.Verify(m => m.GetAllPortfoliosAsync(), Times.Once());
            Assert.Equal(portfolios, result.Value);
        }

        [Fact]
        public async Task GetPortfolio_ReturnsCorrectPortfolio_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolio = fixture.Create<PortfolioDto>();

            var portfolioService = fixture.Freeze<Mock<IPortfolioService>>();
            portfolioService
                .Setup(m => m.GetPortfolioAsync(portfolio.Id))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(portfolio));

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolio(portfolio.Id);

            portfolioService.Verify(m => m.GetPortfolioAsync(portfolio.Id), Times.Once());
            Assert.Equal(portfolio, result.Value);
        }

        [Fact]
        public async Task GetPortfolio_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();

            var portfolioService = fixture.Freeze<Mock<IPortfolioService>>();
            portfolioService
                .Setup(m => m.GetPortfolioAsync(It.IsAny<int>()))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<PortfolioDto>());

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolio(portfolioId);

            portfolioService.Verify(m => m.GetPortfolioAsync(portfolioId), Times.Once());
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPositions_ReturnsPositions_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();
            var positions = fixture.CreateMany<PositionDto>();

            var positionService = fixture.Freeze<Mock<IPositionService>>();
            positionService
                .Setup(m => m.GetPortfolioPositionsAsync(portfolioId))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(positions));

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPositions(portfolioId);

            positionService.Verify(m => m.GetPortfolioPositionsAsync(portfolioId));
            Assert.Equal(positions, result.Value);
        }

        [Fact]
        public async Task GetPositions_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();

            var positionService = fixture.Freeze<Mock<IPositionService>>();
            positionService
                .Setup(m => m.GetPortfolioPositionsAsync(portfolioId))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<IEnumerable<PositionDto>>());

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPositions(portfolioId);

            positionService.Verify(m => m.GetPortfolioPositionsAsync(portfolioId));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPositionsStatistics_ReturnsPortfolioPositionsStatistics_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();
            var statistics = fixture.CreateMany<PositionStatisticsDto>();

            var positionService = fixture.Freeze<Mock<IPositionService>>();
            positionService
                .Setup(m => m.GetPortfolioPositionsStatisticsAsync(portfolioId))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(statistics));

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionsStatistics(portfolioId);

            positionService.Verify(m => m.GetPortfolioPositionsStatisticsAsync(portfolioId));
            Assert.Equal(statistics, result.Value);
        }

        [Fact]
        public async Task GetPositionStatistics_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();

            var positionService = fixture.Freeze<Mock<IPositionService>>();
            positionService
                .Setup(m => m.GetPortfolioPositionsStatisticsAsync(portfolioId))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<IEnumerable<PositionStatisticsDto>>());

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionsStatistics(portfolioId);

            positionService.Verify(m => m.GetPortfolioPositionsStatisticsAsync(portfolioId));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPortfolioValue_ReturnsPortfolioValue_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();
            var value = fixture.Create<EntityValueDto>();

            var portfolioService = fixture.Freeze<Mock<IPortfolioService>>();
            portfolioService
                .Setup(m => m.GetPortfolioValueAsync(portfolioId, value.Time))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(value));

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioValue(portfolioId, value.Time);

            portfolioService.Verify(m => m.GetPortfolioValueAsync(portfolioId, value.Time));
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

            var portfolioService = fixture.Freeze<Mock<IPortfolioService>>();
            portfolioService
                .Setup(m => m.GetPortfolioValueAsync(portfolioId, It.Is<DateTime>(d => d >= now)))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(value));

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioValue(portfolioId, null);

            portfolioService.Verify(m => m.GetPortfolioValueAsync(portfolioId, It.Is<DateTime>(d => d >= now)));
            Assert.Equal(value, result.Value);
        }

        [Fact]
        public async Task GetPortfolioValue_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();

            var portfolioService = fixture.Freeze<Mock<IPortfolioService>>();
            portfolioService
                .Setup(m => m.GetPortfolioValueAsync(portfolioId, It.IsAny<DateTime>()))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<EntityValueDto>());

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioValue(portfolioId, DateTime.UtcNow);

            portfolioService.Verify(m => m.GetPortfolioValueAsync(portfolioId, It.IsAny<DateTime>()));
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

            var portfolioService = fixture.Freeze<Mock<IPortfolioService>>();
            portfolioService
                .Setup(m => m.GetPortfolioProfitAsync(portfolioId, dateRange))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(profit));

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioProfit(portfolioId, dateRange);

            portfolioService.Verify(m => m.GetPortfolioProfitAsync(portfolioId, dateRange));
            Assert.Equal(profit, result.Value);
        }

        [Fact]
        public async Task GetPortfolioProfit_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();
            var dateRange = fixture.Create<DateRangeParams>();

            var portfolioService = fixture.Freeze<Mock<IPortfolioService>>();
            portfolioService
                .Setup(m => m.GetPortfolioProfitAsync(portfolioId, dateRange))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<EntityProfitDto>());

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioProfit(portfolioId, dateRange);

            portfolioService.Verify(m => m.GetPortfolioProfitAsync(portfolioId, dateRange));
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

            var portfolioService = fixture.Freeze<Mock<IPortfolioService>>();
            portfolioService
                .Setup(m => m.GetPortfolioPerformanceAsync(portfolioId, dateRange))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(performance));

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioPerformance(portfolioId, dateRange);

            portfolioService.Verify(m => m.GetPortfolioPerformanceAsync(portfolioId, dateRange));
            Assert.Equal(performance, result.Value);
        }

        [Fact]
        public async Task GetPortfolioPerformance_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();
            var dateRange = fixture.Create<DateRangeParams>();

            var portfolioService = fixture.Freeze<Mock<IPortfolioService>>();
            portfolioService
                .Setup(m => m.GetPortfolioPerformanceAsync(portfolioId, dateRange))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<EntityPerformanceDto>());

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioPerformance(portfolioId, dateRange);

            portfolioService.Verify(m => m.GetPortfolioPerformanceAsync(portfolioId, dateRange));
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

            var portfolioService = fixture.Freeze<Mock<IPortfolioService>>();
            portfolioService
                .Setup(m => m.ChartPortfolioValueAsync(portfolioId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(chartedValue));

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioChartedValue(portfolioId, dateRange, aggregationFrequency, currencyCode);

            portfolioService.Verify(m => m.ChartPortfolioValueAsync(portfolioId, dateRange, aggregationFrequency, currencyCode));
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

            var portfolioService = fixture.Freeze<Mock<IPortfolioService>>();
            portfolioService
                .Setup(m => m.ChartPortfolioValueAsync(portfolioId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<IEnumerable<EntityChartPointDto>>());

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioChartedValue(portfolioId, dateRange, aggregationFrequency, currencyCode);

            portfolioService.Verify(m => m.ChartPortfolioValueAsync(portfolioId, dateRange, aggregationFrequency, currencyCode));
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

            var portfolioService = fixture.Freeze<Mock<IPortfolioService>>();
            portfolioService
                .Setup(m => m.ChartPortfolioProfitAsync(portfolioId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(chartedProfit));

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioChartedProfit(portfolioId, dateRange, aggregationFrequency, currencyCode);

            portfolioService.Verify(m => m.ChartPortfolioProfitAsync(portfolioId, dateRange, aggregationFrequency, currencyCode));
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

            var portfolioService = fixture.Freeze<Mock<IPortfolioService>>();
            portfolioService
                .Setup(m => m.ChartPortfolioProfitAsync(portfolioId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<IEnumerable<EntityChartPointDto>>());

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioChartedProfit(portfolioId, dateRange, aggregationFrequency, currencyCode);

            portfolioService.Verify(m => m.ChartPortfolioProfitAsync(portfolioId, dateRange, aggregationFrequency, currencyCode));
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

            var portfolioService = fixture.Freeze<Mock<IPortfolioService>>();
            portfolioService
                .Setup(m => m.ChartPortfolioPerformanceAsync(portfolioId, dateRange, aggregationFrequency))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(chartedPerformance));

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioChartedPerformance(portfolioId, dateRange, aggregationFrequency);

            portfolioService.Verify(m => m.ChartPortfolioPerformanceAsync(portfolioId, dateRange, aggregationFrequency));
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

            var portfolioService = fixture.Freeze<Mock<IPortfolioService>>();
            portfolioService
                .Setup(m => m.ChartPortfolioPerformanceAsync(portfolioId, dateRange, aggregationFrequency))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<IEnumerable<EntityChartPointDto>>());

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioChartedPerformance(portfolioId, dateRange, aggregationFrequency);

            portfolioService.Verify(m => m.ChartPortfolioPerformanceAsync(portfolioId, dateRange, aggregationFrequency));
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

            var portfolioService = fixture.Freeze<Mock<IPortfolioService>>();
            portfolioService
                .Setup(m => m.ChartPortfolioAggregatedPerformanceAsync(portfolioId, dateRange, aggregationFrequency))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(aggregatedPerformance));

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioAggregatedPerformance(portfolioId, dateRange, aggregationFrequency);

            portfolioService.Verify(m => m.ChartPortfolioAggregatedPerformanceAsync(portfolioId, dateRange, aggregationFrequency));
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

            var portfolioService = fixture.Freeze<Mock<IPortfolioService>>();
            portfolioService
                .Setup(m => m.ChartPortfolioAggregatedPerformanceAsync(portfolioId, dateRange, aggregationFrequency))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<IEnumerable<EntityChartPointDto>>());

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioAggregatedPerformance(portfolioId, dateRange, aggregationFrequency);

            portfolioService.Verify(m => m.ChartPortfolioAggregatedPerformanceAsync(portfolioId, dateRange, aggregationFrequency));
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

            var portfolioService = fixture.Freeze<Mock<IPortfolioService>>();
            portfolioService
                .Setup(m => m.ChartPortfolioAggregatedProfitAsync(portfolioId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(aggregatedProfit));

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioAggregatedProfit(portfolioId, dateRange, aggregationFrequency, currencyCode);

            portfolioService.Verify(m => m.ChartPortfolioAggregatedProfitAsync(portfolioId, dateRange, aggregationFrequency, currencyCode));
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

            var portfolioService = fixture.Freeze<Mock<IPortfolioService>>();
            portfolioService
                .Setup(m => m.ChartPortfolioAggregatedProfitAsync(portfolioId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<IEnumerable<EntityChartPointDto>>());

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioAggregatedProfit(portfolioId, dateRange, aggregationFrequency, currencyCode);

            portfolioService.Verify(m => m.ChartPortfolioAggregatedProfitAsync(portfolioId, dateRange, aggregationFrequency, currencyCode));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetAllPortfoliosStatistics_ReturnsStatistics()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var statistics = fixture.CreateMany<EntityStatisticsDto>();

            var portfolioService = fixture.Freeze<Mock<IPortfolioService>>();
            portfolioService
                .Setup(m => m.GetAllPortfoliosStatisticsAsync())
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(statistics));

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetAllPortfoliosStatistics();

            portfolioService.Verify(m => m.GetAllPortfoliosStatisticsAsync(), Times.Once());
            Assert.Equal(statistics, result.Value);
        }

        [Fact]
        public async Task GetPortfolioStatistics_ReturnsPortfolioStatistics_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();
            var statistics = fixture.Create<EntityStatisticsDto>();

            var portfolioService = fixture.Freeze<Mock<IPortfolioService>>();
            portfolioService
                .Setup(m => m.GetPortfolioStatisticsAsync(portfolioId))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(statistics));

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioStatistics(portfolioId);

            portfolioService.Verify(m => m.GetPortfolioStatisticsAsync(portfolioId), Times.Once());
            Assert.Equal(statistics, result.Value);
        }

        [Fact]
        public async Task GetPortfolioStatistics_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();

            var portfolioService = fixture.Freeze<Mock<IPortfolioService>>();
            portfolioService
                .Setup(m => m.GetPortfolioStatisticsAsync(portfolioId))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<EntityStatisticsDto>());

            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfolioStatistics(portfolioId);

            portfolioService.Verify(m => m.GetPortfolioStatisticsAsync(portfolioId), Times.Once());
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
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(portfolio));
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
            portfolioService
                .Setup(m => m.UpdatePortfolioAsync(portfolio))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(portfolio));

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
            portfolioService
                .Setup(m => m.DeletePortfolioAsync(portfolioId))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse());
            var sut = fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

            await sut.DeletePortfolio(portfolioId);

            portfolioService.Verify(m => m.DeletePortfolioAsync(portfolioId), Times.Once());
        }
    }
}
