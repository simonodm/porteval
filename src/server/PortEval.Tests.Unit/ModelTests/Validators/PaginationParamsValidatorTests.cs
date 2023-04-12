using AutoFixture;
using AutoFixture.AutoMoq;
using PortEval.Application.Models.QueryParams;
using PortEval.Application.Models.Validators;
using Xunit;

namespace PortEval.Tests.Unit.ModelTests.Validators
{
    public class PaginationParamsValidatorTests
    {
        private IFixture _fixture;

        public PaginationParamsValidatorTests()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
        }

        [Theory]
        [InlineData(200, 2)]
        [InlineData(1, 1)]
        [InlineData(299, 2)]
        public void Validate_ValidatesSuccessfully_WhenPaginationIsValid(int limit, int page)
        {
            var pagination = _fixture.Build<PaginationParams>()
                .With(p => p.Limit, limit)
                .With(p => p.Page, page)
                .Create();

            var sut = _fixture.Create<PaginationParamsValidator>();

            var validationResult = sut.Validate(pagination);

            Assert.True(validationResult.IsValid);
        }

        [Fact]
        public void Validate_FailsValidation_WhenLimitIsZero()
        {
            var pagination = _fixture.Build<PaginationParams>()
                .With(p => p.Limit, 0)
                .Create();

            var sut = _fixture.Create<PaginationParamsValidator>();

            var validationResult = sut.Validate(pagination);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(pagination.Limit));
        }

        [Fact]
        public void Validate_FailsValidation_WhenLimitIsNegative()
        {
            var pagination = _fixture.Build<PaginationParams>()
                .With(p => p.Limit, -1)
                .Create();

            var sut = _fixture.Create<PaginationParamsValidator>();

            var validationResult = sut.Validate(pagination);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(pagination.Limit));
        }

        [Fact]
        public void Validate_FailsValidation_WhenPageIsZero()
        {
            var pagination = _fixture.Build<PaginationParams>()
                .With(p => p.Page, 0)
                .Create();

            var sut = _fixture.Create<PaginationParamsValidator>();

            var validationResult = sut.Validate(pagination);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(pagination.Page));
        }

        [Fact]
        public void Validate_FailsValidation_WhenPageIsNegative()
        {
            var pagination = _fixture.Build<PaginationParams>()
                .With(p => p.Page, -1)
                .Create();

            var sut = _fixture.Create<PaginationParamsValidator>();

            var validationResult = sut.Validate(pagination);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(pagination.Page));
        }

        [Fact]
        public void Validate_FailsValidation_WhenLimitIsGreaterThan300()
        {
            var pagination = _fixture.Build<PaginationParams>()
                .With(p => p.Limit, 301)
                .Create();

            var sut = _fixture.Create<PaginationParamsValidator>();

            var validationResult = sut.Validate(pagination);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(pagination.Limit));
        }
    }
}