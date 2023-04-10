using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Models.DTOs;
using PortEval.Tests.Unit.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Services;
using Xunit;

namespace PortEval.Tests.Unit.ControllerTests
{
    public class CurrencyExchangeRatesControllerTests
    {
        [Fact]
        public async Task GetExchangeRatesAsync_ReturnsExchangeRatesFromProvidedCurrency_IfCurrencyExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currencyCode = fixture.Create<string>();
            var time = fixture.Create<DateTime>();
            var exchangeRates = fixture.CreateMany<CurrencyExchangeRateDto>();

            var exchangeRateService = fixture.Freeze<Mock<ICurrencyExchangeRateService>>();
            exchangeRateService
                .Setup(m => m.GetExchangeRatesAsync(currencyCode, time))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(exchangeRates));

            var sut = fixture.Build<CurrencyExchangeRatesController>().OmitAutoProperties().Create();

            var result = await sut.GetExchangeRates(currencyCode, time);

            exchangeRateService.Verify(m => m.GetExchangeRatesAsync(currencyCode, time), Times.Once());
            Assert.Equal(exchangeRates, result.Value);
        }

        [Fact]
        public async Task GetExchangeRatesAsync_ReturnsCurrentExchangeRatesFromProvidedCurrency_IfNoTimeIsProvided()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currencyCode = fixture.Create<string>();
            var now = DateTime.UtcNow;
            var exchangeRates = fixture.CreateMany<CurrencyExchangeRateDto>();

            var exchangeRateService = fixture.Freeze<Mock<ICurrencyExchangeRateService>>();
            exchangeRateService
                .Setup(m => m.GetExchangeRatesAsync(currencyCode, It.Is<DateTime>(dt => dt >= now)))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(exchangeRates));

            var sut = fixture.Build<CurrencyExchangeRatesController>().OmitAutoProperties().Create();

            var result = await sut.GetExchangeRates(currencyCode, null);

            exchangeRateService.Verify(m => m.GetExchangeRatesAsync(currencyCode, It.Is<DateTime>(dt => dt >= now)), Times.Once());
            Assert.Equal(exchangeRates, result.Value);
        }

        [Fact]
        public async Task GetExchangeRatesAsync_ReturnsNotFound_WhenCurrencyDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currencyCode = fixture.Create<string>();
            var time = fixture.Create<DateTime>();

            var exchangeRateService = fixture.Freeze<Mock<ICurrencyExchangeRateService>>();
            exchangeRateService
                .Setup(m => m.GetExchangeRatesAsync(currencyCode, time))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<IEnumerable<CurrencyExchangeRateDto>>());

            var sut = fixture.Build<CurrencyExchangeRatesController>().OmitAutoProperties().Create();

            var result = await sut.GetExchangeRates(currencyCode, time);

            exchangeRateService.Verify(m => m.GetExchangeRatesAsync(currencyCode, time), Times.Once());
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetLatestExchangeRates_ReturnsCurrentExchangeRates_WhenCurrencyExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currencyCode = fixture.Create<string>();
            var now = DateTime.UtcNow;
            var exchangeRates = fixture.CreateMany<CurrencyExchangeRateDto>();

            var exchangeRateService = fixture.Freeze<Mock<ICurrencyExchangeRateService>>();
            exchangeRateService
                .Setup(m => m.GetExchangeRatesAsync(currencyCode, It.Is<DateTime>(dt => dt >= now)))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(exchangeRates));

            var sut = fixture.Build<CurrencyExchangeRatesController>().OmitAutoProperties().Create();

            var result = await sut.GetLatestExchangeRates(currencyCode);

            exchangeRateService.Verify(m => m.GetExchangeRatesAsync(currencyCode, It.Is<DateTime>(dt => dt >= now)), Times.Once());
            Assert.Equal(exchangeRates, result.Value);
        }

        [Fact]
        public async Task GetLatestExchangeRates_ReturnsNotFound_WhenCurrencyDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currencyCode = fixture.Create<string>();
            var now = DateTime.UtcNow;

            var exchangeRateService = fixture.Freeze<Mock<ICurrencyExchangeRateService>>();
            exchangeRateService
                .Setup(m => m.GetExchangeRatesAsync(currencyCode, It.Is<DateTime>(dt => dt >= now)))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<IEnumerable<CurrencyExchangeRateDto>>());

            var sut = fixture.Build<CurrencyExchangeRatesController>().OmitAutoProperties().Create();

            var result = await sut.GetLatestExchangeRates(currencyCode);

            exchangeRateService.Verify(m => m.GetExchangeRatesAsync(currencyCode, It.Is<DateTime>(dt => dt >= now)), Times.Once());
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetLatestExchangeRate_ReturnsLatestExchangeRateBetweenProvidedCurrencies_IfCurrenciesExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currencyCodeFrom = fixture.Create<string>();
            var currencyCodeTo = fixture.Create<string>();
            var now = DateTime.UtcNow;
            var exchangeRate = fixture.Create<CurrencyExchangeRateDto>();

            var exchangeRateService = fixture.Freeze<Mock<ICurrencyExchangeRateService>>();
            exchangeRateService
                .Setup(m =>
                    m.GetExchangeRateAtAsync(
                        currencyCodeFrom,
                        currencyCodeTo,
                        It.Is<DateTime>(dt => dt >= now)
                    )
                )
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(exchangeRate));

            var sut = fixture.Build<CurrencyExchangeRatesController>().OmitAutoProperties().Create();

            var result = await sut.GetLatestExchangeRate(currencyCodeFrom, currencyCodeTo);

            exchangeRateService.Verify(m => m.GetExchangeRateAtAsync(currencyCodeFrom, currencyCodeTo, It.Is<DateTime>(dt => dt >= now)), Times.Once());
            Assert.Equal(exchangeRate, result.Value);
        }

        [Fact]
        public async Task GetLatestExchangeRate_ReturnsNotFound_WhenCurrenciesDoNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currencyCodeFrom = fixture.Create<string>();
            var currencyCodeTo = fixture.Create<string>();
            var now = DateTime.UtcNow;

            var exchangeRateService = fixture.Freeze<Mock<ICurrencyExchangeRateService>>();
            exchangeRateService
                .Setup(m =>
                    m.GetExchangeRateAtAsync(
                        currencyCodeFrom,
                        currencyCodeTo,
                        It.Is<DateTime>(dt => dt >= now)
                    )
                )
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<CurrencyExchangeRateDto>());

            var sut = fixture.Build<CurrencyExchangeRatesController>().OmitAutoProperties().Create();

            var result = await sut.GetLatestExchangeRate(currencyCodeFrom, currencyCodeTo);

            exchangeRateService.Verify(m => m.GetExchangeRateAtAsync(currencyCodeFrom, currencyCodeTo, It.Is<DateTime>(dt => dt >= now)), Times.Once());
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetExchangeRateAt_ReturnsExchangeRateBetweenProvidedCurrencies_IfCurrenciesExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currencyCodeFrom = fixture.Create<string>();
            var currencyCodeTo = fixture.Create<string>();
            var time = fixture.Create<DateTime>();
            var exchangeRate = fixture.Create<CurrencyExchangeRateDto>();

            var exchangeRateService = fixture.Freeze<Mock<ICurrencyExchangeRateService>>();
            exchangeRateService
                .Setup(m =>
                    m.GetExchangeRateAtAsync(
                        currencyCodeFrom,
                        currencyCodeTo,
                        time
                    )
                )
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(exchangeRate));

            var sut = fixture.Build<CurrencyExchangeRatesController>().OmitAutoProperties().Create();

            var result = await sut.GetExchangeRateAt(currencyCodeFrom, currencyCodeTo, time);

            exchangeRateService.Verify(m => m.GetExchangeRateAtAsync(currencyCodeFrom, currencyCodeTo, time), Times.Once());
            Assert.Equal(exchangeRate, result.Value);
        }

        [Fact]
        public async Task GetExchangeRateAt_ReturnsNotFound_WhenCurrenciesDoNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currencyCodeFrom = fixture.Create<string>();
            var currencyCodeTo = fixture.Create<string>();
            var time = fixture.Create<DateTime>();

            var exchangeRateService = fixture.Freeze<Mock<ICurrencyExchangeRateService>>();
            exchangeRateService
                .Setup(m =>
                    m.GetExchangeRateAtAsync(
                        currencyCodeFrom,
                        currencyCodeTo,
                        time
                    )
                )
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<CurrencyExchangeRateDto>());

            var sut = fixture.Build<CurrencyExchangeRatesController>().OmitAutoProperties().Create();

            var result = await sut.GetExchangeRateAt(currencyCodeFrom, currencyCodeTo, time);

            exchangeRateService.Verify(m => m.GetExchangeRateAtAsync(currencyCodeFrom, currencyCodeTo, time), Times.Once());
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }
    }
}
