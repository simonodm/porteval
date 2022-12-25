using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Features.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;

namespace PortEval.Tests.Unit.FeatureTests.Services
{
    public class CurrencyServiceTests
    {
        [Fact]
        public async Task UpdatingCurrency_UpdatesDefaultCurrencyInRepository_WhenWellFormed()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currencyDto = fixture.Build<CurrencyDto>().With(c => c.IsDefault, true).Create();
            var currency = new Currency(currencyDto.Code, currencyDto.Name, currencyDto.Symbol);

            var currencyRepository = fixture.CreateDefaultCurrencyRepositoryMock();
            currencyRepository
                .Setup(r => r.FindAsync(currencyDto.Code))
                .Returns(Task.FromResult(currency));

            var sut = fixture.Create<CurrencyService>();

            await sut.UpdateAsync(currencyDto);

            currencyRepository.Verify(r => r.Update(It.Is<Currency>(c =>
                c.IsDefault &&
                c.Code == currencyDto.Code
            )), Times.Once());
        }

        [Fact]
        public async Task UpdatingCurrency_UnsetsPreviousDefaultCurrency()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currencyDto = fixture.Build<CurrencyDto>().With(c => c.IsDefault, true).Create();
            var existingCurrency = new Currency(currencyDto.Code, currencyDto.Name, currencyDto.Symbol);
            var defaultCurrency = fixture.Create<Currency>();
            defaultCurrency.SetAsDefault();

            var currencyRepository = fixture.CreateDefaultCurrencyRepositoryMock();
            currencyRepository
                .Setup(r => r.FindAsync(currencyDto.Code))
                .Returns(Task.FromResult(existingCurrency));
            currencyRepository
                .Setup(r => r.GetDefaultCurrencyAsync())
                .Returns(Task.FromResult(defaultCurrency));

            var sut = fixture.Create<CurrencyService>();

            await sut.UpdateAsync(currencyDto);

            currencyRepository.Verify(r => r.Update(It.Is<Currency>(c =>
                c.IsDefault == false &&
                c.Code == defaultCurrency.Code
            )), Times.Once());
        }

        [Fact]
        public async Task UpdatingCurrency_ThrowsException_WhenCurrencyDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currencyDto = fixture.Build<CurrencyDto>().With(c => c.IsDefault, true).Create();

            var currencyRepository = fixture.CreateDefaultCurrencyRepositoryMock();
            currencyRepository
                .Setup(r => r.FindAsync(currencyDto.Code))
                .Returns(Task.FromResult<Currency>(null));
            currencyRepository
                .Setup(r => r.ExistsAsync(currencyDto.Code))
                .Returns(Task.FromResult(false));

            var sut = fixture.Create<CurrencyService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await sut.UpdateAsync(currencyDto));
        }

        [Fact]
        public async Task UpdatingCurrency_ChangesNothing_WhenCurrencyIsNotSetToDefault()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var currencyDto = fixture.Build<CurrencyDto>().With(c => c.IsDefault, false).Create();
            var existingCurrency = new Currency(currencyDto.Code, currencyDto.Name, currencyDto.Symbol);

            var currencyRepository = fixture.CreateDefaultCurrencyRepositoryMock();
            currencyRepository
                .Setup(r => r.FindAsync(currencyDto.Code))
                .Returns(Task.FromResult(existingCurrency));

            var sut = fixture.Create<CurrencyService>();

            await sut.UpdateAsync(currencyDto);

            currencyRepository.Verify(r => r.Update(It.IsAny<Currency>()), Times.Never());
        }
    }
}