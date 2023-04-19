using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Tests.Unit.Helpers;
using Xunit;

namespace PortEval.Tests.Unit.ControllerTests;

public class ChartsControllerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IChartService> _chartService;

    public ChartsControllerTests()
    {
        _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
        _chartService = _fixture.Freeze<Mock<IChartService>>();
    }

    [Fact]
    public async Task GetAllCharts_ReturnsCharts()
    {
        var charts = _fixture.CreateMany<ChartDto>();

        _chartService
            .Setup(m => m.GetAllChartsAsync())
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(charts));
        var sut = _fixture.Build<ChartsController>().OmitAutoProperties().Create();

        var result = await sut.GetAllCharts();

        _chartService.Verify(m => m.GetAllChartsAsync(), Times.Once());
        Assert.Equal(charts, result.Value);
    }

    [Fact]
    public async Task GetChart_ReturnsCorrectChart_WhenChartExists()
    {
        var chart = _fixture.Create<ChartDto>();

        _chartService
            .Setup(m => m.GetChartAsync(chart.Id))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(chart));

        var sut = _fixture.Build<ChartsController>().OmitAutoProperties().Create();

        var result = await sut.GetChart(chart.Id);

        _chartService.Verify(m => m.GetChartAsync(chart.Id), Times.Once());
        Assert.Equal(chart, result.Value);
    }

    [Fact]
    public async Task GetChart_ReturnsNotFound_WhenChartDoesNotExist()
    {
        var chartId = _fixture.Create<int>();

        _chartService
            .Setup(m => m.GetChartAsync(It.IsAny<int>()))
            .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<ChartDto>());

        var sut = _fixture.Build<ChartsController>().OmitAutoProperties().Create();

        var result = await sut.GetChart(chartId);

        _chartService.Verify(m => m.GetChartAsync(chartId), Times.Once());
        Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task PostChart_CreatesChart()
    {
        var chart = _fixture.Create<ChartDto>();

        _chartService
            .Setup(m => m.CreateChartAsync(chart))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(_fixture.Create<ChartDto>()));
        var sut = _fixture.Build<ChartsController>().OmitAutoProperties().Create();

        await sut.PostChart(chart);

        _chartService.Verify(m => m.CreateChartAsync(chart), Times.Once());
    }

    [Fact]
    public async Task PutChart_UpdatesChart()
    {
        var chart = _fixture.Create<ChartDto>();

        _chartService
            .Setup(m => m.UpdateChartAsync(chart))
            .Returns<ChartDto>(c =>
                Task.FromResult(OperationResponseHelper.GenerateSuccessfulOperationResponse(c)));
        var sut = _fixture.Build<ChartsController>().OmitAutoProperties().Create();

        await sut.PutChart(chart.Id, chart);

        _chartService.Verify(m => m.UpdateChartAsync(chart), Times.Once());
    }

    [Fact]
    public async Task PutChart_ReturnsBadRequest_WhenQueryParameterIdAndBodyIdDontMatch()
    {
        var chart = _fixture.Create<ChartDto>();

        var sut = _fixture.Build<ChartsController>().OmitAutoProperties().Create();

        var result = await sut.PutChart(chart.Id + 1, chart);

        _chartService.Verify(m => m.UpdateChartAsync(chart), Times.Never());
        Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task DeleteChart_DeletesChart()
    {
        var chartId = _fixture.Create<int>();

        _chartService
            .Setup(m => m.DeleteChartAsync(It.IsAny<int>()))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse());
        var sut = _fixture.Build<ChartsController>().OmitAutoProperties().Create();

        await sut.DeleteChart(chartId);

        _chartService.Verify(m => m.DeleteChartAsync(chartId), Times.Once());
    }
}