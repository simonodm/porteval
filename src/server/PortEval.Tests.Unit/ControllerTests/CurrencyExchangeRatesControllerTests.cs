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
using PortEval.Application.Services.Queries.Interfaces;
using PortEval.Tests.Unit.Helpers;
using Xunit;

namespace PortEval.Tests.Unit.ControllerTests
{
    public class CurrencyExchangeRatesControllerTests
    {
        [Fact]
        public async Task GetExchangeRates_ReturnsExchangeRatesFromProvidedCurrency_IfCurrencyExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currencyCode = fixture.Create<string>();
            var time = fixture.Create<DateTime>();
            var exchangeRates = fixture.CreateMany<CurrencyExchangeRateDto>();

            var exchangeRateQueries = fixture.Freeze<Mock<ICurrencyExchangeRateQueries>>();
            exchangeRateQueries
                .Setup(m => m.GetExchangeRates(currencyCode, time))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(exchangeRates));

            var sut = fixture.Build<CurrencyExchangeRatesController>().OmitAutoProperties().Create();

            var result = await sut.GetExchangeRates(currencyCode, time);

            exchangeRateQueries.Verify(m => m.GetExchangeRates(currencyCode, time), Times.Once());
            Assert.Equal(exchangeRates, result.Value);
        }

        [Fact]
        public async Task GetExchangeRates_ReturnsCurrentExchangeRatesFromProvidedCurrency_IfNoTimeIsProvided()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currencyCode = fixture.Create<string>();
            var now = DateTime.UtcNow;
            var exchangeRates = fixture.CreateMany<CurrencyExchangeRateDto>();

            var exchangeRateQueries = fixture.Freeze<Mock<ICurrencyExchangeRateQueries>>();
            exchangeRateQueries
                .Setup(m => m.GetExchangeRates(currencyCode, It.Is<DateTime>(dt => dt >= now)))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(exchangeRates));

            var sut = fixture.Build<CurrencyExchangeRatesController>().OmitAutoProperties().Create();

            var result = await sut.GetExchangeRates(currencyCode, null);

            exchangeRateQueries.Verify(m => m.GetExchangeRates(currencyCode, It.Is<DateTime>(dt => dt >= now)), Times.Once());
            Assert.Equal(exchangeRates, result.Value);
        }

        [Fact]
        public async Task GetExchangeRates_ReturnsNotFound_WhenCurrencyDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currencyCode = fixture.Create<string>();
            var time = fixture.Create<DateTime>();

            var exchangeRateQueries = fixture.Freeze<Mock<ICurrencyExchangeRateQueries>>();
            exchangeRateQueries
                .Setup(m => m.GetExchangeRates(currencyCode, time))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<IEnumerable<CurrencyExchangeRateDto>>());

            var sut = fixture.Build<CurrencyExchangeRatesController>().OmitAutoProperties().Create();

            var result = await sut.GetExchangeRates(currencyCode, time);

            exchangeRateQueries.Verify(m => m.GetExchangeRates(currencyCode, time), Times.Once());
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

            var exchangeRateQueries = fixture.Freeze<Mock<ICurrencyExchangeRateQueries>>();
            exchangeRateQueries
                .Setup(m => m.GetExchangeRates(currencyCode, It.Is<DateTime>(dt => dt >= now)))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(exchangeRates));

            var sut = fixture.Build<CurrencyExchangeRatesController>().OmitAutoProperties().Create();

            var result = await sut.GetLatestExchangeRates(currencyCode);

            exchangeRateQueries.Verify(m => m.GetExchangeRates(currencyCode, It.Is<DateTime>(dt => dt >= now)), Times.Once());
            Assert.Equal(exchangeRates, result.Value);
        }

        [Fact]
        public async Task GetLatestExchangeRates_ReturnsNotFound_WhenCurrencyDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currencyCode = fixture.Create<string>();
            var now = DateTime.UtcNow;

            var exchangeRateQueries = fixture.Freeze<Mock<ICurrencyExchangeRateQueries>>();
            exchangeRateQueries
                .Setup(m => m.GetExchangeRates(currencyCode, It.Is<DateTime>(dt => dt >= now)))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<IEnumerable<CurrencyExchangeRateDto>>());

            var sut = fixture.Build<CurrencyExchangeRatesController>().OmitAutoProperties().Create();

            var result = await sut.GetLatestExchangeRates(currencyCode);

            exchangeRateQueries.Verify(m => m.GetExchangeRates(currencyCode, It.Is<DateTime>(dt => dt >= now)), Times.Once());
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

            var exchangeRateQueries = fixture.Freeze<Mock<ICurrencyExchangeRateQueries>>();
            exchangeRateQueries
                .Setup(m =>
                    m.GetExchangeRateAt(
                        currencyCodeFrom,
                        currencyCodeTo,
                        It.Is<DateTime>(dt => dt >= now)
                    )
                )
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(exchangeRate));

            var sut = fixture.Build<CurrencyExchangeRatesController>().OmitAutoProperties().Create();

            var result = await sut.GetLatestExchangeRate(currencyCodeFrom, currencyCodeTo);

            exchangeRateQueries.Verify(m => m.GetExchangeRateAt(currencyCodeFrom, currencyCodeTo, It.Is<DateTime>(dt => dt >= now)), Times.Once());
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

            var exchangeRateQueries = fixture.Freeze<Mock<ICurrencyExchangeRateQueries>>();
            exchangeRateQueries
                .Setup(m =>
                    m.GetExchangeRateAt(
                        currencyCodeFrom,
                        currencyCodeTo,
                        It.Is<DateTime>(dt => dt >= now)
                    )
                )
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<CurrencyExchangeRateDto>());

            var sut = fixture.Build<CurrencyExchangeRatesController>().OmitAutoProperties().Create();

            var result = await sut.GetLatestExchangeRate(currencyCodeFrom, currencyCodeTo);

            exchangeRateQueries.Verify(m => m.GetExchangeRateAt(currencyCodeFrom, currencyCodeTo, It.Is<DateTime>(dt => dt >= now)), Times.Once());
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

            var exchangeRateQueries = fixture.Freeze<Mock<ICurrencyExchangeRateQueries>>();
            exchangeRateQueries
                .Setup(m =>
                    m.GetExchangeRateAt(
                        currencyCodeFrom,
                        currencyCodeTo,
                        time
                    )
                )
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(exchangeRate));

            var sut = fixture.Build<CurrencyExchangeRatesController>().OmitAutoProperties().Create();

            var result = await sut.GetExchangeRateAt(currencyCodeFrom, currencyCodeTo, time);

            exchangeRateQueries.Verify(m => m.GetExchangeRateAt(currencyCodeFrom, currencyCodeTo, time), Times.Once());
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

            var exchangeRateQueries = fixture.Freeze<Mock<ICurrencyExchangeRateQueries>>();
            exchangeRateQueries
                .Setup(m =>
                    m.GetExchangeRateAt(
                        currencyCodeFrom,
                        currencyCodeTo,
                        time
                    )
                )
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<CurrencyExchangeRateDto>());

            var sut = fixture.Build<CurrencyExchangeRatesController>().OmitAutoProperties().Create();

            var result = await sut.GetExchangeRateAt(currencyCodeFrom, currencyCodeTo, time);

            exchangeRateQueries.Verify(m => m.GetExchangeRateAt(currencyCodeFrom, currencyCodeTo, time), Times.Once());
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }
    }
}
