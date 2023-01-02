using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Features.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Services;
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;

namespace PortEval.Tests.Unit.DomainTests.Services
{
    public class CurrencyDomainServiceTests
    {
        [Fact]
        public void ChangeDefaultCurrency_UnsetsPreviousDefaultCurrency()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            
            var newDefaultCurrency = new Currency(fixture.Create<string>(), fixture.Create<string>(), fixture.Create<string>());
            var previousDefaultCurrency = new Currency(fixture.Create<string>(), fixture.Create<string>(), fixture.Create<string>(), true);

            var sut = fixture.Create<CurrencyDomainService>();

            sut.ChangeDefaultCurrency(previousDefaultCurrency, newDefaultCurrency);

            Assert.False(previousDefaultCurrency.IsDefault);
        }

        [Fact]
        public void ChangeDefaultCurrency_ThrowsException_WhenProvidedDefaultCurrencyIsNotDefault()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var newDefaultCurrency = new Currency(fixture.Create<string>(), fixture.Create<string>(), fixture.Create<string>());
            var previousDefaultCurrency = new Currency(fixture.Create<string>(), fixture.Create<string>(), fixture.Create<string>());

            var sut = fixture.Create<CurrencyDomainService>();

            Assert.Throws<OperationNotAllowedException>(() => sut.ChangeDefaultCurrency(previousDefaultCurrency, newDefaultCurrency));
        }

        [Fact]
        public void ChangeDefaultCurrency_SetsNewDefaultCurrencyAsDefault()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var newDefaultCurrency = new Currency(fixture.Create<string>(), fixture.Create<string>(), fixture.Create<string>());
            var previousDefaultCurrency = new Currency(fixture.Create<string>(), fixture.Create<string>(), fixture.Create<string>(), true);

            var sut = fixture.Create<CurrencyDomainService>();

            sut.ChangeDefaultCurrency(previousDefaultCurrency, newDefaultCurrency);

            Assert.True(newDefaultCurrency.IsDefault);
        }
    }
}
