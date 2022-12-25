using AutoFixture;
using AutoFixture.AutoMoq;
using PortEval.Application.Features.Common;
using PortEval.Application.Features.Common.Calculators;
using PortEval.Application.Models.DTOs;
using PortEval.Tests.Unit.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PortEval.Tests.Unit.FeatureTests.Common
{
    public class PortfolioStatisticsCalculatorTests
    {
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

            var portfolioData = fixture.Build<PortfolioPositionsPriceListData>()
                .With(p => p.PositionsPriceListData, new[] { positionPriceListData })
                .Create();

            var sut = fixture.Create<PortfolioStatisticsCalculator>();

            var stats = sut.CalculateStatistics(portfolioData, DateTime.UtcNow);

            Assert.Equal(0, stats.TotalPerformance);
            Assert.Equal(0, stats.LastMonthPerformance);
            Assert.Equal(0, stats.LastWeekPerformance);
            Assert.Equal(0, stats.LastDayPerformance);
            Assert.Equal(0, stats.TotalProfit);
            Assert.Equal(0, stats.LastMonthProfit);
            Assert.Equal(0, stats.LastWeekProfit);
            Assert.Equal(0, stats.LastDayProfit);
        }

        [Fact]
        public void CalculateStatistics_ReturnsCorrectStatistics()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var time = DateTime.Parse("2022-12-01");

            var firstInstrumentFirstPrice = fixture.Build<InstrumentPriceDto>()
                .With(p => p.Time, DateTime.Parse("2022-01-01"))
                .Create();
            var firstInstrumentSecondPrice = fixture.Build<InstrumentPriceDto>()
                .With(p => p.Time, time)
                .Create();
            var firstTransaction = fixture.Build<TransactionDto>()
                .With(t => t.Amount, 2)
                .With(t => t.Time, DateTime.Parse("2022-01-01 13:50"))
                .Create();
            var firstTransactions = new List<TransactionDto>
            {
                firstTransaction
            };

            var secondInstrumentFirstPrice = fixture.Build<InstrumentPriceDto>()
                .With(p => p.Time, DateTime.Parse("2021-12-31"))
                .Create();
            var secondInstrumentSecondPrice = fixture.Build<InstrumentPriceDto>()
                .With(p => p.Time, time)
                .Create();
            var secondTransaction = fixture.Build<TransactionDto>()
                .With(t => t.Amount, 4)
                .With(t => t.Time, DateTime.Parse("2022-10-04 18:19"))
                .Create();
            var secondTransactions = new List<TransactionDto>
            {
                secondTransaction
            };

            var firstPositionPriceListData = fixture
                .Build<PositionPriceListData>()
                .With(p => p.Prices, new[] { firstInstrumentFirstPrice, firstInstrumentSecondPrice })
                .With(p => p.Transactions, firstTransactions)
                .Create();

            var secondPositionPriceListData = fixture
                .Build<PositionPriceListData>()
                .With(p => p.Prices, new[] { secondInstrumentFirstPrice, secondInstrumentSecondPrice })
                .With(p => p.Transactions, secondTransactions)
                .Create();

            var firstInstrumentPriceDifference = firstInstrumentSecondPrice.Price - firstInstrumentFirstPrice.Price;
            var secondInstrumentPriceDifference = secondInstrumentSecondPrice.Price - secondInstrumentFirstPrice.Price;

            var portfolioData = fixture.Build<PortfolioPositionsPriceListData>()
                .With(p => p.PositionsPriceListData, new[] { firstPositionPriceListData, secondPositionPriceListData })
                .Create();

            fixture.CreatePositionProfitCalculatorMock();
            fixture.CreatePositionPerformanceCalculatorMock();

            var sut = fixture.Create<PortfolioStatisticsCalculator>();

            var stats = sut.CalculateStatistics(portfolioData, time);

            Assert.Equal(firstTransaction.Amount * firstInstrumentPriceDifference + secondTransaction.Amount * secondInstrumentPriceDifference, stats.TotalPerformance);
            Assert.Equal(firstTransaction.Amount * firstInstrumentPriceDifference + secondTransaction.Amount * secondInstrumentPriceDifference, stats.LastMonthPerformance);
            Assert.Equal(firstTransaction.Amount * firstInstrumentPriceDifference + secondTransaction.Amount * secondInstrumentPriceDifference, stats.LastWeekPerformance);
            Assert.Equal(firstTransaction.Amount * firstInstrumentPriceDifference + secondTransaction.Amount * secondInstrumentPriceDifference, stats.LastDayPerformance);
            Assert.Equal(firstTransaction.Amount * firstInstrumentPriceDifference + secondTransaction.Amount * secondInstrumentPriceDifference, stats.TotalProfit);
            Assert.Equal(firstTransaction.Amount * firstInstrumentPriceDifference + secondTransaction.Amount * secondInstrumentPriceDifference, stats.LastMonthProfit);
            Assert.Equal(firstTransaction.Amount * firstInstrumentPriceDifference + secondTransaction.Amount * secondInstrumentPriceDifference, stats.LastWeekProfit);
            Assert.Equal(firstTransaction.Amount * firstInstrumentPriceDifference + secondTransaction.Amount * secondInstrumentPriceDifference, stats.LastDayProfit);
        }
    }
}
