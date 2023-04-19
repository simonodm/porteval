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

public class CurrenciesControllerTests
{
    private readonly Mock<ICurrencyService> _currencyService;
    private readonly IFixture _fixture;

    public CurrenciesControllerTests()
    {
        _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
        _currencyService = _fixture.Freeze<Mock<ICurrencyService>>();
    }

    [Fact]
    public async Task GetAllCurrenciesAsync_ReturnsCurrencies()
    {
        var currencies = _fixture.CreateMany<CurrencyDto>();

        _currencyService
            .Setup(m => m.GetAllCurrenciesAsync())
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(currencies));
        var sut = _fixture.Build<CurrenciesController>().OmitAutoProperties().Create();

        var result = await sut.GetAllCurrencies();

        _currencyService.Verify(m => m.GetAllCurrenciesAsync(), Times.Once());
        Assert.Equal(currencies, result.Value);
    }

    [Fact]
    public async Task GetCurrencyAsync_ReturnsCorrectCurrency_WhenCurrencyExists()
    {
        var currency = _fixture.Create<CurrencyDto>();

        _currencyService
            .Setup(m => m.GetCurrencyAsync(currency.Code))
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(currency));

        var sut = _fixture.Build<CurrenciesController>().OmitAutoProperties().Create();

        var result = await sut.GetCurrency(currency.Code);

        _currencyService.Verify(m => m.GetCurrencyAsync(currency.Code), Times.Once());
        Assert.Equal(currency, result.Value);
    }

    [Fact]
    public async Task GetCurrencyAsync_ReturnsNotFound_WhenCurrencyDoesNotExist()
    {
        var currencyCode = _fixture.Create<string>();

        _currencyService
            .Setup(m => m.GetCurrencyAsync(It.IsAny<string>()))
            .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<CurrencyDto>());

        var sut = _fixture.Build<CurrenciesController>().OmitAutoProperties().Create();

        var result = await sut.GetCurrency(currencyCode);

        _currencyService.Verify(m => m.GetCurrencyAsync(currencyCode), Times.Once());
        Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task PutCurrency_UpdatesCurrency()
    {
        var currency = _fixture.Create<CurrencyDto>();

        _currencyService
            .Setup(m => m.UpdateAsync(It.IsAny<CurrencyDto>()))
            .Returns<CurrencyDto>(c =>
                Task.FromResult(OperationResponseHelper.GenerateSuccessfulOperationResponse(c)));
        var sut = _fixture.Build<CurrenciesController>().OmitAutoProperties().Create();

        await sut.UpdateCurrency(currency.Code, currency);

        _currencyService.Verify(m => m.UpdateAsync(currency), Times.Once());
    }

    [Fact]
    public async Task PutCurrency_ReturnsBadRequest_WhenQueryParameterIdAndBodyIdDontMatch()
    {
        var currency = _fixture.Create<CurrencyDto>();

        var sut = _fixture.Build<CurrenciesController>().OmitAutoProperties().Create();

        var result = await sut.UpdateCurrency(currency.Code + "A", currency);

        _currencyService.Verify(m => m.UpdateAsync(currency), Times.Never());
        Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
    }
}