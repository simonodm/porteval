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

namespace PortEval.Tests.Unit.CoreTests.Common
{
    public class PositionStatisticsCalculatorTests
    {
        [Fact]
        public void CalculateStatistics_AssignsPositionIdFromSourceDataToResult()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var data = fixture.Create<PositionPriceListData>();

            var sut = fixture.Create<PositionStatisticsCalculator>();

            var stats = sut.CalculateStatistics(data, DateTime.UtcNow);

            Assert.Equal(data.PositionId, stats.Id);
        }

        [Fact]
        public void CalculateStatistics_ReturnsZeroForEachMetric_WhenNoTransactionsAreAvailable()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var price = fixture.Create<InstrumentPriceDto>();

            var positionPriceListData = fixture
                .Build<PositionPriceListData>()
                .With(p => p.Prices, new[] { price })
                .With(p => p.Transactions, Enumerable.Empty<TransactionDto>())
                .Create();

            var sut = fixture.Create<PositionStatisticsCalculator>();

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
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var time = DateTime.Parse("2022-12-01");
            var firstPrice = fixture.Build<InstrumentPriceDto>()
                .With(p => p.Time, DateTime.Parse("2022-01-01"))
                .Create();

            var secondPrice = fixture.Build<InstrumentPriceDto>()
                .With(p => p.Time, time)
                .Create();

            var firstTransaction = fixture.Build<TransactionDto>()
                .With(t => t.Amount, 2)
                .With(t => t.Time, DateTime.Parse("2022-01-01 13:50"))
                .Create();

            var secondTransaction = fixture.Build<TransactionDto>()
                .With(t => t.Amount, 4)
                .With(t => t.Time, DateTime.Parse("2022-10-04 18:19"))
                .Create();

            var transactions = new List<TransactionDto>
            {
                firstTransaction,
                secondTransaction
            };


            var positionPriceListData = fixture
                .Build<PositionPriceListData>()
                .With(p => p.Prices, new[] { firstPrice, secondPrice })
                .With(p => p.Transactions, transactions)
                .Create();

            fixture.CreatePositionProfitCalculatorMock();
            fixture.CreatePositionPerformanceCalculatorMock();
            fixture.CreatePositionBEPCalculatorMock();

            var sut = fixture.Create<PositionStatisticsCalculator>();

            var stats = sut.CalculateStatistics(positionPriceListData, time);

            Assert.Equal((firstTransaction.Amount + secondTransaction.Amount) * (secondPrice.Price - firstPrice.Price), stats.TotalPerformance);
            Assert.Equal((firstTransaction.Amount + secondTransaction.Amount) * (secondPrice.Price - firstPrice.Price), stats.LastMonthPerformance);
            Assert.Equal((firstTransaction.Amount + secondTransaction.Amount) * (secondPrice.Price - firstPrice.Price), stats.LastWeekPerformance);
            Assert.Equal((firstTransaction.Amount + secondTransaction.Amount) * (secondPrice.Price - firstPrice.Price), stats.LastDayPerformance);
            Assert.Equal((firstTransaction.Amount + secondTransaction.Amount) * (secondPrice.Price - firstPrice.Price), stats.TotalProfit);
            Assert.Equal((firstTransaction.Amount + secondTransaction.Amount) * (secondPrice.Price - firstPrice.Price), stats.LastMonthProfit);
            Assert.Equal((firstTransaction.Amount + secondTransaction.Amount) * (secondPrice.Price - firstPrice.Price), stats.LastWeekProfit);
            Assert.Equal((firstTransaction.Amount + secondTransaction.Amount) * (secondPrice.Price - firstPrice.Price), stats.LastDayProfit);
            Assert.Equal(2, stats.BreakEvenPoint);
        }
    }
}
