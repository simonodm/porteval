using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Models.DTOs;
using PortEval.Tests.Unit.Helpers;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Services;
using Xunit;

namespace PortEval.Tests.Unit.ControllerTests
{
    public class CurrenciesControllerTests
    {
        [Fact]
        public async Task GetAllCurrenciesAsync_ReturnsCurrencies()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currencies = fixture.CreateMany<CurrencyDto>();

            var currencyService = fixture.Freeze<Mock<ICurrencyService>>();
            currencyService
                .Setup(m => m.GetAllCurrenciesAsync())
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(currencies));
            var sut = fixture.Build<CurrenciesController>().OmitAutoProperties().Create();

            var result = await sut.GetAllCurrencies();

            currencyService.Verify(m => m.GetAllCurrenciesAsync(), Times.Once());
            Assert.Equal(currencies, result.Value);
        }

        [Fact]
        public async Task GetCurrencyAsync_ReturnsCorrectCurrency_WhenCurrencyExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currency = fixture.Create<CurrencyDto>();

            var currencyService = fixture.Freeze<Mock<ICurrencyService>>();
            currencyService
                .Setup(m => m.GetCurrencyAsync(currency.Code))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(currency));

            var sut = fixture.Build<CurrenciesController>().OmitAutoProperties().Create();

            var result = await sut.GetCurrency(currency.Code);

            currencyService.Verify(m => m.GetCurrencyAsync(currency.Code), Times.Once());
            Assert.Equal(currency, result.Value);
        }

        [Fact]
        public async Task GetCurrencyAsync_ReturnsNotFound_WhenCurrencyDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currencyCode = fixture.Create<string>();

            var currencyService = fixture.Freeze<Mock<ICurrencyService>>();
            currencyService
                .Setup(m => m.GetCurrencyAsync(It.IsAny<string>()))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<CurrencyDto>());

            var sut = fixture.Build<CurrenciesController>().OmitAutoProperties().Create();

            var result = await sut.GetCurrency(currencyCode);

            currencyService.Verify(m => m.GetCurrencyAsync(currencyCode), Times.Once());
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task PutCurrency_UpdatesCurrency()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currency = fixture.Create<CurrencyDto>();

            var currencyService = fixture.Freeze<Mock<ICurrencyService>>();
            currencyService
                .Setup(m => m.UpdateAsync(It.IsAny<CurrencyDto>()))
                .Returns<CurrencyDto>(c =>
                    Task.FromResult(OperationResponseHelper.GenerateSuccessfulOperationResponse(c)));
            var sut = fixture.Build<CurrenciesController>().OmitAutoProperties().Create();

            await sut.UpdateCurrency(currency.Code, currency);

            currencyService.Verify(m => m.UpdateAsync(currency), Times.Once());
        }

        [Fact]
        public async Task PutCurrency_ReturnsBadRequest_WhenQueryParameterIdAndBodyIdDontMatch()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currency = fixture.Create<CurrencyDto>();

            var currencyService = fixture.Freeze<Mock<ICurrencyService>>();
            var sut = fixture.Build<CurrenciesController>().OmitAutoProperties().Create();

            var result = await sut.UpdateCurrency(currency.Code + "A", currency);

            currencyService.Verify(m => m.UpdateAsync(currency), Times.Never());
            Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
        }
    }
}
