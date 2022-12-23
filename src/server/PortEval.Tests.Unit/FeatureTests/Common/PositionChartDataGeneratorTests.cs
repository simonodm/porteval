using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Features.Common;
using PortEval.Application.Features.Interfaces.Calculators;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PortEval.Tests.Unit.FeatureTests.Common
{
    public class PositionChartDataGeneratorTests
    {
        [Fact]
        public void ChartValue_ChartsValueCorrectly()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var data = GenerateTestData(fixture);
            var dateRange = new DateRangeParams
            {
                From = DateTime.Parse("2022-01-01"),
                To = DateTime.Parse("2022-01-05")
            };
            var frequency = AggregationFrequency.Day;

            var valueCalculator = fixture.Freeze<Mock<IPositionValueCalculator>>();

            valueCalculator
                .Setup(m => m.CalculateValue(
                    It.IsAny<IEnumerable<PositionPriceData>>(),
                    It.IsAny<DateTime>()))
                .Returns<IEnumerable<PositionPriceData>, DateTime>(
                    (positionsPriceData, dt) =>
                        positionsPriceData.Sum(p => p.Transactions.Where(t => t.Time <= dt).Sum(t => t.Amount * p.Price.Price)
                    )
                );

            var sut = fixture.Create<PositionChartDataGenerator>();

            var result = sut.ChartValue(data, dateRange, frequency);

            Assert.Collection(result, point =>
            {
                Assert.Equal(dateRange.From, point.Time);
                Assert.Equal(300, point.Value);
            }, point =>
            {
                Assert.Equal(dateRange.From.AddDays(1), point.Time);
                Assert.Equal(300, point.Value);
            }, point =>
            {
                Assert.Equal(dateRange.From.AddDays(2), point.Time);
                Assert.Equal(300, point.Value);
            }, point =>
            {
                Assert.Equal(dateRange.From.AddDays(3), point.Time);
                Assert.Equal(1050, point.Value);
            }, point =>
            {
                Assert.Equal(dateRange.From.AddDays(4), point.Time);
                Assert.Equal(1050, point.Value);
            });
        }

        [Fact]
        public void ChartValue_LimitsChartRangeToFirstAvailableTransaction()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var data = GenerateTestData(fixture);
            var dateRange = new DateRangeParams
            {
                From = DateTime.Parse("2021-01-01"),
                To = DateTime.Parse("2022-01-05")
            };
            var expectedChartStartDate = DateTime.Parse("2022-01-01");
            var frequency = AggregationFrequency.Day;

            fixture.Freeze<Mock<IPositionValueCalculator>>();

            var sut = fixture.Create<PositionChartDataGenerator>();

            var result = sut.ChartValue(data, dateRange, frequency);

            Assert.Equal(expectedChartStartDate, result.First().Time);
        }

        [Fact]
        public void ChartProfit_ChartsProfitCorrectly()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var data = GenerateTestData(fixture);
            var dateRange = new DateRangeParams
            {
                From = DateTime.Parse("2022-01-01"),
                To = DateTime.Parse("2022-01-05")
            };
            var frequency = AggregationFrequency.Day;

            var profitCalculator = fixture.Freeze<Mock<IPositionProfitCalculator>>();

            // CalculateProfit is mocked to return the total amount of transactions which occurred before the time at which the profit is calculated,
            // multiplied by the difference between the end price and start price of the position
            profitCalculator
                .Setup(m => m.CalculateProfit(
                    It.IsAny<IEnumerable<PositionPriceRangeData>>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>()))
                .Returns<IEnumerable<PositionPriceRangeData>, DateTime, DateTime>(
                    (positionsPriceData, from, to) =>
                        positionsPriceData.Sum(p => p.Transactions
                            .Where(t => t.Time <= to)
                            .Sum(t => t.Amount * (p.PriceAtRangeEnd.Price - p.PriceAtRangeStart.Price))
                    )
                );

            var sut = fixture.Create<PositionChartDataGenerator>();

            var result = sut.ChartProfit(data, dateRange, frequency);

            Assert.Collection(result, point =>
            {
                Assert.Equal(dateRange.From, point.Time);
                Assert.Equal(0, point.Value);
            }, point =>
            {
                Assert.Equal(dateRange.From.AddDays(1), point.Time);
                Assert.Equal(0, point.Value);
            }, point =>
            {
                Assert.Equal(dateRange.From.AddDays(2), point.Time);
                Assert.Equal(0, point.Value);
            }, point =>
            {
                Assert.Equal(dateRange.From.AddDays(3), point.Time);
                Assert.Equal(350, point.Value);
            }, point =>
            {
                Assert.Equal(dateRange.From.AddDays(4), point.Time);
                Assert.Equal(350, point.Value);
            });
        }

        [Fact]
        public void ChartProfit_LimitsChartRangeToFirstAvailableTransaction()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var data = GenerateTestData(fixture);
            var dateRange = new DateRangeParams
            {
                From = DateTime.Parse("2021-01-01"),
                To = DateTime.Parse("2022-01-05")
            };
            var expectedChartStartDate = DateTime.Parse("2022-01-01");
            var frequency = AggregationFrequency.Day;

            fixture.Freeze<Mock<IPositionProfitCalculator>>();

            var sut = fixture.Create<PositionChartDataGenerator>();

            var result = sut.ChartProfit(data, dateRange, frequency);

            Assert.Equal(expectedChartStartDate, result.First().Time);
        }

        [Fact]
        public void ChartPerformance_ChartsPerformanceCorrectly()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var data = GenerateTestData(fixture);
            var dateRange = new DateRangeParams
            {
                From = DateTime.Parse("2022-01-01"),
                To = DateTime.Parse("2022-01-05")
            };
            var frequency = AggregationFrequency.Day;

            var performanceCalculator = fixture.Freeze<Mock<IPositionPerformanceCalculator>>();

            // CalculatePerformance is mocked to return the total number of transactions which occurred before the time at which the performance is calculated
            performanceCalculator
                .Setup(m => m.CalculatePerformance(
                    It.IsAny<IEnumerable<PositionPriceRangeData>>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>()))
                .Returns<IEnumerable<PositionPriceRangeData>, DateTime, DateTime>(
                    (positionsPriceData, from, to) =>
                        positionsPriceData.Sum(p => p.Transactions.Count(t => t.Time <= to)
                    )
                );

            var sut = fixture.Create<PositionChartDataGenerator>();

            var result = sut.ChartPerformance(data, dateRange, frequency);

            Assert.Collection(result, point =>
            {
                Assert.Equal(dateRange.From, point.Time);
                Assert.Equal(1, point.Value);
            }, point =>
            {
                Assert.Equal(dateRange.From.AddDays(1), point.Time);
                Assert.Equal(1, point.Value);
            }, point =>
            {
                Assert.Equal(dateRange.From.AddDays(2), point.Time);
                Assert.Equal(1, point.Value);
            }, point =>
            {
                Assert.Equal(dateRange.From.AddDays(3), point.Time);
                Assert.Equal(2, point.Value);
            }, point =>
            {
                Assert.Equal(dateRange.From.AddDays(4), point.Time);
                Assert.Equal(2, point.Value);
            });
        }

        [Fact]
        public void ChartPerformance_LimitsChartRangeToFirstAvailableTransaction()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var data = GenerateTestData(fixture);
            var dateRange = new DateRangeParams
            {
                From = DateTime.Parse("2021-01-01"),
                To = DateTime.Parse("2022-01-05")
            };
            var expectedChartStartDate = DateTime.Parse("2022-01-01");
            var frequency = AggregationFrequency.Day;

            fixture.Freeze<Mock<IPositionPerformanceCalculator>>();

            var sut = fixture.Create<PositionChartDataGenerator>();

            var result = sut.ChartPerformance(data, dateRange, frequency);

            Assert.Equal(expectedChartStartDate, result.First().Time);
        }

        [Fact]
        public void ChartAggregatedProfit_ChartsAggregatedProfitCorrectly()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var data = GenerateTestData(fixture);
            var dateRange = new DateRangeParams
            {
                From = DateTime.Parse("2022-01-01"),
                To = DateTime.Parse("2022-01-05")
            };
            var frequency = AggregationFrequency.Day;

            var profitCalculator = fixture.Freeze<Mock<IPositionProfitCalculator>>();

            // CalculateProfit is mocked to return the total number of transactions which occurred in the range in which the profit is calculated
            profitCalculator
                .Setup(m => m.CalculateProfit(
                    It.IsAny<IEnumerable<PositionPriceRangeData>>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>()))
                .Returns<IEnumerable<PositionPriceRangeData>, DateTime, DateTime>(
                    (positionsPriceData, from, to) =>
                        positionsPriceData.Sum(p => p.Transactions.Count(t => t.Time >= from && t.Time < to)
                    )
                );

            var sut = fixture.Create<PositionChartDataGenerator>();

            var result = sut.ChartAggregatedProfit(data, dateRange, frequency);

            Assert.Collection(result, point =>
            {
                Assert.Equal(dateRange.From.AddDays(1), point.Time);
                Assert.Equal(1, point.Value);
            }, point =>
            {
                Assert.Equal(dateRange.From.AddDays(2), point.Time);
                Assert.Equal(0, point.Value);
            }, point =>
            {
                Assert.Equal(dateRange.From.AddDays(3), point.Time);
                Assert.Equal(0, point.Value);
            }, point =>
            {
                Assert.Equal(dateRange.From.AddDays(4), point.Time);
                Assert.Equal(1, point.Value);
            });
        }

        [Fact]
        public void ChartAggregatedProfit_LimitsChartRangeToFirstAvailableTransaction()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var data = GenerateTestData(fixture);
            var dateRange = new DateRangeParams
            {
                From = DateTime.Parse("2021-01-01"),
                To = DateTime.Parse("2022-01-05")
            };
            var expectedChartStartDate = DateTime.Parse("2022-01-02");
            var frequency = AggregationFrequency.Day;

            fixture.Freeze<Mock<IPositionProfitCalculator>>();

            var sut = fixture.Create<PositionChartDataGenerator>();

            var result = sut.ChartAggregatedProfit(data, dateRange, frequency);

            Assert.Equal(expectedChartStartDate, result.First().Time);
        }

        private IEnumerable<PositionPriceListData> GenerateTestData(IFixture fixture)
        {
            var firstTestTransactions = new List<TransactionDto>
            {
                fixture
                    .Build<TransactionDto>()
                    .With(t => t.Time, DateTime.Parse("2022-01-01"))
                    .With(t => t.Amount, 3)
                    .Create()
            };

            var secondTestTransactions = new List<TransactionDto>
            {
                fixture
                    .Build<TransactionDto>()
                    .With(t => t.Time, DateTime.Parse("2022-01-04"))
                    .With(t => t.Amount, 2)
                    .Create()
            };

            var firstInstrumentTestPrices = new List<InstrumentPriceDto>
            {
                fixture.Build<InstrumentPriceDto>()
                    .With(p => p.InstrumentId, firstTestTransactions[0].Instrument.Id)
                    .With(p => p.Time, DateTime.Parse("2022-01-01"))
                    .With(p => p.Price, 100m)
                    .Create(),
                fixture.Build<InstrumentPriceDto>()
                    .With(p => p.InstrumentId, firstTestTransactions[0].Instrument.Id)
                    .With(p => p.Time, DateTime.Parse("2022-01-04"))
                    .With(p => p.Price, 150m)
                    .Create()
            };

            var secondInstrumentTestPrices = new List<InstrumentPriceDto>
            {
                fixture.Build<InstrumentPriceDto>()
                    .With(p => p.InstrumentId, secondTestTransactions[0].Instrument.Id)
                    .With(p => p.Time, DateTime.Parse("2022-01-01"))
                    .With(p => p.Price, 200m)
                    .Create(),
                fixture.Build<InstrumentPriceDto>()
                    .With(p => p.InstrumentId, secondTestTransactions[0].Instrument.Id)
                    .With(p => p.Time, DateTime.Parse("2022-01-04"))
                    .With(p => p.Price, 300m)
                    .Create()
            };

            return new[]
            {
                new PositionPriceListData
                {
                    Transactions = firstTestTransactions,
                    Prices = firstInstrumentTestPrices
                },
                new PositionPriceListData
                {
                    Transactions = secondTestTransactions,
                    Prices = secondInstrumentTestPrices
                }
            };
        }
    }
}
