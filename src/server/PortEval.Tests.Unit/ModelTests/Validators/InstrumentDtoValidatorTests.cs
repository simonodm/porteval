using AutoFixture;
using AutoFixture.AutoMoq;
using FluentValidation;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators;
using System.Collections.Generic;
using Xunit;

namespace PortEval.Tests.Unit.ModelTests.Validators
{
    public class InstrumentDtoValidatorTests
    {
        private IFixture _fixture;

        public InstrumentDtoValidatorTests()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
        }

        public static IEnumerable<object[]> ValidInstrumentData = new List<object[]>
        {
            new object[] { "AAPL", "Apple Inc.", "NASDAQ", "USD", "Test Note" },
            new object[] { "TWTR", "Twitter Inc.", "NYSE", "EUR", "" },
            new object[] { "GME", "Gamestop Inc.", "NYSE", "CZK", "A" }
        };

        [Theory]
        [MemberData(nameof(ValidInstrumentData))]
        public void Validate_ValidatesSuccessfully_WhenInstrumentIsValid(string symbol, string name, string exchange,
            string currencyCode, string note)
        {
            var instrument = _fixture.Build<InstrumentDto>()
                .With(i => i.Symbol, symbol)
                .With(i => i.Name, name)
                .With(i => i.Exchange, exchange)
                .With(i => i.CurrencyCode, currencyCode)
                .With(i => i.Note, note)
                .Create();

            var sut = _fixture.Create<InstrumentDtoValidator>();

            var validationResult = sut.Validate(instrument);

            Assert.True(validationResult.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("A")]
        [InlineData("ABCD")]
        public void Validate_FailsValidation_WhenCurrencyCodeIsNot3Characters(string currencyCode)
        {
            var instrument = _fixture.Build<InstrumentDto>()
                .With(i => i.CurrencyCode, currencyCode)
                .Create();

            var sut = _fixture.Create<InstrumentDtoValidator>();
            sut.ClassLevelCascadeMode = CascadeMode.Continue;

            var validationResult = sut.Validate(instrument);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(instrument.CurrencyCode));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("ABCDEFGHIJK")]
        public void Validate_FailsValidation_WhenSymbolIsInvalid(string symbol)
        {
            var instrument = _fixture.Build<InstrumentDto>()
                .With(i => i.Symbol, symbol)
                .Create();

            var sut = _fixture.Create<InstrumentDtoValidator>();
            sut.ClassLevelCascadeMode = CascadeMode.Continue;

            var validationResult = sut.Validate(instrument);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(instrument.Symbol));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Validate_FailsValidation_WhenNameIsMissing(string name)
        {
            var instrument = _fixture.Build<InstrumentDto>()
                .With(i => i.Name, name)
                .Create();

            var sut = _fixture.Create<InstrumentDtoValidator>();
            sut.ClassLevelCascadeMode = CascadeMode.Continue;

            var validationResult = sut.Validate(instrument);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(instrument.Name));
        }

        [Fact]
        public void Validate_FailsValidation_WhenNameIsLongerThan64Characters()
        {
            var instrument = _fixture.Build<InstrumentDto>()
                .With(i => i.Name, string.Join(string.Empty, _fixture.CreateMany<string>(10)))
                .Create();

            var sut = _fixture.Create<InstrumentDtoValidator>();
            sut.ClassLevelCascadeMode = CascadeMode.Continue;

            var validationResult = sut.Validate(instrument);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(instrument.Name));
        }

        [Fact]
        public void Validate_FailsValidation_WhenExchangeIsLongerThan32Characters()
        {
            var instrument = _fixture.Build<InstrumentDto>()
                .With(i => i.Exchange, string.Join(string.Empty, _fixture.CreateMany<string>(5)))
                .Create();

            var sut = _fixture.Create<InstrumentDtoValidator>();
            sut.ClassLevelCascadeMode = CascadeMode.Continue;

            var validationResult = sut.Validate(instrument);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(instrument.Exchange));
        }

        [Fact]
        public void Validate_FailsValidation_WhenNoteIsLongerThan255Characters()
        {
            var instrument = _fixture.Build<InstrumentDto>()
                .With(i => i.Note, string.Join(string.Empty, _fixture.CreateMany<string>(50)))
                .Create();

            var sut = _fixture.Create<InstrumentDtoValidator>();
            sut.ClassLevelCascadeMode = CascadeMode.Continue;

            var validationResult = sut.Validate(instrument);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(instrument.Note));
        }
    }
}