using AutoFixture;
using AutoFixture.AutoMoq;
using PortEval.Application.Models.QueryParams;
using PortEval.Application.Models.Validators;
using PortEval.Domain;
using System;
using System.Collections.Generic;
using Xunit;

namespace PortEval.Tests.Unit.ModelTests.Validators
{
    public class DateRangeParamsValidatorTests
    {
        private IFixture _fixture;

        public DateRangeParamsValidatorTests()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
        }

        public static IEnumerable<object[]> ValidDateRanges = new List<object[]>
        {
            new object[] { DateTime.Parse("2022-01-01"), DateTime.Parse("2022-01-02") },
            new object[] { DateTime.Parse("2004-06-12 13:33"), DateTime.Parse("2022-01-02") },
            new object[] { DateTime.Parse("2022-07-31 12:01"), DateTime.Parse("2022-07-31 14:00") },
            new object[] { DateTime.Parse("2016-02-29"), DateTime.Parse("2016-03-01") },
            new object[] { DateTime.Parse("2001-05-05 23:59"), DateTime.Parse("2001-05-06") }
        };

        [Theory]
        [MemberData(nameof(ValidDateRanges))]
        public void Validate_ValidatesSuccessfully_WhenProvidedDateRangeIsValid(DateTime from, DateTime to)
        {
            var dateRange = _fixture.Build<DateRangeParams>()
                .With(dr => dr.From, from)
                .With(dr => dr.To, to)
                .Create();

            var validator = _fixture.Create<DateRangeParamsValidator>();

            var validationResult = validator.Validate(dateRange);

            Assert.True(validationResult.IsValid);
        }

        [Fact]
        public void Validate_FailsValidation_WhenDateRangeStartIsLaterThanEnd()
        {
            var dateRange = _fixture.Build<DateRangeParams>()
                .With(dr => dr.From, DateTime.UtcNow)
                .With(dr => dr.To, DateTime.MinValue)
                .Create();

            var validator = _fixture.Create<DateRangeParamsValidator>();

            var validationResult = validator.Validate(dateRange);

            Assert.False(validationResult.IsValid);
        }

        [Fact]
        public void Validate_FailsValidation_WhenDateRangeIncludesFutureDate()
        {
            var dateRange = _fixture.Build<DateRangeParams>()
                .With(dr => dr.From, DateTime.UtcNow)
                .With(dr => dr.To, DateTime.MaxValue)
                .Create();

            var validator = _fixture.Create<DateRangeParamsValidator>();

            var validationResult = validator.Validate(dateRange);

            Assert.False(validationResult.IsValid);
        }

        [Fact]
        public void Validate_FailsValidation_WhenDateRangeIsBeforeAllowedStartTime()
        {
            var dateRange = _fixture.Build<DateRangeParams>()
                .With(dr => dr.From, PortEvalConstants.FinancialDataStartTime.AddDays(-10))
                .With(dr => dr.To, PortEvalConstants.FinancialDataStartTime.AddDays(1))
                .Create();

            var validator = _fixture.Create<DateRangeParamsValidator>();

            var validationResult = validator.Validate(dateRange);

            Assert.False(validationResult.IsValid);
        }
    }
}