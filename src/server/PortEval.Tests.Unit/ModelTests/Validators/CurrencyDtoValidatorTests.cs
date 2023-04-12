using AutoFixture;
using AutoFixture.AutoMoq;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators;
using System.Collections.Generic;
using Xunit;

namespace PortEval.Tests.Unit.ModelTests.Validators
{
    public class CurrencyDtoValidatorTests
    {
        private IFixture _fixture;

        public CurrencyDtoValidatorTests()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
        }

        public static IEnumerable<object[]> ValidCurrencies = new List<object[]>
        {
            new object[] { "US Dollar", "USD", "$" },
            new object[] { "Euro", "EUR", "€" },
            new object[] { "Czech Crown", "CZK", "Kč" }
        };

        [Theory]
        [MemberData(nameof(ValidCurrencies))]
        public void Validate_ValidatesSuccessfully_WhenCurrenciesAreValid(string name, string code, string symbol)
        {
            var currency = _fixture.Build<CurrencyDto>()
                .With(c => c.Name, name)
                .With(c => c.Code, code)
                .With(c => c.Symbol, symbol)
                .Create();
            var validator = _fixture.Create<CurrencyDtoValidator>();

            var validationResult = validator.Validate(currency);

            Assert.True(validationResult.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("A")]
        [InlineData("ABCD")]
        public void Validate_FailsValidation_WhenCurrencyCodeIsInvalid(string currencyCode)
        {
            var currency = _fixture.Build<CurrencyDto>()
                .With(c => c.Code, currencyCode)
                .Create();
            var validator = _fixture.Create<CurrencyDtoValidator>();

            var validationResult = validator.Validate(currency);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(currency.Code));
        }

        [Fact]
        public void Validate_FailsValidation_WhenSymbolIsLongerThanFourCharacters()
        {
            var currency = _fixture.Build<CurrencyDto>()
                .With(c => c.Code, "USD")
                .With(c => c.Symbol, "ABCDE")
                .Create();
            var validator = _fixture.Create<CurrencyDtoValidator>();

            var validationResult = validator.Validate(currency);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(currency.Symbol));
        }

        [Fact]
        public void Validate_FailsValidation_WhenNameIsLongerThanSixtyFourCharacters()
        {
            var currency = _fixture.Build<CurrencyDto>()
                .With(c => c.Code, "USD")
                .With(c => c.Symbol, "$")
                .With(c => c.Name, string.Join(string.Empty, _fixture.CreateMany<string>(10)))
                .Create();
            var validator = _fixture.Create<CurrencyDtoValidator>();

            var validationResult = validator.Validate(currency);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(currency.Name));
        }
    }
}