using AutoFixture;
using AutoFixture.AutoMoq;
using PortEval.Application.Models.QueryParams;
using PortEval.Application.Models.Validators;
using Xunit;

namespace PortEval.Tests.UnitTests.Models.Validators
{
    public class PaginationParamsValidatorTests
    {
        [Theory]
        [InlineData(200, 2)]
        [InlineData(1, 1)]
        [InlineData(299, 2)]
        public void Validate_ValidatesSuccessfully_WhenPaginationIsValid(int limit, int page)
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var pagination = fixture.Build<PaginationParams>()
                .With(p => p.Limit, limit)
                .With(p => p.Page, page)
                .Create();

            var sut = fixture.Create<PaginationParamsValidator>();

            var validationResult = sut.Validate(pagination);

            Assert.True(validationResult.IsValid);
        }

        [Fact]
        public void Validate_FailsValidation_WhenLimitIsZero()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var pagination = fixture.Build<PaginationParams>()
                .With(p => p.Limit, 0)
                .Create();

            var sut = fixture.Create<PaginationParamsValidator>();

            var validationResult = sut.Validate(pagination);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(pagination.Limit));
        }

        [Fact]
        public void Validate_FailsValidation_WhenLimitIsNegative()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var pagination = fixture.Build<PaginationParams>()
                .With(p => p.Limit, -1)
                .Create();

            var sut = fixture.Create<PaginationParamsValidator>();

            var validationResult = sut.Validate(pagination);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(pagination.Limit));
        }

        [Fact]
        public void Validate_FailsValidation_WhenPageIsZero()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var pagination = fixture.Build<PaginationParams>()
                .With(p => p.Page, 0)
                .Create();

            var sut = fixture.Create<PaginationParamsValidator>();

            var validationResult = sut.Validate(pagination);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(pagination.Page));
        }

        [Fact]
        public void Validate_FailsValidation_WhenPageIsNegative()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var pagination = fixture.Build<PaginationParams>()
                .With(p => p.Page, -1)
                .Create();

            var sut = fixture.Create<PaginationParamsValidator>();

            var validationResult = sut.Validate(pagination);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(pagination.Page));
        }

        [Fact]
        public void Validate_FailsValidation_WhenLimitIsGreaterThan300()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var pagination = fixture.Build<PaginationParams>()
                .With(p => p.Limit, 301)
                .Create();

            var sut = fixture.Create<PaginationParamsValidator>();

            var validationResult = sut.Validate(pagination);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(pagination.Limit));
        }
    }
}