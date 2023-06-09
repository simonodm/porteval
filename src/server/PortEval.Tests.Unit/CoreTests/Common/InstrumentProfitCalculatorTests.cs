﻿using PortEval.Application.Core.Common.Calculators;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.Common;

public class InstrumentProfitCalculatorTests
{
    [Theory]
    [InlineData(100, 200, 100)]
    [InlineData(0, 150, 150)]
    [InlineData(250, 100, -150)]
    public void CalculateProfit_ReturnsAccurateProfit(decimal startPrice, decimal endPrice, decimal expectedProfit)
    {
        var calculator = new InstrumentProfitCalculator();

        var profit = calculator.CalculateProfit(startPrice, endPrice);

        Assert.Equal(expectedProfit, profit);
    }
}