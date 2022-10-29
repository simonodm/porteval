using AutoFixture;
using AutoFixture.AutoMoq;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators;
using Xunit;

namespace PortEval.Tests.UnitTests.Models.Validators
{
    public class PortfolioDtoValidatorTests
    {
        [Theory]
        [InlineData("US stocks", "USD", "Test Note")]
        [InlineData("Test Portfolio", "EUR", "")]
        public void Validate_ValidatesSuccessfully_WhenPortfolioIsValid(string name, string currencyCode, string note)
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolio = fixture.Build<PortfolioDto>()
                .With(p => p.Name, name)
                .With(p => p.CurrencyCode, currencyCode)
                .With(p => p.Note, note)
                .Create();

            var sut = fixture.Create<PortfolioDtoValidator>();

            var validationResult = sut.Validate(portfolio);

            Assert.True(validationResult.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Validate_FailsValidation_WhenNameIsMissing(string name)
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolio = fixture.Build<PortfolioDto>()
                .With(p => p.Name, name)
                .Create();

            var sut = fixture.Create<PortfolioDtoValidator>();

            var validationResult = sut.Validate(portfolio);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(portfolio.Name));
        }

        [Fact]
        public void Validate_FailsValidation_WhenNameIsLongerThan64Characters()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolio = fixture.Build<PortfolioDto>()
                .With(p => p.Name, string.Join(string.Empty, fixture.CreateMany<string>(10)))
                .Create();

            var sut = fixture.Create<PortfolioDtoValidator>();

            var validationResult = sut.Validate(portfolio);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(portfolio.Name));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("A")]
        [InlineData("ABCD")]
        public void Validate_FailsValidation_WhenCurrencyCodeIsInvalid(string currencyCode)
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolio = fixture.Build<PortfolioDto>()
                .With(p => p.CurrencyCode, currencyCode)
                .Create();

            var sut = fixture.Create<PortfolioDtoValidator>();

            var validationResult = sut.Validate(portfolio);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(portfolio.CurrencyCode));
        }

        [Fact]
        public void Validate_FailsValidation_WhenNoteIsLongerThan255Characters()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolio = fixture.Build<PortfolioDto>()
                .With(p => p.Note, string.Join(string.Empty, fixture.CreateMany<string>(50)))
                .Create();

            var sut = fixture.Create<PortfolioDtoValidator>();

            var validationResult = sut.Validate(portfolio);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(portfolio.Note));
        }
    }
}