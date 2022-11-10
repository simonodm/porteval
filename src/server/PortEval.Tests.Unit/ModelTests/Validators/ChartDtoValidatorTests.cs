using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators;
using PortEval.Domain;
using PortEval.Domain.Models.Enums;
using PortEval.Domain.Models.ValueObjects;
using Xunit;

namespace PortEval.Tests.UnitTests.Models.Validators
{
    public class ChartDtoValidatorTests
    {
        public static IEnumerable<object[]> ValidCharts => new List<object[]>
        {
            new object[]
            {
                ChartType.Price,
                true,
                null,
                null,
                DateRangeUnit.DAY,
                3,
                null
            },
            new object[]
            {
                ChartType.Profit,
                false,
                DateTime.Parse("2021-01-01"),
                DateTime.Parse("2022-06-01"),
                null,
                null,
                null
            },
            new object[]
            {
                ChartType.Performance,
                false,
                DateTime.Parse("2022-01-03 12:00"),
                DateTime.Parse("2022-01-06 18:35"),
                null,
                null,
                null
            },
            new object[]
            {
                ChartType.AggregatedProfit,
                true,
                null,
                null,
                DateRangeUnit.YEAR,
                1,
                AggregationFrequency.Week
            },
            new object[]
            {
                ChartType.AggregatedPerformance,
                true,
                null,
                null,
                DateRangeUnit.WEEK,
                2,
                AggregationFrequency.Hour
            },
            new object[]
            {
                ChartType.Price,
                true,
                null,
                null,
                DateRangeUnit.MONTH,
                4,
                null
            }
        };

        [Theory]
        [MemberData(nameof(ValidCharts))]
        public void Validate_ValidatesSuccessfully_WhenProvidedChartIsValid(ChartType type, bool isToDate,
            DateTime? dateRangeStart, DateTime? dateRangeEnd, DateRangeUnit? dateRangeUnit, int? dateRangeValue,
            AggregationFrequency? aggregationFrequency)
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.CurrencyCode, "USD")
                .With(c => c.Type, type)
                .With(c => c.IsToDate, isToDate)
                .With(c => c.DateRangeStart, dateRangeStart)
                .With(c => c.DateRangeEnd, dateRangeEnd)
                .With(c => c.Frequency, aggregationFrequency)
                .With(c => c.ToDateRange,
                    dateRangeUnit == null || dateRangeValue == null
                        ? null
                        : new ToDateRange((DateRangeUnit)dateRangeUnit, (int)dateRangeValue))
                .With(c => c.Lines, new List<ChartLineDto>())
                .Create();

            var sut = fixture.Create<ChartDtoValidator>();

            var validationResult = sut.Validate(chart);

            Assert.True(validationResult.IsValid);
        }

        [Fact]
        public void Validate_FailsValidation_WhenChartIsToDateAndToDateRangeIsNotProvided()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.IsToDate, true)
                .With(c => c.ToDateRange, (ToDateRange)null)
                .Create();

            var sut = fixture.Create<ChartDtoValidator>();

            var validationResult = sut.Validate(chart);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(chart.ToDateRange));
        }

        [Fact]
        public void Validate_FailsValidation_WhenChartIsNotToDateAndDateRangeStartIsNotProvided()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.IsToDate, false)
                .With(c => c.DateRangeStart, (DateTime?)null)
                .Create();

            var sut = fixture.Create<ChartDtoValidator>();

            var validationResult = sut.Validate(chart);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(chart.DateRangeStart));
        }

        [Fact]
        public void Validate_FailsValidation_WhenChartIsNotToDateAndDateRangeEndIsNotProvided()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.IsToDate, false)
                .With(c => c.DateRangeEnd, (DateTime?)null)
                .Create();

            var sut = fixture.Create<ChartDtoValidator>();

            var validationResult = sut.Validate(chart);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(chart.DateRangeEnd));
        }

        [Theory]
        [InlineData(ChartType.AggregatedProfit)]
        [InlineData(ChartType.AggregatedPerformance)]
        public void Validate_FailsValidation_WhenChartIsAggregatedAndAggregationFrequencyIsNotProvided(
            ChartType chartType)
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.Type, chartType)
                .With(c => c.Frequency, (AggregationFrequency?)null)
                .Create();

            var sut = fixture.Create<ChartDtoValidator>();

            var validationResult = sut.Validate(chart);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(chart.Frequency));
        }

        [Theory]
        [InlineData(ChartType.Price)]
        [InlineData(ChartType.Profit)]
        [InlineData(ChartType.AggregatedProfit)]
        public void Validate_FailsValidation_WhenChartIsCurrencyChartAndCurrencyCodeIsNotProvided(ChartType chartType)
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.Type, chartType)
                .With(c => c.CurrencyCode, (string)null)
                .Create();

            var sut = fixture.Create<ChartDtoValidator>();

            var validationResult = sut.Validate(chart);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(chart.CurrencyCode));
        }

        [Fact]
        public void Validate_FailsValidation_WhenChartDateRangeIsInTheFuture()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.IsToDate, false)
                .With(c => c.DateRangeStart, DateTime.UtcNow)
                .With(c => c.DateRangeEnd, DateTime.UtcNow.AddDays(1))
                .Create();

            var sut = fixture.Create<ChartDtoValidator>();

            var validationResult = sut.Validate(chart);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(chart.DateRangeEnd));
        }

        [Fact]
        public void Validate_FailsValidation_WhenChartDateRangeEndIsBeforeStart()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.IsToDate, false)
                .With(c => c.DateRangeStart, DateTime.UtcNow.AddDays(-1))
                .With(c => c.DateRangeEnd, DateTime.UtcNow.AddDays(-2))
                .Create();

            var sut = fixture.Create<ChartDtoValidator>();

            var validationResult = sut.Validate(chart);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(chart.DateRangeEnd));
        }

        [Fact]
        public void Validate_FailsValidation_WhenChartDateRangeEndIsBeforeAllowedStartTime()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.IsToDate, false)
                .With(c => c.DateRangeStart, PortEvalConstants.FinancialDataStartTime.AddDays(-1))
                .With(c => c.DateRangeEnd, DateTime.UtcNow.AddDays(-2))
                .Create();

            var sut = fixture.Create<ChartDtoValidator>();

            var validationResult = sut.Validate(chart);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(chart.DateRangeStart));
        }

        [Theory]
        [InlineData("")]
        [InlineData("A")]
        [InlineData("ABCD")]
        public void Validate_FailsValidation_WhenChartCurrencyIsInvalid(string currencyCode)
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.Type, ChartType.Price)
                .With(c => c.CurrencyCode, currencyCode)
                .Create();

            var sut = fixture.Create<ChartDtoValidator>();

            var validationResult = sut.Validate(chart);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(chart.CurrencyCode));
        }

        [Theory]
        [InlineData("")]
        [InlineData("A")]
        public void Validate_FailsValidation_WhenChartNameIsShorterThanThreeCharacters(string name)
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.Name, name)
                .Create();

            var sut = fixture.Create<ChartDtoValidator>();

            var validationResult = sut.Validate(chart);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(chart.Name));
        }

        [Fact]
        public void Validate_FailsValidation_WhenChartNameIsLongerThanSixtyFourCharacters()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.Name, string.Join(string.Empty, fixture.CreateMany<string>(10)))
                .Create();

            var sut = fixture.Create<ChartDtoValidator>();

            var validationResult = sut.Validate(chart);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(chart.Name));
        }
    }
}