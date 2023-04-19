using System;
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

public class InstrumentPricesControllerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IInstrumentPriceService> _instrumentPriceService;

    public InstrumentPricesControllerTests()
    {
        _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
        _instrumentPriceService = _fixture.Freeze<Mock<IInstrumentPriceService>>();
    }

    [Fact]
    public async Task GetInstrumentPriceAt_ReturnsInstrumentPrice_WhenPriceExists()
    {
        var instrumentId = _fixture.Create<int>();
        var price = _fixture.Create<InstrumentPriceDto>();

        _instrumentPriceService
            .Setup(m => m.GetInstrumentPriceAsync(instrumentId, price.Time))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(price));

        var sut = _fixture.Build<InstrumentPricesController>().OmitAutoProperties().Create();

        var result = await sut.GetInstrumentPriceAt(instrumentId, price.Time);

        _instrumentPriceService.Verify(m => m.GetInstrumentPriceAsync(instrumentId, price.Time));
        Assert.Equal(price, result.Value);
    }

    [Fact]
    public async Task GetInstrumentPriceAt_ReturnsNotFound_WhenQueryReturnsNotFound()
    {
        var instrumentId = _fixture.Create<int>();

        _instrumentPriceService
            .Setup(m => m.GetInstrumentPriceAsync(instrumentId, It.IsAny<DateTime>()))
            .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<InstrumentPriceDto>());

        var sut = _fixture.Build<InstrumentPricesController>().OmitAutoProperties().Create();

        var result = await sut.GetInstrumentPriceAt(instrumentId, DateTime.UtcNow);

        _instrumentPriceService.Verify(m => m.GetInstrumentPriceAsync(instrumentId, It.IsAny<DateTime>()));
        Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetLatestInstrumentPrice_ReturnsCurrentPrice_WhenPriceExists()
    {
        var instrumentId = _fixture.Create<int>();
        var now = DateTime.UtcNow;
        var price = _fixture.Build<InstrumentPriceDto>().With(p => p.Time, now).Create();

        _instrumentPriceService
            .Setup(m => m.GetInstrumentPriceAsync(instrumentId, It.Is<DateTime>(dt => dt >= now)))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(price));

        var sut = _fixture.Build<InstrumentPricesController>().OmitAutoProperties().Create();

        var result = await sut.GetLatestInstrumentPrice(instrumentId);

        _instrumentPriceService.Verify(m => m.GetInstrumentPriceAsync(instrumentId, It.Is<DateTime>(dt => dt >= now)));
        Assert.Equal(price, result.Value);
    }

    [Fact]
    public async Task GetLatestInstrumentPrice_ReturnsNotFound_WhenPriceDoesNotExist()
    {
        var instrumentId = _fixture.Create<int>();

        _instrumentPriceService
            .Setup(m => m.GetInstrumentPriceAsync(instrumentId, It.IsAny<DateTime>()))
            .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<InstrumentPriceDto>());

        var sut = _fixture.Build<InstrumentPricesController>().OmitAutoProperties().Create();

        var result = await sut.GetLatestInstrumentPrice(instrumentId);

        _instrumentPriceService.Verify(m => m.GetInstrumentPriceAsync(instrumentId, It.IsAny<DateTime>()));
        Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task PostPricePoint_CreatesPrice()
    {
        var price = _fixture.Build<InstrumentPriceDto>()
            .With(p => p.Time, DateTime.UtcNow)
            .Create();

        _instrumentPriceService
            .Setup(m => m.AddPricePointAsync(price))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(price));

        var sut = _fixture.Build<InstrumentPricesController>().OmitAutoProperties().Create();

        await sut.PostPricePoint(price.InstrumentId, price);

        _instrumentPriceService.Verify(m => m.AddPricePointAsync(price));
    }

    [Fact]
    public async Task PostPricePoint_ReturnsBadRequest_WhenQueryParamInstrumentIdAndBodyInstrumentIdDontMatch()
    {
        var price = _fixture.Create<InstrumentPriceDto>();

        _instrumentPriceService
            .Setup(m => m.AddPricePointAsync(price))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(price));

        var sut = _fixture.Build<InstrumentPricesController>().OmitAutoProperties().Create();

        var result = await sut.PostPricePoint(price.InstrumentId + 1, price);

        _instrumentPriceService.Verify(m => m.AddPricePointAsync(price), Times.Never());
        Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task DeletePricePoint_DeletesPrice()
    {
        var instrumentId = _fixture.Create<int>();
        var priceId = _fixture.Create<int>();

        _instrumentPriceService
            .Setup(m => m.DeletePricePointByIdAsync(instrumentId, priceId))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse());

        var sut = _fixture.Build<InstrumentPricesController>().OmitAutoProperties().Create();

        await sut.DeletePricePoint(instrumentId, priceId);

        _instrumentPriceService.Verify(m => m.DeletePricePointByIdAsync(instrumentId, priceId));
    }
}