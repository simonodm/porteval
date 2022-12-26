using PortEval.Application.Features.Common.Calculators;
using System.Collections.Generic;
using Xunit;

namespace PortEval.Tests.Unit.FeatureTests.Common
{
    public class InstrumentPerformanceCalculatorTests
    {
        [Theory]
        [MemberData(nameof(PerformanceTestData))]
        public void CalculatePerformance_ReturnsAccuratePerformance_WhenBothPricesAreProvided(decimal startPrice,
            decimal endPrice, decimal expectedPerformance)
        {
            var calculator = new InstrumentPerformanceCalculator();

            var performance = calculator.CalculatePerformance(startPrice, endPrice);

            Assert.InRange(performance, expectedPerformance - 0.01m, expectedPerformance + 0.01m);
        }

        [Fact]
        public void CalculatePerformance_Returns100Percent_WhenNoStartPriceIsProvided()
        {
            var startPrice = 0m;
            var endPrice = 114.1m;
            var calculator = new InstrumentPerformanceCalculator();

            var performance = calculator.CalculatePerformance(startPrice, endPrice);

            Assert.Equal(1, performance);
        }

        [Fact]
        public void CalculatePerformance_ReturnsZero_WhenNoPricesAreProvided()
        {
            var startPrice = 0m;
            var endPrice = 0m;
            var calculator = new InstrumentPerformanceCalculator();

            var performance = calculator.CalculatePerformance(startPrice, endPrice);

            Assert.Equal(0, performance);
        }

        public static IEnumerable<object[]> PerformanceTestData = new List<object[]>
        {
            new object[] { 115m, 200m, 0.739m },
            new object[] { 141.15m, 137.24m, -0.028m },
            new object[] { 297.15m, 297.15m, 0m },
            new object[] { 987.57m, 1001.35m, 0.014m }
        };
    }
}
