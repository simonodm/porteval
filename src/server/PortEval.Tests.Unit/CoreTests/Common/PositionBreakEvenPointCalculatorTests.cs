using AutoFixture;
using AutoFixture.AutoMoq;
using PortEval.Application.Models.DTOs;
using System.Linq;
using PortEval.Application.Core.Common.Calculators;
using Xunit;

namespace PortEval.Tests.Unit.FeatureTests.Common
{
    public class PositionBreakEvenPointCalculatorTests
    {
        [Fact]
        public void CalculatePositionBreakEvenPoint_ReturnsZero_WhenNoTransactionsAreProvided()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var sut = fixture.Create<PositionBreakEvenPointCalculator>();

            var result = sut.CalculatePositionBreakEvenPoint(Enumerable.Empty<TransactionDto>());

            Assert.Equal(0m, result);
        }

        [Fact]
        public void CalculatePositionBreakEvenPoint_ReturnsTransactionPrice_WhenOneTransactionIsProvided()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var transaction = fixture.Create<TransactionDto>();

            var sut = fixture.Create<PositionBreakEvenPointCalculator>();

            var result = sut.CalculatePositionBreakEvenPoint(new[] { transaction });

            Assert.Equal(transaction.Price, result);
        }

        [Fact]
        public void CalculatePositionBreakEvenPoint_ReturnsCorrectBreakEvenPoint_WhenMultipleTransactionsAreProvided()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var firstTransaction = fixture.Build<TransactionDto>()
                .With(t => t.Amount, 3)
                .With(t => t.Price, 100m)
                .Create();

            var secondTransaction = fixture.Build<TransactionDto>()
                .With(t => t.Amount, -2)
                .With(t => t.Price, 105m)
                .Create();

            var thirdTransaction = fixture.Build<TransactionDto>()
                .With(t => t.Amount, 3)
                .With(t => t.Price, 110m)
                .Create();

            var sut = fixture.Create<PositionBreakEvenPointCalculator>();

            var result =
                sut.CalculatePositionBreakEvenPoint(new[] { firstTransaction, secondTransaction, thirdTransaction });

            Assert.Equal(105m, result);
        }
    }
}
