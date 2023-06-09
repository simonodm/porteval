﻿using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators;
using PortEval.Domain;
using Xunit;

namespace PortEval.Tests.Unit.ModelTests.Validators;

public class InstrumentPriceDtoValidatorTests
{
    public static IEnumerable<object[]> ValidInstrumentPriceData = new List<object[]>
    {
        new object[] { 1, DateTime.Parse("2022-01-01"), 100 },
        new object[] { 3, DateTime.Parse("2022-07-13 14:45"), 112 }
    };

    private readonly IFixture _fixture;

    public InstrumentPriceDtoValidatorTests()
    {
        _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
    }

    [Theory]
    [MemberData(nameof(ValidInstrumentPriceData))]
    public void Validate_ValidatesSuccessfully_WhenPriceIsValid(int instrumentId, DateTime time, decimal price)
    {
        var priceDto = _fixture.Build<InstrumentPriceDto>()
            .With(p => p.InstrumentId, instrumentId)
            .With(p => p.Time, time)
            .With(p => p.Price, price)
            .Create();

        var sut = _fixture.Create<InstrumentPriceDtoValidator>();

        var validationResult = sut.Validate(priceDto);

        Assert.True(validationResult.IsValid);
    }

    [Fact]
    public void Validate_FailsValidation_WhenInstrumentIdIsMissing()
    {
        var priceDto = _fixture.Build<InstrumentPriceDto>()
            .With(p => p.InstrumentId, 0)
            .Create();

        var sut = _fixture.Create<InstrumentPriceDtoValidator>();

        var validationResult = sut.Validate(priceDto);

        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(priceDto.InstrumentId));
    }

    [Fact]
    public void Validate_FailsValidation_WhenPriceIsZero()
    {
        var priceDto = _fixture.Build<InstrumentPriceDto>()
            .With(p => p.Price, 0)
            .Create();

        var sut = _fixture.Create<InstrumentPriceDtoValidator>();

        var validationResult = sut.Validate(priceDto);

        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(priceDto.Price));
    }

    [Fact]
    public void Validate_FailsValidation_WhenPriceIsNegative()
    {
        var priceDto = _fixture.Build<InstrumentPriceDto>()
            .With(p => p.Price, -1)
            .Create();

        var sut = _fixture.Create<InstrumentPriceDtoValidator>();

        var validationResult = sut.Validate(priceDto);

        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(priceDto.Price));
    }

    [Fact]
    public void Validate_FailsValidation_WhenPriceIsTooLarge()
    {
        var priceDto = _fixture.Build<InstrumentPriceDto>()
            .With(p => p.Price, decimal.MaxValue)
            .Create();

        var sut = _fixture.Create<InstrumentPriceDtoValidator>();

        var validationResult = sut.Validate(priceDto);

        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(priceDto.Price));
    }

    [Fact]
    public void Validate_FailsValidation_WhenTimeIsInTheFuture()
    {
        var priceDto = _fixture.Build<InstrumentPriceDto>()
            .With(p => p.Time, DateTime.MaxValue)
            .Create();

        var sut = _fixture.Create<InstrumentPriceDtoValidator>();

        var validationResult = sut.Validate(priceDto);

        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(priceDto.Time));
    }

    [Fact]
    public void Validate_FailsValidation_WhenTimeIsBeforeEarliestAllowedTime()
    {
        var priceDto = _fixture.Build<InstrumentPriceDto>()
            .With(p => p.Time, PortEvalConstants.FinancialDataStartTime.AddDays(-1))
            .Create();

        var sut = _fixture.Create<InstrumentPriceDtoValidator>();

        var validationResult = sut.Validate(priceDto);

        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(priceDto.Time));
    }
}