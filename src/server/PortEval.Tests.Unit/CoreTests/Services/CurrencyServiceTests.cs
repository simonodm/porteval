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

namespace PortEval.Tests.Unit.CoreTests.Services;

public class CurrencyServiceTests
{
    private readonly Mock<ICurrencyQueries> _currencyQueries;
    private readonly Mock<ICurrencyRepository> _currencyRepository;
    private readonly IFixture _fixture;

    public CurrencyServiceTests()
    {
        _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
        _currencyQueries = _fixture.CreateDefaultCurrencyQueriesMock();
        _currencyRepository = _fixture.CreateDefaultCurrencyRepositoryMock();
    }

    [Fact]
    public async Task GetAllCurrenciesAsync_ReturnsAllCurrencies()
    {
        var currencies = _fixture.CreateMany<CurrencyDto>();

        _currencyQueries
            .Setup(m => m.GetAllCurrenciesAsync())
            .ReturnsAsync(currencies);

        var sut = _fixture.Create<CurrencyService>();

        var response = await sut.GetAllCurrenciesAsync();

        Assert.Equal(OperationStatus.Ok, response.Status);
        Assert.Equal(currencies, response.Response);
    }

    [Fact]
    public async Task GetCurrencyAsync_ReturnsCorrectCurrency_WhenItExists()
    {
        var currency = _fixture.Create<CurrencyDto>();

        _currencyQueries
            .Setup(m => m.GetCurrencyAsync(currency.Code))
            .ReturnsAsync(currency);

        var sut = _fixture.Create<CurrencyService>();

        var response = await sut.GetCurrencyAsync(currency.Code);

        Assert.Equal(OperationStatus.Ok, response.Status);
        Assert.Equal(currency, response.Response);
    }

    [Fact]
    public async Task GetCurrencyAsync_ReturnsNotFound_WhenItDoesNotExist()
    {
        _currencyQueries
            .Setup(m => m.GetCurrencyAsync(It.IsAny<string>()))
            .ReturnsAsync((CurrencyDto)null);

        var sut = _fixture.Create<CurrencyService>();

        var response = await sut.GetCurrencyAsync(_fixture.Create<string>());

        Assert.Equal(OperationStatus.NotFound, response.Status);
    }

    [Fact]
    public async Task UpdatingCurrency_ChangesDefaultCurrencyUsingDomainService()
    {
        var currencyDto = _fixture.Build<CurrencyDto>().With(c => c.IsDefault, true).Create();
        var currency = new Currency(currencyDto.Code, currencyDto.Name, currencyDto.Symbol);
        var defaultCurrency = new Currency(_fixture.Create<string>(), _fixture.Create<string>(),
            _fixture.Create<string>(), true);

        _currencyRepository
            .Setup(r => r.FindAsync(currencyDto.Code))
            .ReturnsAsync(currency);
        _currencyRepository
            .Setup(r => r.GetDefaultCurrencyAsync())
            .ReturnsAsync(defaultCurrency);

        var currencyDomainService = _fixture.Freeze<Mock<ICurrencyDomainService>>();

        var sut = _fixture.Create<CurrencyService>();

        await sut.UpdateAsync(currencyDto);

        currencyDomainService.Verify(r => r.ChangeDefaultCurrency(defaultCurrency, currency), Times.Once());
    }
}