using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;
using PortEval.Tests.Unit.Helpers;
using Xunit;

namespace PortEval.Tests.Unit.ControllerTests;

public class PortfolioControllerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IPortfolioService> _portfolioService;
    private readonly Mock<IPositionService> _positionService;

    public PortfolioControllerTests()
    {
        _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
        _portfolioService = _fixture.Freeze<Mock<IPortfolioService>>();
        _positionService = _fixture.Freeze<Mock<IPositionService>>();
    }

    [Fact]
    public async Task GetPortfolios_ReturnsPortfolios()
    {
        var portfolios = _fixture.CreateMany<PortfolioDto>();

        _portfolioService
            .Setup(m => m.GetAllPortfoliosAsync())
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(portfolios));
        var sut = _fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

        var result = await sut.GetPortfolios();

        _portfolioService.Verify(m => m.GetAllPortfoliosAsync(), Times.Once());
        Assert.Equal(portfolios, result.Value);
    }

    [Fact]
    public async Task GetPortfolio_ReturnsCorrectPortfolio_WhenPortfolioExists()
    {
        var portfolio = _fixture.Create<PortfolioDto>();

        _portfolioService
            .Setup(m => m.GetPortfolioAsync(portfolio.Id))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(portfolio));

        var sut = _fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

        var result = await sut.GetPortfolio(portfolio.Id);

        _portfolioService.Verify(m => m.GetPortfolioAsync(portfolio.Id), Times.Once());
        Assert.Equal(portfolio, result.Value);
    }

    [Fact]
    public async Task GetPortfolio_ReturnsNotFound_WhenPortfolioDoesNotExist()
    {
        var portfolioId = _fixture.Create<int>();

        _portfolioService
            .Setup(m => m.GetPortfolioAsync(It.IsAny<int>()))
            .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<PortfolioDto>());

        var sut = _fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

        var result = await sut.GetPortfolio(portfolioId);

        _portfolioService.Verify(m => m.GetPortfolioAsync(portfolioId), Times.Once());
        Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetPositions_ReturnsPositions_WhenPortfolioExists()
    {
        var portfolioId = _fixture.Create<int>();
        var positions = _fixture.CreateMany<PositionDto>();

        _positionService
            .Setup(m => m.GetPortfolioPositionsAsync(portfolioId))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(positions));

        var sut = _fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

        var result = await sut.GetPositions(portfolioId);

        _positionService.Verify(m => m.GetPortfolioPositionsAsync(portfolioId));
        Assert.Equal(positions, result.Value);
    }

    [Fact]
    public async Task GetPositions_ReturnsNotFound_WhenPortfolioDoesNotExist()
    {
        var portfolioId = _fixture.Create<int>();

        _positionService
            .Setup(m => m.GetPortfolioPositionsAsync(portfolioId))
            .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<IEnumerable<PositionDto>>());

        var sut = _fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

        var result = await sut.GetPositions(portfolioId);

        _positionService.Verify(m => m.GetPortfolioPositionsAsync(portfolioId));
        Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetPositionsStatistics_ReturnsPortfolioPositionsStatistics_WhenPortfolioExists()
    {
        var portfolioId = _fixture.Create<int>();
        var statistics = _fixture.CreateMany<PositionStatisticsDto>();

        _positionService
            .Setup(m => m.GetPortfolioPositionsStatisticsAsync(portfolioId))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(statistics));

        var sut = _fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

        var result = await sut.GetPositionsStatistics(portfolioId);

        _positionService.Verify(m => m.GetPortfolioPositionsStatisticsAsync(portfolioId));
        Assert.Equal(statistics, result.Value);
    }

    [Fact]
    public async Task GetPositionStatistics_ReturnsNotFound_WhenPortfolioDoesNotExist()
    {
        var portfolioId = _fixture.Create<int>();

        _positionService
            .Setup(m => m.GetPortfolioPositionsStatisticsAsync(portfolioId))
            .ReturnsAsync(
                OperationResponseHelper.GenerateNotFoundOperationResponse<IEnumerable<PositionStatisticsDto>>());

        var sut = _fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

        var result = await sut.GetPositionsStatistics(portfolioId);

        _positionService.Verify(m => m.GetPortfolioPositionsStatisticsAsync(portfolioId));
        Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetPortfolioValue_ReturnsPortfolioValue_WhenPortfolioExists()
    {
        var portfolioId = _fixture.Create<int>();
        var value = _fixture.Create<EntityValueDto>();

        _portfolioService
            .Setup(m => m.GetPortfolioValueAsync(portfolioId, value.Time))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(value));

        var sut = _fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

        var result = await sut.GetPortfolioValue(portfolioId, value.Time);

        _portfolioService.Verify(m => m.GetPortfolioValueAsync(portfolioId, value.Time));
        Assert.Equal(value, result.Value);
    }

    [Fact]
    public async Task GetPortfolioValue_ReturnsCurrentPortfolioValue_WhenTimeParameterIsNotProvided()
    {
        var now = DateTime.UtcNow;
        var portfolioId = _fixture.Create<int>();
        var value = _fixture.Build<EntityValueDto>().With(v => v.Time, now).Create();

        _portfolioService
            .Setup(m => m.GetPortfolioValueAsync(portfolioId, It.Is<DateTime>(d => d >= now)))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(value));

        var sut = _fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

        var result = await sut.GetPortfolioValue(portfolioId, null);

        _portfolioService.Verify(m => m.GetPortfolioValueAsync(portfolioId, It.Is<DateTime>(d => d >= now)));
        Assert.Equal(value, result.Value);
    }

    [Fact]
    public async Task GetPortfolioValue_ReturnsNotFound_WhenPortfolioDoesNotExist()
    {
        var portfolioId = _fixture.Create<int>();

        _portfolioService
            .Setup(m => m.GetPortfolioValueAsync(portfolioId, It.IsAny<DateTime>()))
            .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<EntityValueDto>());

        var sut = _fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

        var result = await sut.GetPortfolioValue(portfolioId, DateTime.UtcNow);

        _portfolioService.Verify(m => m.GetPortfolioValueAsync(portfolioId, It.IsAny<DateTime>()));
        Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetPortfolioProfit_ReturnsProfit_WhenPortfolioExists()
    {
        var portfolioId = _fixture.Create<int>();
        var profit = _fixture.Create<EntityProfitDto>();
        var dateRange = _fixture.Create<DateRangeParams>();

        _portfolioService
            .Setup(m => m.GetPortfolioProfitAsync(portfolioId, dateRange))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(profit));

        var sut = _fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

        var result = await sut.GetPortfolioProfit(portfolioId, dateRange);

        _portfolioService.Verify(m => m.GetPortfolioProfitAsync(portfolioId, dateRange));
        Assert.Equal(profit, result.Value);
    }

    [Fact]
    public async Task GetPortfolioProfit_ReturnsNotFound_WhenPortfolioDoesNotExist()
    {
        var portfolioId = _fixture.Create<int>();
        var dateRange = _fixture.Create<DateRangeParams>();

        _portfolioService
            .Setup(m => m.GetPortfolioProfitAsync(portfolioId, dateRange))
            .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<EntityProfitDto>());

        var sut = _fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

        var result = await sut.GetPortfolioProfit(portfolioId, dateRange);

        _portfolioService.Verify(m => m.GetPortfolioProfitAsync(portfolioId, dateRange));
        Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetPortfolioPerformance_ReturnsPortfolioPerformance_WhenPortfolioExists()
    {
        var portfolioId = _fixture.Create<int>();
        var performance = _fixture.Create<EntityPerformanceDto>();
        var dateRange = _fixture.Create<DateRangeParams>();

        _portfolioService
            .Setup(m => m.GetPortfolioPerformanceAsync(portfolioId, dateRange))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(performance));

        var sut = _fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

        var result = await sut.GetPortfolioPerformance(portfolioId, dateRange);

        _portfolioService.Verify(m => m.GetPortfolioPerformanceAsync(portfolioId, dateRange));
        Assert.Equal(performance, result.Value);
    }

    [Fact]
    public async Task GetPortfolioPerformance_ReturnsNotFound_WhenPortfolioDoesNotExist()
    {
        var portfolioId = _fixture.Create<int>();
        var dateRange = _fixture.Create<DateRangeParams>();

        _portfolioService
            .Setup(m => m.GetPortfolioPerformanceAsync(portfolioId, dateRange))
            .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<EntityPerformanceDto>());

        var sut = _fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

        var result = await sut.GetPortfolioPerformance(portfolioId, dateRange);

        _portfolioService.Verify(m => m.GetPortfolioPerformanceAsync(portfolioId, dateRange));
        Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetPortfolioChartedValue_ReturnsChartedValue_WhenPortfolioExists()
    {
        var portfolioId = _fixture.Create<int>();
        var chartedValue = _fixture.CreateMany<EntityChartPointDto>();
        var dateRange = _fixture.Create<DateRangeParams>();
        var aggregationFrequency = _fixture.Create<AggregationFrequency>();
        var currencyCode = _fixture.Create<string>();

        _portfolioService
            .Setup(m => m.ChartPortfolioValueAsync(portfolioId, dateRange, aggregationFrequency, currencyCode))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(chartedValue));

        var sut = _fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

        var result = await sut.GetPortfolioChartedValue(portfolioId, dateRange, aggregationFrequency, currencyCode);

        _portfolioService.Verify(m =>
            m.ChartPortfolioValueAsync(portfolioId, dateRange, aggregationFrequency, currencyCode));
        Assert.Equal(chartedValue, result.Value);
    }

    [Fact]
    public async Task GetPortfolioChartedValue_ReturnsNotFound_WhenPortfolioDoesNotExist()
    {
        var portfolioId = _fixture.Create<int>();
        var dateRange = _fixture.Create<DateRangeParams>();
        var aggregationFrequency = _fixture.Create<AggregationFrequency>();
        var currencyCode = _fixture.Create<string>();

        _portfolioService
            .Setup(m => m.ChartPortfolioValueAsync(portfolioId, dateRange, aggregationFrequency, currencyCode))
            .ReturnsAsync(OperationResponseHelper
                .GenerateNotFoundOperationResponse<IEnumerable<EntityChartPointDto>>());

        var sut = _fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

        var result = await sut.GetPortfolioChartedValue(portfolioId, dateRange, aggregationFrequency, currencyCode);

        _portfolioService.Verify(m =>
            m.ChartPortfolioValueAsync(portfolioId, dateRange, aggregationFrequency, currencyCode));
        Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetPortfolioChartedProfit_ReturnsChartedProfit_WhenPortfolioExists()
    {
        var portfolioId = _fixture.Create<int>();
        var chartedProfit = _fixture.CreateMany<EntityChartPointDto>();
        var dateRange = _fixture.Create<DateRangeParams>();
        var aggregationFrequency = _fixture.Create<AggregationFrequency>();
        var currencyCode = _fixture.Create<string>();

        _portfolioService
            .Setup(m => m.ChartPortfolioProfitAsync(portfolioId, dateRange, aggregationFrequency, currencyCode))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(chartedProfit));

        var sut = _fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

        var result = await sut.GetPortfolioChartedProfit(portfolioId, dateRange, aggregationFrequency, currencyCode);

        _portfolioService.Verify(m =>
            m.ChartPortfolioProfitAsync(portfolioId, dateRange, aggregationFrequency, currencyCode));
        Assert.Equal(chartedProfit, result.Value);
    }

    [Fact]
    public async Task GetPortfolioChartedProfit_ReturnsNotFound_WhenPortfolioDoesNotExist()
    {
        var portfolioId = _fixture.Create<int>();
        var dateRange = _fixture.Create<DateRangeParams>();
        var aggregationFrequency = _fixture.Create<AggregationFrequency>();
        var currencyCode = _fixture.Create<string>();

        _portfolioService
            .Setup(m => m.ChartPortfolioProfitAsync(portfolioId, dateRange, aggregationFrequency, currencyCode))
            .ReturnsAsync(OperationResponseHelper
                .GenerateNotFoundOperationResponse<IEnumerable<EntityChartPointDto>>());

        var sut = _fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

        var result = await sut.GetPortfolioChartedProfit(portfolioId, dateRange, aggregationFrequency, currencyCode);

        _portfolioService.Verify(m =>
            m.ChartPortfolioProfitAsync(portfolioId, dateRange, aggregationFrequency, currencyCode));
        Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetPortfolioChartedPerformance_ReturnsChartedPerformance_WhenPortfolioExists()
    {
        var portfolioId = _fixture.Create<int>();
        var chartedPerformance = _fixture.CreateMany<EntityChartPointDto>();
        var dateRange = _fixture.Create<DateRangeParams>();
        var aggregationFrequency = _fixture.Create<AggregationFrequency>();

        _portfolioService
            .Setup(m => m.ChartPortfolioPerformanceAsync(portfolioId, dateRange, aggregationFrequency))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(chartedPerformance));

        var sut = _fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

        var result = await sut.GetPortfolioChartedPerformance(portfolioId, dateRange, aggregationFrequency);

        _portfolioService.Verify(m => m.ChartPortfolioPerformanceAsync(portfolioId, dateRange, aggregationFrequency));
        Assert.Equal(chartedPerformance, result.Value);
    }

    [Fact]
    public async Task GetPortfolioChartedPerformance_ReturnsNotFound_WhenPortfolioDoesNotExist()
    {
        var portfolioId = _fixture.Create<int>();
        var dateRange = _fixture.Create<DateRangeParams>();
        var aggregationFrequency = _fixture.Create<AggregationFrequency>();

        _portfolioService
            .Setup(m => m.ChartPortfolioPerformanceAsync(portfolioId, dateRange, aggregationFrequency))
            .ReturnsAsync(OperationResponseHelper
                .GenerateNotFoundOperationResponse<IEnumerable<EntityChartPointDto>>());

        var sut = _fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

        var result = await sut.GetPortfolioChartedPerformance(portfolioId, dateRange, aggregationFrequency);

        _portfolioService.Verify(m => m.ChartPortfolioPerformanceAsync(portfolioId, dateRange, aggregationFrequency));
        Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetPortfolioAggregatedPerformance_ReturnsAggregatedPerformance_WhenPortfolioExists()
    {
        var portfolioId = _fixture.Create<int>();
        var aggregatedPerformance = _fixture.CreateMany<EntityChartPointDto>();
        var dateRange = _fixture.Create<DateRangeParams>();
        var aggregationFrequency = _fixture.Create<AggregationFrequency>();

        _portfolioService
            .Setup(m => m.ChartPortfolioAggregatedPerformanceAsync(portfolioId, dateRange, aggregationFrequency))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(aggregatedPerformance));

        var sut = _fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

        var result = await sut.GetPortfolioAggregatedPerformance(portfolioId, dateRange, aggregationFrequency);

        _portfolioService.Verify(m =>
            m.ChartPortfolioAggregatedPerformanceAsync(portfolioId, dateRange, aggregationFrequency));
        Assert.Equal(aggregatedPerformance, result.Value);
    }

    [Fact]
    public async Task GetPortfolioAggregatedPerformance_ReturnsNotFound_WhenPortfolioDoesNotExist()
    {
        var portfolioId = _fixture.Create<int>();
        var dateRange = _fixture.Create<DateRangeParams>();
        var aggregationFrequency = _fixture.Create<AggregationFrequency>();

        _portfolioService
            .Setup(m => m.ChartPortfolioAggregatedPerformanceAsync(portfolioId, dateRange, aggregationFrequency))
            .ReturnsAsync(OperationResponseHelper
                .GenerateNotFoundOperationResponse<IEnumerable<EntityChartPointDto>>());

        var sut = _fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

        var result = await sut.GetPortfolioAggregatedPerformance(portfolioId, dateRange, aggregationFrequency);

        _portfolioService.Verify(m =>
            m.ChartPortfolioAggregatedPerformanceAsync(portfolioId, dateRange, aggregationFrequency));
        Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetPortfolioAggregatedProfit_ReturnsAggregatedProfit_WhenPortfolioExists()
    {
        var portfolioId = _fixture.Create<int>();
        var aggregatedProfit = _fixture.CreateMany<EntityChartPointDto>();
        var dateRange = _fixture.Create<DateRangeParams>();
        var aggregationFrequency = _fixture.Create<AggregationFrequency>();
        var currencyCode = _fixture.Create<string>();

        _portfolioService
            .Setup(m => m.ChartPortfolioAggregatedProfitAsync(portfolioId, dateRange, aggregationFrequency,
                currencyCode))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(aggregatedProfit));

        var sut = _fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

        var result = await sut.GetPortfolioAggregatedProfit(portfolioId, dateRange, aggregationFrequency, currencyCode);

        _portfolioService.Verify(m =>
            m.ChartPortfolioAggregatedProfitAsync(portfolioId, dateRange, aggregationFrequency, currencyCode));
        Assert.Equal(aggregatedProfit, result.Value);
    }

    [Fact]
    public async Task GetPortfolioAggregatedProfit_ReturnsNotFound_WhenPortfolioDoesNotExist()
    {
        var portfolioId = _fixture.Create<int>();
        var dateRange = _fixture.Create<DateRangeParams>();
        var aggregationFrequency = _fixture.Create<AggregationFrequency>();
        var currencyCode = _fixture.Create<string>();

        _portfolioService
            .Setup(m => m.ChartPortfolioAggregatedProfitAsync(portfolioId, dateRange, aggregationFrequency,
                currencyCode))
            .ReturnsAsync(OperationResponseHelper
                .GenerateNotFoundOperationResponse<IEnumerable<EntityChartPointDto>>());

        var sut = _fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

        var result = await sut.GetPortfolioAggregatedProfit(portfolioId, dateRange, aggregationFrequency, currencyCode);

        _portfolioService.Verify(m =>
            m.ChartPortfolioAggregatedProfitAsync(portfolioId, dateRange, aggregationFrequency, currencyCode));
        Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetAllPortfoliosStatistics_ReturnsStatistics()
    {
        var statistics = _fixture.CreateMany<EntityStatisticsDto>();

        _portfolioService
            .Setup(m => m.GetAllPortfoliosStatisticsAsync())
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(statistics));

        var sut = _fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

        var result = await sut.GetAllPortfoliosStatistics();

        _portfolioService.Verify(m => m.GetAllPortfoliosStatisticsAsync(), Times.Once());
        Assert.Equal(statistics, result.Value);
    }

    [Fact]
    public async Task GetPortfolioStatistics_ReturnsPortfolioStatistics_WhenPortfolioExists()
    {
        var portfolioId = _fixture.Create<int>();
        var statistics = _fixture.Create<EntityStatisticsDto>();

        _portfolioService
            .Setup(m => m.GetPortfolioStatisticsAsync(portfolioId))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(statistics));

        var sut = _fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

        var result = await sut.GetPortfolioStatistics(portfolioId);

        _portfolioService.Verify(m => m.GetPortfolioStatisticsAsync(portfolioId), Times.Once());
        Assert.Equal(statistics, result.Value);
    }

    [Fact]
    public async Task GetPortfolioStatistics_ReturnsNotFound_WhenPortfolioDoesNotExist()
    {
        var portfolioId = _fixture.Create<int>();

        _portfolioService
            .Setup(m => m.GetPortfolioStatisticsAsync(portfolioId))
            .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<EntityStatisticsDto>());

        var sut = _fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

        var result = await sut.GetPortfolioStatistics(portfolioId);

        _portfolioService.Verify(m => m.GetPortfolioStatisticsAsync(portfolioId), Times.Once());
        Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task PostPortfolio_CreatesPortfolio()
    {
        var portfolio = _fixture.Create<PortfolioDto>();

        _portfolioService
            .Setup(m => m.CreatePortfolioAsync(portfolio))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(portfolio));
        var sut = _fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

        await sut.PostPortfolio(portfolio);

        _portfolioService.Verify(m => m.CreatePortfolioAsync(portfolio), Times.Once());
    }

    [Fact]
    public async Task PutPortfolio_UpdatesPortfolio()
    {
        var portfolio = _fixture.Create<PortfolioDto>();

        _portfolioService
            .Setup(m => m.UpdatePortfolioAsync(portfolio))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(portfolio));

        var sut = _fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

        await sut.PutPortfolio(portfolio.Id, portfolio);

        _portfolioService.Verify(m => m.UpdatePortfolioAsync(portfolio), Times.Once());
    }

    [Fact]
    public async Task PutPortfolio_ReturnsBadRequest_WhenQueryParameterIdAndBodyIdDontMatch()
    {
        var portfolio = _fixture.Create<PortfolioDto>();

        var sut = _fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

        var result = await sut.PutPortfolio(portfolio.Id + 1, portfolio);

        _portfolioService.Verify(m => m.UpdatePortfolioAsync(portfolio), Times.Never());
        Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task DeletePortfolio_DeletesPortfolio()
    {
        var portfolioId = _fixture.Create<int>();

        _portfolioService
            .Setup(m => m.DeletePortfolioAsync(portfolioId))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse());
        var sut = _fixture.Build<PortfoliosController>().OmitAutoProperties().Create();

        await sut.DeletePortfolio(portfolioId);

        _portfolioService.Verify(m => m.DeletePortfolioAsync(portfolioId), Times.Once());
    }
}