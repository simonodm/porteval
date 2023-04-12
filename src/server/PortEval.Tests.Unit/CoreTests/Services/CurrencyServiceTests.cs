using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Core;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Core.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Services;
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.Services
{
    public class CurrencyServiceTests
    {
        [Fact]
        public async Task GetAllCurrenciesAsync_ReturnsAllCurrencies()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currencies = fixture.CreateMany<CurrencyDto>();

            var currencyQueriesMock = fixture.CreateDefaultCurrencyQueriesMock();
            currencyQueriesMock
                .Setup(m => m.GetAllCurrenciesAsync())
                .ReturnsAsync(currencies);

            var sut = fixture.Create<CurrencyService>();

            var response = await sut.GetAllCurrenciesAsync();

            Assert.Equal(OperationStatus.Ok, response.Status);
            Assert.Equal(currencies, response.Response);
        }

        [Fact]
        public async Task GetCurrencyAsync_ReturnsCorrectCurrency_WhenItExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currency = fixture.Create<CurrencyDto>();

            var currencyQueriesMock = fixture.CreateDefaultCurrencyQueriesMock();
            currencyQueriesMock
                .Setup(m => m.GetCurrencyAsync(currency.Code))
                .ReturnsAsync(currency);

            var sut = fixture.Create<CurrencyService>();

            var response = await sut.GetCurrencyAsync(currency.Code);

            Assert.Equal(OperationStatus.Ok, response.Status);
            Assert.Equal(currency, response.Response);
        }

        [Fact]
        public async Task GetCurrencyAsync_ReturnsNotFound_WhenItDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currencyQueriesMock = fixture.CreateDefaultCurrencyQueriesMock();
            currencyQueriesMock
                .Setup(m => m.GetCurrencyAsync(It.IsAny<string>()))
                .ReturnsAsync((CurrencyDto)null);

            var sut = fixture.Create<CurrencyService>();

            var response = await sut.GetCurrencyAsync(fixture.Create<string>());

            Assert.Equal(OperationStatus.NotFound, response.Status);
        }

        [Fact]
        public async Task UpdatingCurrency_ChangesDefaultCurrencyUsingDomainService()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currencyDto = fixture.Build<CurrencyDto>().With(c => c.IsDefault, true).Create();
            var currency = new Currency(currencyDto.Code, currencyDto.Name, currencyDto.Symbol);
            var defaultCurrency = new Currency(fixture.Create<string>(), fixture.Create<string>(),
                fixture.Create<string>(), true);

            var currencyRepository = fixture.CreateDefaultCurrencyRepositoryMock();
            currencyRepository
                .Setup(r => r.FindAsync(currencyDto.Code))
                .ReturnsAsync(currency);
            currencyRepository
                .Setup(r => r.GetDefaultCurrencyAsync())
                .ReturnsAsync(defaultCurrency);
            currencyRepository
                .Setup(r => r.Update(It.IsAny<Currency>()))
                .Returns<Currency>(c => c);

            var currencyDomainService = fixture.Freeze<Mock<ICurrencyDomainService>>();

            var sut = fixture.Create<CurrencyService>();

            await sut.UpdateAsync(currencyDto);

            currencyDomainService.Verify(r => r.ChangeDefaultCurrency(defaultCurrency, currency), Times.Once());
        }
    }
}