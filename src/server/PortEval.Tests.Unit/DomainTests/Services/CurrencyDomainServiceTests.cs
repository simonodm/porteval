using AutoFixture;
using AutoFixture.AutoMoq;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Services;
using Xunit;

namespace PortEval.Tests.Unit.DomainTests.Services
{
    public class CurrencyDomainServiceTests
    {
        private IFixture _fixture;

        public CurrencyDomainServiceTests()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
        }

        [Fact]
        public void ChangeDefaultCurrency_UnsetsPreviousDefaultCurrency()
        {
            var newDefaultCurrency = new Currency(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>());
            var previousDefaultCurrency = new Currency(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), true);

            var sut = _fixture.Create<CurrencyDomainService>();

            sut.ChangeDefaultCurrency(previousDefaultCurrency, newDefaultCurrency);

            Assert.False(previousDefaultCurrency.IsDefault);
        }

        [Fact]
        public void ChangeDefaultCurrency_ThrowsException_WhenProvidedDefaultCurrencyIsNotDefault()
        {
            var newDefaultCurrency = new Currency(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>());
            var previousDefaultCurrency = new Currency(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>());

            var sut = _fixture.Create<CurrencyDomainService>();

            Assert.Throws<OperationNotAllowedException>(() => sut.ChangeDefaultCurrency(previousDefaultCurrency, newDefaultCurrency));
        }

        [Fact]
        public void ChangeDefaultCurrency_SetsNewDefaultCurrencyAsDefault()
        {
            var newDefaultCurrency = new Currency(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>());
            var previousDefaultCurrency = new Currency(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), true);

            var sut = _fixture.Create<CurrencyDomainService>();

            sut.ChangeDefaultCurrency(previousDefaultCurrency, newDefaultCurrency);

            Assert.True(newDefaultCurrency.IsDefault);
        }
    }
}
