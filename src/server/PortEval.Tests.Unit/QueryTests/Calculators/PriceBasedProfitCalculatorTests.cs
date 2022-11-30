﻿using PortEval.Application.Services.Queries.Calculators;
using Xunit;

namespace PortEval.Tests.Unit.QueryTests.Calculators
{
    public class PriceBasedProfitCalculatorTests
    {
        [Theory]
        [InlineData(100, 200, 100)]
        [InlineData(0, 150, 150)]
        [InlineData(250, 100, -150)]

        public void CalculateProfit_ReturnsAccurateProfit(decimal startPrice, decimal endPrice, decimal expectedProfit)
        {
            var calculator = new PriceBasedProfitCalculator();

            var profit = calculator.CalculateProfit(startPrice, endPrice);

            Assert.Equal(expectedProfit, profit);
        }
    }
}
