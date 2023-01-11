using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Features.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Services;
using PortEval.Tests.Unit.Helpers.Extensions;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.FeatureTests.Services
{
    public class CurrencyServiceTests
    {
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

            var currencyDomainService = fixture.Freeze<Mock<ICurrencyDomainService>>();

            var sut = fixture.Create<CurrencyService>();

            await sut.UpdateAsync(currencyDto);

            currencyDomainService.Verify(r => r.ChangeDefaultCurrency(defaultCurrency, currency), Times.Once());
        }
    }
}