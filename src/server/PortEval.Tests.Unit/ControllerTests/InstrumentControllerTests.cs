using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;
using PortEval.Tests.Unit.Helpers;
using Xunit;

namespace PortEval.Tests.Unit.ControllerTests;

public class InstrumentControllerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IInstrumentService> _instrumentService;

    public InstrumentControllerTests()
    {
        _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
        _instrumentService = _fixture.Freeze<Mock<IInstrumentService>>();
    }

    [Fact]
    public async Task GetAllInstruments_ReturnsInstrumentsPage()
    {
        var instrumentsPage = _fixture.Create<PaginatedResponse<InstrumentDto>>();
        var pagination = _fixture.Create<PaginationParams>();

        _instrumentService
            .Setup(m => m.GetInstrumentsPageAsync(pagination))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(instrumentsPage));
        var sut = _fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

        var result = await sut.GetAllInstruments(pagination);

        _instrumentService.Verify(m => m.GetInstrumentsPageAsync(pagination), Times.Once());
        Assert.Equal(instrumentsPage, result.Value);
    }

    [Fact]
    public async Task GetInstrument_ReturnsCorrectInstrument_WhenInstrumentExists()
    {
        var instrument = _fixture.Create<InstrumentDto>();

        _instrumentService
            .Setup(m => m.GetInstrumentAsync(instrument.Id))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(instrument));

        var sut = _fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

        var result = await sut.GetInstrument(instrument.Id);

        _instrumentService.Verify(m => m.GetInstrumentAsync(instrument.Id), Times.Once());
        Assert.Equal(instrument, result.Value);
    }

    [Fact]
    public async Task GetInstrument_ReturnsNotFound_WhenInstrumentDoesNotExist()
    {
        var instrumentId = _fixture.Create<int>();

        _instrumentService
            .Setup(m => m.GetInstrumentAsync(It.IsAny<int>()))
            .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<InstrumentDto>());

        var sut = _fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

        var result = await sut.GetInstrument(instrumentId);

        _instrumentService.Verify(m => m.GetInstrumentAsync(instrumentId), Times.Once());
        Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetInstrumentProfit_ReturnsProfit_WhenInstrumentExists()
    {
        var instrumentId = _fixture.Create<int>();
        var profit = _fixture.Create<EntityProfitDto>();
        var dateRange = _fixture.Create<DateRangeParams>();

        _instrumentService
            .Setup(m => m.GetInstrumentProfitAsync(instrumentId, dateRange))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(profit));

        var sut = _fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

        var result = await sut.GetInstrumentProfit(instrumentId, dateRange);

        _instrumentService.Verify(m => m.GetInstrumentProfitAsync(instrumentId, dateRange));
        Assert.Equal(profit, result.Value);
    }

    [Fact]
    public async Task GetInstrumentProfit_ReturnsNotFound_WhenInstrumentDoesNotExist()
    {
        var instrumentId = _fixture.Create<int>();
        var dateRange = _fixture.Create<DateRangeParams>();

        _instrumentService
            .Setup(m => m.GetInstrumentProfitAsync(instrumentId, dateRange))
            .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<EntityProfitDto>());

        var sut = _fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

        var result = await sut.GetInstrumentProfit(instrumentId, dateRange);

        _instrumentService.Verify(m => m.GetInstrumentProfitAsync(instrumentId, dateRange));
        Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetInstrumentPerformance_ReturnsInstrumentPerformance_WhenInstrumentExists()
    {
        var instrumentId = _fixture.Create<int>();
        var performance = _fixture.Create<EntityPerformanceDto>();
        var dateRange = _fixture.Create<DateRangeParams>();

        _instrumentService
            .Setup(m => m.GetInstrumentPerformanceAsync(instrumentId, dateRange))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(performance));

        var sut = _fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

        var result = await sut.GetInstrumentPerformance(instrumentId, dateRange);

        _instrumentService.Verify(m => m.GetInstrumentPerformanceAsync(instrumentId, dateRange));
        Assert.Equal(performance, result.Value);
    }

    [Fact]
    public async Task GetInstrumentPerformance_ReturnsNotFound_WhenInstrumentDoesNotExist()
    {
        var instrumentId = _fixture.Create<int>();
        var dateRange = _fixture.Create<DateRangeParams>();

        _instrumentService
            .Setup(m => m.GetInstrumentPerformanceAsync(instrumentId, dateRange))
            .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<EntityPerformanceDto>());

        var sut = _fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

        var result = await sut.GetInstrumentPerformance(instrumentId, dateRange);

        _instrumentService.Verify(m => m.GetInstrumentPerformanceAsync(instrumentId, dateRange));
        Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetInstrumentChartedPrices_ReturnsChartedPrices_WhenInstrumentExists()
    {
        var instrumentId = _fixture.Create<int>();
        var chartedPrices = _fixture.CreateMany<EntityChartPointDto>();
        var dateRange = _fixture.Create<DateRangeParams>();
        var aggregationFrequency = _fixture.Create<AggregationFrequency>();
        var currencyCode = _fixture.Create<string>();

        _instrumentService
            .Setup(m => m.ChartInstrumentPricesAsync(instrumentId, dateRange, aggregationFrequency, currencyCode))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(chartedPrices));

        var sut = _fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

        var result = await sut.GetInstrumentChartedPrices(instrumentId, dateRange, aggregationFrequency, currencyCode);

        _instrumentService.Verify(m =>
            m.ChartInstrumentPricesAsync(instrumentId, dateRange, aggregationFrequency, currencyCode));
        Assert.Equal(chartedPrices, result.Value);
    }

    [Fact]
    public async Task GetInstrumentChartedPrices_ReturnsNotFound_WhenInstrumentDoesNotExist()
    {
        var instrumentId = _fixture.Create<int>();
        var dateRange = _fixture.Create<DateRangeParams>();
        var aggregationFrequency = _fixture.Create<AggregationFrequency>();
        var currencyCode = _fixture.Create<string>();

        _instrumentService
            .Setup(m => m.ChartInstrumentPricesAsync(instrumentId, dateRange, aggregationFrequency, currencyCode))
            .ReturnsAsync(OperationResponseHelper
                .GenerateNotFoundOperationResponse<IEnumerable<EntityChartPointDto>>());

        var sut = _fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

        var result = await sut.GetInstrumentChartedPrices(instrumentId, dateRange, aggregationFrequency, currencyCode);

        _instrumentService.Verify(m =>
            m.ChartInstrumentPricesAsync(instrumentId, dateRange, aggregationFrequency, currencyCode));
        Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetInstrumentChartedProfit_ReturnsChartedProfit_WhenInstrumentExists()
    {
        var instrumentId = _fixture.Create<int>();
        var chartedProfit = _fixture.CreateMany<EntityChartPointDto>();
        var dateRange = _fixture.Create<DateRangeParams>();
        var aggregationFrequency = _fixture.Create<AggregationFrequency>();
        var currencyCode = _fixture.Create<string>();

        _instrumentService
            .Setup(m => m.ChartInstrumentProfitAsync(instrumentId, dateRange, aggregationFrequency, currencyCode))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(chartedProfit));

        var sut = _fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

        var result = await sut.GetInstrumentChartedProfit(instrumentId, dateRange, aggregationFrequency, currencyCode);

        _instrumentService.Verify(m =>
            m.ChartInstrumentProfitAsync(instrumentId, dateRange, aggregationFrequency, currencyCode));
        Assert.Equal(chartedProfit, result.Value);
    }

    [Fact]
    public async Task GetInstrumentChartedProfit_ReturnsNotFound_WhenInstrumentDoesNotExist()
    {
        var instrumentId = _fixture.Create<int>();
        var dateRange = _fixture.Create<DateRangeParams>();
        var aggregationFrequency = _fixture.Create<AggregationFrequency>();
        var currencyCode = _fixture.Create<string>();

        _instrumentService
            .Setup(m => m.ChartInstrumentProfitAsync(instrumentId, dateRange, aggregationFrequency, currencyCode))
            .ReturnsAsync(OperationResponseHelper
                .GenerateNotFoundOperationResponse<IEnumerable<EntityChartPointDto>>());

        var sut = _fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

        var result = await sut.GetInstrumentChartedProfit(instrumentId, dateRange, aggregationFrequency, currencyCode);

        _instrumentService.Verify(m =>
            m.ChartInstrumentProfitAsync(instrumentId, dateRange, aggregationFrequency, currencyCode));
        Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetInstrumentChartedPerformance_ReturnsChartedPerformance_WhenInstrumentExists()
    {
        var instrumentId = _fixture.Create<int>();
        var chartedPerformance = _fixture.CreateMany<EntityChartPointDto>();
        var dateRange = _fixture.Create<DateRangeParams>();
        var aggregationFrequency = _fixture.Create<AggregationFrequency>();

        _instrumentService
            .Setup(m => m.ChartInstrumentPerformanceAsync(instrumentId, dateRange, aggregationFrequency))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(chartedPerformance));

        var sut = _fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

        var result = await sut.GetInstrumentChartedPerformance(instrumentId, dateRange, aggregationFrequency);

        _instrumentService.Verify(m =>
            m.ChartInstrumentPerformanceAsync(instrumentId, dateRange, aggregationFrequency));
        Assert.Equal(chartedPerformance, result.Value);
    }

    [Fact]
    public async Task GetInstrumentChartedPerformance_ReturnsNotFound_WhenInstrumentDoesNotExist()
    {
        var instrumentId = _fixture.Create<int>();
        var dateRange = _fixture.Create<DateRangeParams>();
        var aggregationFrequency = _fixture.Create<AggregationFrequency>();

        _instrumentService
            .Setup(m => m.ChartInstrumentPerformanceAsync(instrumentId, dateRange, aggregationFrequency))
            .ReturnsAsync(OperationResponseHelper
                .GenerateNotFoundOperationResponse<IEnumerable<EntityChartPointDto>>());

        var sut = _fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

        var result = await sut.GetInstrumentChartedPerformance(instrumentId, dateRange, aggregationFrequency);

        _instrumentService.Verify(m =>
            m.ChartInstrumentPerformanceAsync(instrumentId, dateRange, aggregationFrequency));
        Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetInstrumentAggregatedPerformance_ReturnsAggregatedPerformance_WhenInstrumentExists()
    {
        var instrumentId = _fixture.Create<int>();
        var aggregatedPerformance = _fixture.CreateMany<EntityChartPointDto>();
        var dateRange = _fixture.Create<DateRangeParams>();
        var aggregationFrequency = _fixture.Create<AggregationFrequency>();

        _instrumentService
            .Setup(m => m.ChartInstrumentAggregatedPerformanceAsync(instrumentId, dateRange, aggregationFrequency))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(aggregatedPerformance));

        var sut = _fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

        var result = await sut.GetInstrumentAggregatedPerformance(instrumentId, dateRange, aggregationFrequency);

        _instrumentService.Verify(m =>
            m.ChartInstrumentAggregatedPerformanceAsync(instrumentId, dateRange, aggregationFrequency));
        Assert.Equal(aggregatedPerformance, result.Value);
    }

    [Fact]
    public async Task GetInstrumentAggregatedPerformance_ReturnsNotFound_WhenInstrumentDoesNotExist()
    {
        var instrumentId = _fixture.Create<int>();
        var dateRange = _fixture.Create<DateRangeParams>();
        var aggregationFrequency = _fixture.Create<AggregationFrequency>();

        _instrumentService
            .Setup(m => m.ChartInstrumentAggregatedPerformanceAsync(instrumentId, dateRange, aggregationFrequency))
            .ReturnsAsync(OperationResponseHelper
                .GenerateNotFoundOperationResponse<IEnumerable<EntityChartPointDto>>());

        var sut = _fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

        var result = await sut.GetInstrumentAggregatedPerformance(instrumentId, dateRange, aggregationFrequency);

        _instrumentService.Verify(m =>
            m.ChartInstrumentAggregatedPerformanceAsync(instrumentId, dateRange, aggregationFrequency));
        Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetInstrumentAggregatedProfit_ReturnsAggregatedProfit_WhenInstrumentExists()
    {
        var instrumentId = _fixture.Create<int>();
        var aggregatedProfit = _fixture.CreateMany<EntityChartPointDto>();
        var dateRange = _fixture.Create<DateRangeParams>();
        var aggregationFrequency = _fixture.Create<AggregationFrequency>();
        var currencyCode = _fixture.Create<string>();

        _instrumentService
            .Setup(m => m.ChartInstrumentAggregatedProfitAsync(instrumentId, dateRange, aggregationFrequency,
                currencyCode))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(aggregatedProfit));

        var sut = _fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

        var result =
            await sut.GetInstrumentAggregatedProfit(instrumentId, dateRange, aggregationFrequency, currencyCode);

        _instrumentService.Verify(m =>
            m.ChartInstrumentAggregatedProfitAsync(instrumentId, dateRange, aggregationFrequency, currencyCode));
        Assert.Equal(aggregatedProfit, result.Value);
    }

    [Fact]
    public async Task GetInstrumentAggregatedProfit_ReturnsNotFound_WhenInstrumentDoesNotExist()
    {
        var instrumentId = _fixture.Create<int>();
        var dateRange = _fixture.Create<DateRangeParams>();
        var aggregationFrequency = _fixture.Create<AggregationFrequency>();
        var currencyCode = _fixture.Create<string>();

        _instrumentService
            .Setup(m => m.ChartInstrumentAggregatedProfitAsync(instrumentId, dateRange, aggregationFrequency,
                currencyCode))
            .ReturnsAsync(OperationResponseHelper
                .GenerateNotFoundOperationResponse<IEnumerable<EntityChartPointDto>>());

        var sut = _fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

        var result =
            await sut.GetInstrumentAggregatedProfit(instrumentId, dateRange, aggregationFrequency, currencyCode);

        _instrumentService.Verify(m =>
            m.ChartInstrumentAggregatedProfitAsync(instrumentId, dateRange, aggregationFrequency, currencyCode));
        Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task PostInstrument_CreatesInstrument()
    {
        var instrument = _fixture.Create<InstrumentDto>();

        _instrumentService
            .Setup(m => m.CreateInstrumentAsync(instrument))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(instrument));
        var sut = _fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

        await sut.PostInstrument(instrument);

        _instrumentService.Verify(m => m.CreateInstrumentAsync(instrument), Times.Once());
    }

    [Fact]
    public async Task PutInstrument_UpdatesInstrument()
    {
        var instrument = _fixture.Create<InstrumentDto>();

        _instrumentService
            .Setup(m => m.UpdateInstrumentAsync(instrument))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(instrument));
        var sut = _fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

        await sut.PutInstrument(instrument.Id, instrument);

        _instrumentService.Verify(m => m.UpdateInstrumentAsync(instrument), Times.Once());
    }

    [Fact]
    public async Task PutInstrument_ReturnsBadRequest_WhenQueryParameterIdAndBodyIdDontMatch()
    {
        var instrument = _fixture.Create<InstrumentDto>();

        var sut = _fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

        var result = await sut.PutInstrument(instrument.Id + 1, instrument);

        _instrumentService.Verify(m => m.UpdateInstrumentAsync(instrument), Times.Never());
        Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task DeleteInstrument_DeletesInstrument()
    {
        var instrumentId = _fixture.Create<int>();

        _instrumentService
            .Setup(m => m.DeleteAsync(It.IsAny<int>()))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse());
        var sut = _fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

        await sut.DeleteInstrument(instrumentId);

        _instrumentService.Verify(m => m.DeleteAsync(instrumentId), Times.Once());
    }
}