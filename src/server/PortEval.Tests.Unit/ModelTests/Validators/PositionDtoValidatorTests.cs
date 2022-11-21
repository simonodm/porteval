using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators;
using PortEval.Domain;
using Xunit;

namespace PortEval.Tests.Unit.ModelTests.Validators
{
    public class PositionDtoValidatorTests
    {
        public static IEnumerable<object[]> ValidPositionData = new List<object[]>
        {
            new object[] { 0, 1, 1, "", 2m, 100.1m, DateTime.Parse("2020-06-30 22:17:13") },
            new object[] { 4, 1, 1, "New Note", null, null, null },
            new object[] { 4, 1, 1, "", null, null, null },
            new object[] { 0, 1, 1, "", 1m, 456.72m, DateTime.Parse("2020-02-29 11:40") }
        };

        [Theory]
        [MemberData(nameof(ValidPositionData))]
        public void Validate_ValidatesSuccessfully_WhenPositionDataIsValid(int positionId, int portfolioId,
            int instrumentId, string note, decimal? amount, decimal? price, DateTime? time)
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Build<PositionDto>()
                .With(p => p.Id, positionId)
                .With(p => p.PortfolioId, portfolioId)
                .With(p => p.InstrumentId, instrumentId)
                .With(p => p.Note, note)
                .With(p => p.Amount, amount)
                .With(p => p.Price, price)
                .With(p => p.Time, time)
                .Create();

            var sut = fixture.Create<PositionDtoValidator>();

            var validationResult = sut.Validate(position);

            Assert.True(validationResult.IsValid);
        }

        [Fact]
        public void Validate_FailsValidation_WhenPortfolioIdIsMissing()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Build<PositionDto>()
                .With(p => p.PortfolioId, 0)
                .Create();

            var sut = fixture.Create<PositionDtoValidator>();

            var validationResult = sut.Validate(position);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(position.PortfolioId));
        }

        [Fact]
        public void Validate_FailsValidation_WhenInstrumentIdIsMissing()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Build<PositionDto>()
                .With(p => p.InstrumentId, 0)
                .Create();

            var sut = fixture.Create<PositionDtoValidator>();

            var validationResult = sut.Validate(position);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(position.InstrumentId));
        }

        [Fact]
        public void Validate_FailsValidation_WhenNoteIsLongerThan255Characters()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Build<PositionDto>()
                .With(p => p.Note, string.Join(string.Empty, fixture.CreateMany<string>(50)))
                .Create();

            var sut = fixture.Create<PositionDtoValidator>();

            var validationResult = sut.Validate(position);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(position.Note));
        }

        [Fact]
        public void Validate_FailsValidation_WhenPositionIdAndAmountIsMissing()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Build<PositionDto>()
                .With(p => p.Id, 0)
                .With(p => p.Amount, (decimal?)null)
                .Create();

            var sut = fixture.Create<PositionDtoValidator>();

            var validationResult = sut.Validate(position);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(position.Amount));
        }

        [Fact]
        public void Validate_FailsValidation_WhenPositionIdAndPriceIsMissing()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Build<PositionDto>()
                .With(p => p.Id, 0)
                .With(p => p.Price, (decimal?)null)
                .Create();

            var sut = fixture.Create<PositionDtoValidator>();

            var validationResult = sut.Validate(position);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(position.Price));
        }

        [Fact]
        public void Validate_FailsValidation_WhenPositionIdAndTimeIsMissing()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Build<PositionDto>()
                .With(p => p.Id, 0)
                .With(p => p.Time, (DateTime?)null)
                .Create();

            var sut = fixture.Create<PositionDtoValidator>();

            var validationResult = sut.Validate(position);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(position.Time));
        }

        [Fact]
        public void Validate_FailsValidation_WhenPositionIdIsMissingAndAmountIsNegative()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Build<PositionDto>()
                .With(p => p.Id, 0)
                .With(p => p.Amount, -1)
                .Create();

            var sut = fixture.Create<PositionDtoValidator>();

            var validationResult = sut.Validate(position);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(position.Amount));
        }

        [Fact]
        public void Validate_FailsValidation_WhenPositionIdIsMissingAndPriceIsNegative()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Build<PositionDto>()
                .With(p => p.Id, 0)
                .With(p => p.Price, -1)
                .Create();

            var sut = fixture.Create<PositionDtoValidator>();

            var validationResult = sut.Validate(position);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(position.Price));
        }

        [Fact]
        public void Validate_FailsValidation_WhenPositionIdIsMissingAndTimeIsInTheFuture()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Build<PositionDto>()
                .With(p => p.Id, 0)
                .With(p => p.Time, DateTime.UtcNow.AddDays(1))
                .Create();

            var sut = fixture.Create<PositionDtoValidator>();

            var validationResult = sut.Validate(position);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(position.Time));
        }

        [Fact]
        public void Validate_FailsValidation_WhenPositionIdIsMissingAndTimeIsBeforeEarliestAllowedTime()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Build<PositionDto>()
                .With(p => p.Id, 0)
                .With(p => p.Time, PortEvalConstants.FinancialDataStartTime.AddDays(-1))
                .Create();

            var sut = fixture.Create<PositionDtoValidator>();

            var validationResult = sut.Validate(position);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(position.Time));
        }

        [Fact]
        public void Validate_FailsValidation_WhenPositionIdIsMissingAndPriceIsTooLarge()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Build<PositionDto>()
                .With(p => p.Id, 0)
                .With(p => p.Price, decimal.MaxValue)
                .Create();

            var sut = fixture.Create<PositionDtoValidator>();

            var validationResult = sut.Validate(position);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(position.Price));
        }

        [Fact]
        public void Validate_FailsValidation_WhenPositionIdIsMissingAndAmountIsTooLarge()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Build<PositionDto>()
                .With(p => p.Id, 0)
                .With(p => p.Amount, decimal.MaxValue)
                .Create();

            var sut = fixture.Create<PositionDtoValidator>();

            var validationResult = sut.Validate(position);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(position.Amount));
        }
    }
}