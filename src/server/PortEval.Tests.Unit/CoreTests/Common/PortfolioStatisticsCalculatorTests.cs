using AutoFixture;
using AutoFixture.AutoMoq;
using PortEval.Application.Core.Common;
using PortEval.Application.Core.Common.Calculators;
using PortEval.Application.Models.DTOs;
using PortEval.Tests.Unit.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.Common
{
    public class PortfolioStatisticsCalculatorTests
    {
        private IFixture _fixture;

        public PortfolioStatisticsCalculatorTests()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
        }

        [Fact]
        public void CalculateStatistics_AssignsPortfolioIdFromSourceDataToResult()
        {
            var data = _fixture.Create<PortfolioPositionsPriceListData>();

            var sut = _fixture.Create<PortfolioStatisticsCalculator>();

            var stats = sut.CalculateStatistics(data, DateTime.UtcNow);

            Assert.Equal(data.PortfolioId, stats.Id);
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

            var portfolioData = _fixture.Build<PortfolioPositionsPriceListData>()
                .With(p => p.PositionsPriceListData, new[] { positionPriceListData })
                .Create();

            var sut = _fixture.Create<PortfolioStatisticsCalculator>();

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
            var time = DateTime.Parse("2022-12-01");

            var firstInstrumentFirstPrice = _fixture.Build<InstrumentPriceDto>()
                .With(p => p.Time, DateTime.Parse("2022-01-01"))
                .Create();
            var firstInstrumentSecondPrice = _fixture.Build<InstrumentPriceDto>()
                .With(p => p.Time, time)
                .Create();
            var firstTransaction = _fixture.Build<TransactionDto>()
                .With(t => t.Amount, 2)
                .With(t => t.Time, DateTime.Parse("2022-01-01 13:50"))
                .Create();
            var firstTransactions = new List<TransactionDto>
            {
                firstTransaction
            };

            var secondInstrumentFirstPrice = _fixture.Build<InstrumentPriceDto>()
                .With(p => p.Time, DateTime.Parse("2021-12-31"))
                .Create();
            var secondInstrumentSecondPrice = _fixture.Build<InstrumentPriceDto>()
                .With(p => p.Time, time)
                .Create();
            var secondTransaction = _fixture.Build<TransactionDto>()
                .With(t => t.Amount, 4)
                .With(t => t.Time, DateTime.Parse("2022-10-04 18:19"))
                .Create();
            var secondTransactions = new List<TransactionDto>
            {
                secondTransaction
            };

            var firstPositionPriceListData = _fixture
                .Build<PositionPriceListData>()
                .With(p => p.Prices, new[] { firstInstrumentFirstPrice, firstInstrumentSecondPrice })
                .With(p => p.Transactions, firstTransactions)
                .Create();

            var secondPositionPriceListData = _fixture
                .Build<PositionPriceListData>()
                .With(p => p.Prices, new[] { secondInstrumentFirstPrice, secondInstrumentSecondPrice })
                .With(p => p.Transactions, secondTransactions)
                .Create();

            var firstInstrumentPriceDifference = firstInstrumentSecondPrice.Price - firstInstrumentFirstPrice.Price;
            var secondInstrumentPriceDifference = secondInstrumentSecondPrice.Price - secondInstrumentFirstPrice.Price;

            var portfolioData = _fixture.Build<PortfolioPositionsPriceListData>()
                .With(p => p.PositionsPriceListData, new[] { firstPositionPriceListData, secondPositionPriceListData })
                .Create();

            _fixture.CreatePositionProfitCalculatorMock();
            _fixture.CreatePositionPerformanceCalculatorMock();

            var sut = _fixture.Create<PortfolioStatisticsCalculator>();

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
