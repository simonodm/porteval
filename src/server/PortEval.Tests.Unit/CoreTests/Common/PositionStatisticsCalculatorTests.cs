using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using PortEval.Application.Core.Common;
using PortEval.Application.Core.Common.Calculators;
using PortEval.Application.Models.DTOs;
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.Common;

public class PositionStatisticsCalculatorTests
{
    private readonly IFixture _fixture;

    public PositionStatisticsCalculatorTests()
    {
        _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
    }

    [Fact]
    public void CalculateStatistics_AssignsPositionIdFromSourceDataToResult()
    {
        var data = _fixture.Create<PositionPriceListData>();

        var sut = _fixture.Create<PositionStatisticsCalculator>();

        var stats = sut.CalculateStatistics(data, DateTime.UtcNow);

        Assert.Equal(data.PositionId, stats.Id);
    }

    [Fact]
    public void CalculateStatistics_ReturnsZeroForEachMetric_WhenNoTransactionsAreAvailable()
    {
        var price = _fixture.Create<InstrumentPriceDto>();

        var positionPriceListData = _fixture
            .Build<PositionPriceListData>()
            .With(p => p.Prices, new[] { price })
            .With(p => p.Transactions, Enumerable.Empty<TransactionDto>())
            .Create();

        var sut = _fixture.Create<PositionStatisticsCalculator>();

        var stats = sut.CalculateStatistics(positionPriceListData, DateTime.UtcNow);

        Assert.Equal(0, stats.TotalPerformance);
        Assert.Equal(0, stats.LastMonthPerformance);
        Assert.Equal(0, stats.LastWeekPerformance);
        Assert.Equal(0, stats.LastDayPerformance);
        Assert.Equal(0, stats.TotalProfit);
        Assert.Equal(0, stats.LastMonthProfit);
        Assert.Equal(0, stats.LastWeekProfit);
        Assert.Equal(0, stats.LastDayProfit);
        Assert.Equal(0, stats.BreakEvenPoint);
    }

    [Fact]
    public void CalculateStatistics_ReturnsCorrectStatistics()
    {
        var time = DateTime.Parse("2022-12-01");
        var firstPrice = _fixture.Build<InstrumentPriceDto>()
            .With(p => p.Time, DateTime.Parse("2022-01-01"))
            .Create();

        var secondPrice = _fixture.Build<InstrumentPriceDto>()
            .With(p => p.Time, time)
            .Create();

        var firstTransaction = _fixture.Build<TransactionDto>()
            .With(t => t.Amount, 2)
            .With(t => t.Time, DateTime.Parse("2022-01-01 13:50"))
            .Create();

        var secondTransaction = _fixture.Build<TransactionDto>()
            .With(t => t.Amount, 4)
            .With(t => t.Time, DateTime.Parse("2022-10-04 18:19"))
            .Create();

        var transactions = new List<TransactionDto>
        {
            firstTransaction,
            secondTransaction
        };


        var positionPriceListData = _fixture
            .Build<PositionPriceListData>()
            .With(p => p.Prices, new[] { firstPrice, secondPrice })
            .With(p => p.Transactions, transactions)
            .Create();

        _fixture.CreatePositionProfitCalculatorMock();
        _fixture.CreatePositionPerformanceCalculatorMock();
        _fixture.CreatePositionBepCalculatorMock();

        var sut = _fixture.Create<PositionStatisticsCalculator>();

        var stats = sut.CalculateStatistics(positionPriceListData, time);

        Assert.Equal((firstTransaction.Amount + secondTransaction.Amount) * (secondPrice.Price - firstPrice.Price),
            stats.TotalPerformance);
        Assert.Equal((firstTransaction.Amount + secondTransaction.Amount) * (secondPrice.Price - firstPrice.Price),
            stats.LastMonthPerformance);
        Assert.Equal((firstTransaction.Amount + secondTransaction.Amount) * (secondPrice.Price - firstPrice.Price),
            stats.LastWeekPerformance);
        Assert.Equal((firstTransaction.Amount + secondTransaction.Amount) * (secondPrice.Price - firstPrice.Price),
            stats.LastDayPerformance);
        Assert.Equal((firstTransaction.Amount + secondTransaction.Amount) * (secondPrice.Price - firstPrice.Price),
            stats.TotalProfit);
        Assert.Equal((firstTransaction.Amount + secondTransaction.Amount) * (secondPrice.Price - firstPrice.Price),
            stats.LastMonthProfit);
        Assert.Equal((firstTransaction.Amount + secondTransaction.Amount) * (secondPrice.Price - firstPrice.Price),
            stats.LastWeekProfit);
        Assert.Equal((firstTransaction.Amount + secondTransaction.Amount) * (secondPrice.Price - firstPrice.Price),
            stats.LastDayProfit);
        Assert.Equal(2, stats.BreakEvenPoint);
    }
}