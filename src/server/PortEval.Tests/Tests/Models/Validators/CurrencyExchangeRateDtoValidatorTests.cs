using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators;
using PortEval.Domain;
using Xunit;

namespace PortEval.Tests.Tests.Models.Validators
{
    public class CurrencyExchangeRateDtoValidatorTests
    {
        [Theory]
        [MemberData(nameof(ValidExchangeRates))]
        public void Validate_ValidatesSuccessfully_WhenExchangeRateIsValid(string currencyFrom, string currencyTo,
            decimal exchangeRateValue, DateTime time)
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var exchangeRate = fixture.Build<CurrencyExchangeRateDto>()
                .With(c => c.CurrencyFromCode, currencyFrom)
                .With(c => c.CurrencyToCode, currencyTo)
                .With(c => c.ExchangeRate, exchangeRateValue)
                .With(c => c.Time, time)
                .Create();

            var validator = fixture.Create<CurrencyExchangeRateDtoValidator>();

            var validationResult = validator.Validate(exchangeRate);

            Assert.True(validationResult.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("A")]
        [InlineData("aaaa")]
        public void Validate_FailsValidation_WhenCurrencyFromCodeIsInvalid(string currencyFromCode)
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var exchangeRate = fixture.Build<CurrencyExchangeRateDto>()
                .With(c => c.CurrencyFromCode, currencyFromCode)
                .Create();

            var validator = fixture.Create<CurrencyExchangeRateDtoValidator>();

            var validationResult = validator.Validate(exchangeRate);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(exchangeRate.CurrencyFromCode));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("A")]
        [InlineData("aaaa")]
        public void Validate_FailsValidation_WhenCurrencyToCodeIsInvalid(string currencyToCode)
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var exchangeRate = fixture.Build<CurrencyExchangeRateDto>()
                .With(c => c.CurrencyToCode, currencyToCode)
                .Create();

            var validator = fixture.Create<CurrencyExchangeRateDtoValidator>();

            var validationResult = validator.Validate(exchangeRate);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(exchangeRate.CurrencyToCode));
        }

        [Fact]
        public void Validate_FailsValidation_WhenTimeIsInTheFuture()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var exchangeRate = fixture.Build<CurrencyExchangeRateDto>()
                .With(c => c.Time, DateTime.MaxValue)
                .Create();

            var validator = fixture.Create<CurrencyExchangeRateDtoValidator>();

            var validationResult = validator.Validate(exchangeRate);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(exchangeRate.Time));
        }

        [Fact]
        public void Validate_FailsValidation_WhenTimeIsBeforeAllowedStartTime()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var exchangeRate = fixture.Build<CurrencyExchangeRateDto>()
                .With(c => c.Time, PortEvalConstants.FinancialDataStartTime.AddDays(-1))
                .Create();

            var validator = fixture.Create<CurrencyExchangeRateDtoValidator>();

            var validationResult = validator.Validate(exchangeRate);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(exchangeRate.Time));
        }

        [Fact]
        public void Validate_FailsValidation_WhenExchangeRateIsZero()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var exchangeRate = fixture.Build<CurrencyExchangeRateDto>()
                .With(c => c.ExchangeRate, 0)
                .Create();

            var validator = fixture.Create<CurrencyExchangeRateDtoValidator>();

            var validationResult = validator.Validate(exchangeRate);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(exchangeRate.ExchangeRate));
        }

        [Fact]
        public void Validate_FailsValidation_WhenExchangeRateIsNegative()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var exchangeRate = fixture.Build<CurrencyExchangeRateDto>()
                .With(c => c.ExchangeRate, -1)
                .Create();

            var validator = fixture.Create<CurrencyExchangeRateDtoValidator>();

            var validationResult = validator.Validate(exchangeRate);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(exchangeRate.ExchangeRate));
        }

        public static IEnumerable<object[]> ValidExchangeRates = new List<object[]>()
        {
            new object[] { "USD", "EUR", 1.04, DateTime.Parse("2022-01-01 12:00") },
            new object[] { "EUR", "USD", 1.0, DateTime.Parse("2022-10-27 00:00") },
            new object[] { "USD", "CZK", 25.21, DateTime.Parse("2022-10-17") }
        };
    }
}
