using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Tests.Unit.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.ControllerTests
{
    public class CurrencyExchangeRatesControllerTests
    {
        private IFixture _fixture;
        private Mock<ICurrencyExchangeRateService> _exchangeRateService;

        public CurrencyExchangeRatesControllerTests()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            _exchangeRateService = _fixture.Freeze<Mock<ICurrencyExchangeRateService>>();
        }

        [Fact]
        public async Task GetExchangeRatesAsync_ReturnsExchangeRatesFromProvidedCurrency_IfCurrencyExists()
        {
            var currencyCode = _fixture.Create<string>();
            var time = _fixture.Create<DateTime>();
            var exchangeRates = _fixture.CreateMany<CurrencyExchangeRateDto>();

            _exchangeRateService
                .Setup(m => m.GetExchangeRatesAsync(currencyCode, time))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(exchangeRates));

            var sut = _fixture.Build<CurrencyExchangeRatesController>().OmitAutoProperties().Create();

            var result = await sut.GetExchangeRates(currencyCode, time);

            _exchangeRateService.Verify(m => m.GetExchangeRatesAsync(currencyCode, time), Times.Once());
            Assert.Equal(exchangeRates, result.Value);
        }

        [Fact]
        public async Task GetExchangeRatesAsync_ReturnsCurrentExchangeRatesFromProvidedCurrency_IfNoTimeIsProvided()
        {
            var currencyCode = _fixture.Create<string>();
            var now = DateTime.UtcNow;
            var exchangeRates = _fixture.CreateMany<CurrencyExchangeRateDto>();

            _exchangeRateService
                .Setup(m => m.GetExchangeRatesAsync(currencyCode, It.Is<DateTime>(dt => dt >= now)))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(exchangeRates));

            var sut = _fixture.Build<CurrencyExchangeRatesController>().OmitAutoProperties().Create();

            var result = await sut.GetExchangeRates(currencyCode, null);

            _exchangeRateService.Verify(m => m.GetExchangeRatesAsync(currencyCode, It.Is<DateTime>(dt => dt >= now)), Times.Once());
            Assert.Equal(exchangeRates, result.Value);
        }

        [Fact]
        public async Task GetExchangeRatesAsync_ReturnsNotFound_WhenCurrencyDoesNotExist()
        {
            var currencyCode = _fixture.Create<string>();
            var time = _fixture.Create<DateTime>();

            _exchangeRateService
                .Setup(m => m.GetExchangeRatesAsync(currencyCode, time))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<IEnumerable<CurrencyExchangeRateDto>>());

            var sut = _fixture.Build<CurrencyExchangeRatesController>().OmitAutoProperties().Create();

            var result = await sut.GetExchangeRates(currencyCode, time);

            _exchangeRateService.Verify(m => m.GetExchangeRatesAsync(currencyCode, time), Times.Once());
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetLatestExchangeRates_ReturnsCurrentExchangeRates_WhenCurrencyExists()
        {
            var currencyCode = _fixture.Create<string>();
            var now = DateTime.UtcNow;
            var exchangeRates = _fixture.CreateMany<CurrencyExchangeRateDto>();

            _exchangeRateService
                .Setup(m => m.GetExchangeRatesAsync(currencyCode, It.Is<DateTime>(dt => dt >= now)))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(exchangeRates));

            var sut = _fixture.Build<CurrencyExchangeRatesController>().OmitAutoProperties().Create();

            var result = await sut.GetLatestExchangeRates(currencyCode);

            _exchangeRateService.Verify(m => m.GetExchangeRatesAsync(currencyCode, It.Is<DateTime>(dt => dt >= now)), Times.Once());
            Assert.Equal(exchangeRates, result.Value);
        }

        [Fact]
        public async Task GetLatestExchangeRates_ReturnsNotFound_WhenCurrencyDoesNotExist()
        {
            var currencyCode = _fixture.Create<string>();
            var now = DateTime.UtcNow;

            _exchangeRateService
                .Setup(m => m.GetExchangeRatesAsync(currencyCode, It.Is<DateTime>(dt => dt >= now)))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<IEnumerable<CurrencyExchangeRateDto>>());

            var sut = _fixture.Build<CurrencyExchangeRatesController>().OmitAutoProperties().Create();

            var result = await sut.GetLatestExchangeRates(currencyCode);

            _exchangeRateService.Verify(m => m.GetExchangeRatesAsync(currencyCode, It.Is<DateTime>(dt => dt >= now)), Times.Once());
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetLatestExchangeRate_ReturnsLatestExchangeRateBetweenProvidedCurrencies_IfCurrenciesExist()
        {
            var currencyCodeFrom = _fixture.Create<string>();
            var currencyCodeTo = _fixture.Create<string>();
            var now = DateTime.UtcNow;
            var exchangeRate = _fixture.Create<CurrencyExchangeRateDto>();

            _exchangeRateService
                .Setup(m =>
                    m.GetExchangeRateAtAsync(
                        currencyCodeFrom,
                        currencyCodeTo,
                        It.Is<DateTime>(dt => dt >= now)
                    )
                )
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(exchangeRate));

            var sut = _fixture.Build<CurrencyExchangeRatesController>().OmitAutoProperties().Create();

            var result = await sut.GetLatestExchangeRate(currencyCodeFrom, currencyCodeTo);

            _exchangeRateService.Verify(m => m.GetExchangeRateAtAsync(currencyCodeFrom, currencyCodeTo, It.Is<DateTime>(dt => dt >= now)), Times.Once());
            Assert.Equal(exchangeRate, result.Value);
        }

        [Fact]
        public async Task GetLatestExchangeRate_ReturnsNotFound_WhenCurrenciesDoNotExist()
        {
            var currencyCodeFrom = _fixture.Create<string>();
            var currencyCodeTo = _fixture.Create<string>();
            var now = DateTime.UtcNow;

            _exchangeRateService
                .Setup(m =>
                    m.GetExchangeRateAtAsync(
                        currencyCodeFrom,
                        currencyCodeTo,
                        It.Is<DateTime>(dt => dt >= now)
                    )
                )
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<CurrencyExchangeRateDto>());

            var sut = _fixture.Build<CurrencyExchangeRatesController>().OmitAutoProperties().Create();

            var result = await sut.GetLatestExchangeRate(currencyCodeFrom, currencyCodeTo);

            _exchangeRateService.Verify(m => m.GetExchangeRateAtAsync(currencyCodeFrom, currencyCodeTo, It.Is<DateTime>(dt => dt >= now)), Times.Once());
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetExchangeRateAt_ReturnsExchangeRateBetweenProvidedCurrencies_IfCurrenciesExist()
        {
            var currencyCodeFrom = _fixture.Create<string>();
            var currencyCodeTo = _fixture.Create<string>();
            var time = _fixture.Create<DateTime>();
            var exchangeRate = _fixture.Create<CurrencyExchangeRateDto>();

            _exchangeRateService
                .Setup(m =>
                    m.GetExchangeRateAtAsync(
                        currencyCodeFrom,
                        currencyCodeTo,
                        time
                    )
                )
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(exchangeRate));

            var sut = _fixture.Build<CurrencyExchangeRatesController>().OmitAutoProperties().Create();

            var result = await sut.GetExchangeRateAt(currencyCodeFrom, currencyCodeTo, time);

            _exchangeRateService.Verify(m => m.GetExchangeRateAtAsync(currencyCodeFrom, currencyCodeTo, time), Times.Once());
            Assert.Equal(exchangeRate, result.Value);
        }

        [Fact]
        public async Task GetExchangeRateAt_ReturnsNotFound_WhenCurrenciesDoNotExist()
        {
            var currencyCodeFrom = _fixture.Create<string>();
            var currencyCodeTo = _fixture.Create<string>();
            var time = _fixture.Create<DateTime>();

            _exchangeRateService
                .Setup(m =>
                    m.GetExchangeRateAtAsync(
                        currencyCodeFrom,
                        currencyCodeTo,
                        time
                    )
                )
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<CurrencyExchangeRateDto>());

            var sut = _fixture.Build<CurrencyExchangeRatesController>().OmitAutoProperties().Create();

            var result = await sut.GetExchangeRateAt(currencyCodeFrom, currencyCodeTo, time);

            _exchangeRateService.Verify(m => m.GetExchangeRateAtAsync(currencyCodeFrom, currencyCodeTo, time), Times.Once());
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }
    }
}
