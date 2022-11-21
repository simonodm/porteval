using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Services.Interfaces;
using PortEval.Application.Services.Queries.Interfaces;
using PortEval.Tests.Unit.Helpers;
using Xunit;

namespace PortEval.Tests.Unit.ControllerTests
{
    public class CurrenciesControllerTests
    {
        [Fact]
        public async Task GetAllCurrencies_ReturnsCurrenciesFromQueries()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currencies = fixture.CreateMany<CurrencyDto>();

            var currencyQueries = fixture.Freeze<Mock<ICurrencyQueries>>();
            currencyQueries
                .Setup(m => m.GetAllCurrencies())
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(currencies));
            var sut = fixture.Build<CurrenciesController>().OmitAutoProperties().Create();

            var result = await sut.GetAllCurrencies();

            currencyQueries.Verify(m => m.GetAllCurrencies(), Times.Once());
            Assert.Equal(currencies, result.Value);
        }

        [Fact]
        public async Task GetCurrency_ReturnsCorrectCurrencyFromQueries_WhenCurrencyExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currency = fixture.Create<CurrencyDto>();

            var currencyQueries = fixture.Freeze<Mock<ICurrencyQueries>>();
            currencyQueries
                .Setup(m => m.GetCurrency(currency.Code))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(currency));

            var sut = fixture.Build<CurrenciesController>().OmitAutoProperties().Create();

            var result = await sut.GetCurrency(currency.Code);

            currencyQueries.Verify(m => m.GetCurrency(currency.Code), Times.Once());
            Assert.Equal(currency, result.Value);
        }

        [Fact]
        public async Task GetCurrency_ReturnsNotFound_WhenCurrencyDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currencyCode = fixture.Create<string>();

            var currencyQueries = fixture.Freeze<Mock<ICurrencyQueries>>();
            currencyQueries
                .Setup(m => m.GetCurrency(It.IsAny<string>()))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<CurrencyDto>());

            var sut = fixture.Build<CurrenciesController>().OmitAutoProperties().Create();

            var result = await sut.GetCurrency(currencyCode);

            currencyQueries.Verify(m => m.GetCurrency(currencyCode), Times.Once());
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task PutCurrency_UpdatesCurrencyUsingService()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currency = fixture.Create<CurrencyDto>();

            var currencyService = fixture.Freeze<Mock<ICurrencyService>>();
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
            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        }
    }
}
