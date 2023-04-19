using System.Collections.Generic;
using System.Drawing;
using AutoFixture;
using AutoFixture.AutoMoq;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.DTOs.Enums;
using PortEval.Application.Models.Validators;
using Xunit;

namespace PortEval.Tests.Unit.ModelTests.Validators;

public class ChartLineDtoValidatorTests
{
    public static IEnumerable<object[]> ValidChartLineDtos = new List<object[]>
    {
        new object[] { ChartLineType.Instrument, 1, 1, null, null, Color.FromArgb(255, 255, 255) },
        new object[] { ChartLineType.Portfolio, 3, null, 2, null, Color.FromArgb(180, 0, 122) },
        new object[] { ChartLineType.Position, 7, null, 4, 1, Color.FromArgb(0, 0, 244) }
    };

    private readonly IFixture _fixture;

    public ChartLineDtoValidatorTests()
    {
        _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
    }

    [Theory]
    [MemberData(nameof(ValidChartLineDtos))]
    public void Validate_ValidatesSuccessfully_WhenLineIsValid(ChartLineType type, int width, int? instrumentId,
        int? portfolioId, int? positionId, Color color)
    {
        var line = _fixture.Build<ChartLineDto>()
            .With(line => line.Type, type)
            .With(line => line.Width, width)
            .With(line => line.Color, color)
            .With(line => line.InstrumentId, instrumentId)
            .With(line => line.PortfolioId, portfolioId)
            .With(line => line.PositionId, positionId)
            .Create();

        var sut = new ChartLineDtoValidator();

        var validationResult = sut.Validate(line);

        Assert.True(validationResult.IsValid);
    }

    [Fact]
    public void Validate_FailsValidation_WhenLineWidthIsZero()
    {
        var line = _fixture.Build<ChartLineDto>()
            .With(line => line.Width, 0)
            .Create();

        var sut = new ChartLineDtoValidator();

        var validationResult = sut.Validate(line);

        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(line.Width));
    }

    [Fact]
    public void Validate_FailsValidation_WhenLineWidthIsNegative()
    {
        var line = _fixture.Build<ChartLineDto>()
            .With(line => line.Width, -1)
            .Create();

        var sut = new ChartLineDtoValidator();

        var validationResult = sut.Validate(line);

        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(line.Width));
    }

    [Fact]
    public void Validate_FailsValidation_WhenLineWidthIsGreaterThanEight()
    {
        var line = _fixture.Build<ChartLineDto>()
            .With(line => line.Width, 9)
            .Create();

        var sut = new ChartLineDtoValidator();

        var validationResult = sut.Validate(line);

        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(line.Width));
    }

    [Fact]
    public void Validate_FailsValidation_WhenLineIsInstrumentLineAndInstrumentIdIsNotProvided()
    {
        var line = _fixture.Build<ChartLineDto>()
            .With(line => line.Type, ChartLineType.Instrument)
            .With(line => line.InstrumentId, (int?)null)
            .Create();

        var sut = new ChartLineDtoValidator();

        var validationResult = sut.Validate(line);

        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(line.InstrumentId));
    }

    [Fact]
    public void Validate_FailsValidation_WhenLineIsPortfolioLineAndPortfolioIdIsNotProvided()
    {
        var line = _fixture.Build<ChartLineDto>()
            .With(line => line.Type, ChartLineType.Portfolio)
            .With(line => line.PortfolioId, (int?)null)
            .Create();

        var sut = new ChartLineDtoValidator();

        var validationResult = sut.Validate(line);

        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(line.PortfolioId));
    }

    [Fact]
    public void Validate_FailsValidation_WhenLineIsPositionLineAndPositionIdIsNotProvided()
    {
        var line = _fixture.Build<ChartLineDto>()
            .With(line => line.Type, ChartLineType.Position)
            .With(line => line.PositionId, (int?)null)
            .Create();

        var sut = new ChartLineDtoValidator();

        var validationResult = sut.Validate(line);

        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(line.PositionId));
    }
}