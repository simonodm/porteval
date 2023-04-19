using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators;
using Xunit;

namespace PortEval.Tests.Unit.ModelTests.Validators;

public class InstrumentSplitDtoValidatorTests
{
    public static IEnumerable<object[]> ValidInstrumentSplitData = new List<object[]>
    {
        new object[] { 1, 1, 5, DateTime.Parse("2022-01-01") },
        new object[] { 2, 3, 7, DateTime.Parse("2022-09-14 12:13") }
    };

    private readonly IFixture _fixture;

    public InstrumentSplitDtoValidatorTests()
    {
        _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
    }

    [Theory]
    [MemberData(nameof(ValidInstrumentSplitData))]
    public void Validate_ValidatesSuccessfully_WhenSplitIsValid(int instrumentId, int denominator, int numerator,
        DateTime time)
    {
        var splitDto = _fixture.Build<InstrumentSplitDto>()
            .With(p => p.InstrumentId, instrumentId)
            .With(p => p.Time, time)
            .With(p => p.SplitRatioDenominator, denominator)
            .With(p => p.SplitRatioNumerator, numerator)
            .Create();

        var sut = _fixture.Create<InstrumentSplitDtoValidator>();

        var validationResult = sut.Validate(splitDto);

        Assert.True(validationResult.IsValid);
    }

    [Fact]
    public void Validate_FailsValidation_WhenDenominatorIsZero()
    {
        var splitDto = _fixture.Build<InstrumentSplitDto>()
            .With(p => p.SplitRatioDenominator, 0)
            .Create();

        var sut = _fixture.Create<InstrumentSplitDtoValidator>();

        var validationResult = sut.Validate(splitDto);

        Assert.False(validationResult.IsValid);
    }

    [Fact]
    public void Validate_FailsValidation_WhenNumeratorIsZero()
    {
        var splitDto = _fixture.Build<InstrumentSplitDto>()
            .With(p => p.SplitRatioNumerator, 0)
            .Create();

        var sut = _fixture.Create<InstrumentSplitDtoValidator>();

        var validationResult = sut.Validate(splitDto);

        Assert.False(validationResult.IsValid);
    }

    [Fact]
    public void Validate_FailsValidation_WhenTimeIsBeforeEarliestAllowedTime()
    {
        var splitDto = _fixture.Build<InstrumentSplitDto>()
            .With(p => p.Time, DateTime.Parse("1999-01-01"))
            .Create();

        var sut = _fixture.Create<InstrumentSplitDtoValidator>();

        var validationResult = sut.Validate(splitDto);

        Assert.False(validationResult.IsValid);
    }

    [Fact]
    public void Validate_FailsValidation_WhenTimeIsInTheFuture()
    {
        var splitDto = _fixture.Build<InstrumentSplitDto>()
            .With(p => p.Time, DateTime.UtcNow.AddDays(1))
            .Create();

        var sut = _fixture.Create<InstrumentSplitDtoValidator>();

        var validationResult = sut.Validate(splitDto);

        Assert.False(validationResult.IsValid);
    }

    [Fact]
    public void Validate_FailsValidation_WhenInstrumentIdIsMissing()
    {
        var splitDto = _fixture.Build<InstrumentSplitDto>()
            .With(p => p.InstrumentId, 0)
            .Create();

        var sut = _fixture.Create<InstrumentSplitDtoValidator>();

        var validationResult = sut.Validate(splitDto);

        Assert.False(validationResult.IsValid);
    }

    [Fact]
    public void Validate_FailsValidation_WhenDenominatorIsTooLarge()
    {
        var splitDto = _fixture.Build<InstrumentSplitDto>()
            .With(p => p.SplitRatioDenominator, int.MaxValue)
            .Create();

        var sut = _fixture.Create<InstrumentSplitDtoValidator>();

        var validationResult = sut.Validate(splitDto);

        Assert.False(validationResult.IsValid);
    }

    [Fact]
    public void Validate_FailsValidation_WhenNumeratorIsTooLarge()
    {
        var splitDto = _fixture.Build<InstrumentSplitDto>()
            .With(p => p.SplitRatioNumerator, int.MaxValue)
            .Create();

        var sut = _fixture.Create<InstrumentSplitDtoValidator>();

        var validationResult = sut.Validate(splitDto);

        Assert.False(validationResult.IsValid);
    }
}