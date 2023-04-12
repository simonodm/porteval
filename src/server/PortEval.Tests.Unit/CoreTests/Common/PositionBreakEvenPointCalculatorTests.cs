using AutoFixture;
using AutoFixture.AutoMoq;
using PortEval.Application.Core.Common.Calculators;
using PortEval.Application.Models.DTOs;
using System.Linq;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.Common
{
    public class PositionBreakEvenPointCalculatorTests
    {
        private IFixture _fixture;

        public PositionBreakEvenPointCalculatorTests()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
        }

        [Fact]
        public void CalculatePositionBreakEvenPoint_ReturnsZero_WhenNoTransactionsAreProvided()
        {
            var sut = _fixture.Create<PositionBreakEvenPointCalculator>();

            var result = sut.CalculatePositionBreakEvenPoint(Enumerable.Empty<TransactionDto>());

            Assert.Equal(0m, result);
        }

        [Fact]
        public void CalculatePositionBreakEvenPoint_ReturnsTransactionPrice_WhenOneTransactionIsProvided()
        {
            var transaction = _fixture.Create<TransactionDto>();

            var sut = _fixture.Create<PositionBreakEvenPointCalculator>();

            var result = sut.CalculatePositionBreakEvenPoint(new[] { transaction });

            Assert.Equal(transaction.Price, result);
        }

        [Fact]
        public void CalculatePositionBreakEvenPoint_ReturnsCorrectBreakEvenPoint_WhenMultipleTransactionsAreProvided()
        {
            var firstTransaction = _fixture.Build<TransactionDto>()
                .With(t => t.Amount, 3)
                .With(t => t.Price, 100m)
                .Create();

            var secondTransaction = _fixture.Build<TransactionDto>()
                .With(t => t.Amount, -2)
                .With(t => t.Price, 105m)
                .Create();

            var thirdTransaction = _fixture.Build<TransactionDto>()
                .With(t => t.Amount, 3)
                .With(t => t.Price, 110m)
                .Create();

            var sut = _fixture.Create<PositionBreakEvenPointCalculator>();

            var result =
                sut.CalculatePositionBreakEvenPoint(new[] { firstTransaction, secondTransaction, thirdTransaction });

            Assert.Equal(105m, result);
        }
    }
}
