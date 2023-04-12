using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Core.Common;
using PortEval.Application.Core.Common.ChartDataGenerators;
using PortEval.Application.Core.Interfaces.Calculators;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;
using PortEval.Tests.Unit.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.Common
{
    public class PositionChartDataGeneratorTests
    {
        private IFixture _fixture;
        private Mock<IPositionValueCalculator> _valueCalculator;
        private Mock<IPositionProfitCalculator> _profitCalculator;
        private Mock<IPositionPerformanceCalculator> _performanceCalculator;

        public PositionChartDataGeneratorTests()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            _valueCalculator = _fixture.Freeze<Mock<IPositionValueCalculator>>();
            _profitCalculator = _fixture.CreatePositionProfitCalculatorMock();
            _performanceCalculator = _fixture.CreatePositionPerformanceCalculatorMock();
        }

        [Fact]
        public void ChartValue_ChartsValueCorrectly()
        {
            var data = GenerateTestData(_fixture);
            var dateRange = new DateRangeParams
            {
                From = DateTime.Parse("2022-01-01"),
                To = DateTime.Parse("2022-01-05")
            };
            var frequency = AggregationFrequency.Day;

            _valueCalculator
                .Setup(m => m.CalculateValue(
                    It.IsAny<IEnumerable<PositionPriceRangeData>>(),
                    It.IsAny<DateTime>()))
                .Returns<IEnumerable<PositionPriceRangeData>, DateTime>(
                    (positionsPriceData, dt) =>
                        positionsPriceData.Sum(p => p.Transactions.Where(t => t.Time <= dt).Sum(t => t.Amount * p.PriceAtRangeEnd.Price)
                    )
                );

            var sut = _fixture.Create<PositionChartDataGenerator>();

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
                Assert.Equal(450, point.Value);
            }, point =>
            {
                Assert.Equal(dateRange.From.AddDays(4), point.Time);
                Assert.Equal(450, point.Value);
            });
        }

        [Fact]
        public void ChartValue_LimitsChartRangeToFirstAvailableTransaction()
        {
            var data = GenerateTestData(_fixture);
            var dateRange = new DateRangeParams
            {
                From = DateTime.Parse("2021-01-01"),
                To = DateTime.Parse("2022-01-05")
            };
            var expectedChartStartDate = DateTime.Parse("2022-01-01");
            var frequency = AggregationFrequency.Day;

            var sut = _fixture.Create<PositionChartDataGenerator>();

            var result = sut.ChartValue(data, dateRange, frequency);

            Assert.Equal(expectedChartStartDate, result.First().Time);
        }

        [Fact]
        public void ChartProfit_ChartsProfitCorrectly()
        {
            var data = GenerateTestData(_fixture);
            var dateRange = new DateRangeParams
            {
                From = DateTime.Parse("2022-01-01"),
                To = DateTime.Parse("2022-01-05")
            };
            var frequency = AggregationFrequency.Day;

            var sut = _fixture.Create<PositionChartDataGenerator>();

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
                Assert.Equal(150, point.Value);
            }, point =>
            {
                Assert.Equal(dateRange.From.AddDays(4), point.Time);
                Assert.Equal(150, point.Value);
            });
        }

        [Fact]
        public void ChartProfit_LimitsChartRangeToFirstAvailableTransaction()
        {
            var data = GenerateTestData(_fixture);
            var dateRange = new DateRangeParams
            {
                From = DateTime.Parse("2021-01-01"),
                To = DateTime.Parse("2022-01-05")
            };
            var expectedChartStartDate = DateTime.Parse("2022-01-01");
            var frequency = AggregationFrequency.Day;

            var sut = _fixture.Create<PositionChartDataGenerator>();

            var result = sut.ChartProfit(data, dateRange, frequency);

            Assert.Equal(expectedChartStartDate, result.First().Time);
        }

        [Fact]
        public void ChartPerformance_ChartsPerformanceCorrectly()
        {
            var data = GenerateTestData(_fixture);
            var dateRange = new DateRangeParams
            {
                From = DateTime.Parse("2022-01-01"),
                To = DateTime.Parse("2022-01-05")
            };
            var frequency = AggregationFrequency.Day;

            var sut = _fixture.Create<PositionChartDataGenerator>();

            var result = sut.ChartPerformance(data, dateRange, frequency);

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
                Assert.Equal(150, point.Value);
            }, point =>
            {
                Assert.Equal(dateRange.From.AddDays(4), point.Time);
                Assert.Equal(150, point.Value);
            });
        }

        [Fact]
        public void ChartPerformance_LimitsChartRangeToFirstAvailableTransaction()
        {
            var data = GenerateTestData(_fixture);
            var dateRange = new DateRangeParams
            {
                From = DateTime.Parse("2021-01-01"),
                To = DateTime.Parse("2022-01-05")
            };
            var expectedChartStartDate = DateTime.Parse("2022-01-01");
            var frequency = AggregationFrequency.Day;

            var sut = _fixture.Create<PositionChartDataGenerator>();

            var result = sut.ChartPerformance(data, dateRange, frequency);

            Assert.Equal(expectedChartStartDate, result.First().Time);
        }

        [Fact]
        public void ChartAggregatedProfit_ChartsAggregatedProfitCorrectly()
        {
            var data = GenerateTestData(_fixture);
            var dateRange = new DateRangeParams
            {
                From = DateTime.Parse("2022-01-01"),
                To = DateTime.Parse("2022-01-05")
            };
            var frequency = AggregationFrequency.Day;

            var sut = _fixture.Create<PositionChartDataGenerator>();

            var result = sut.ChartAggregatedProfit(data, dateRange, frequency);

            Assert.Collection(result, point =>
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
                Assert.Equal(150, point.Value);
            }, point =>
            {
                Assert.Equal(dateRange.From.AddDays(4), point.Time);
                Assert.Equal(0, point.Value);
            });
        }

        [Fact]
        public void ChartAggregatedProfit_LimitsChartRangeToFirstAvailableTransaction()
        {
            var data = GenerateTestData(_fixture);
            var dateRange = new DateRangeParams
            {
                From = DateTime.Parse("2021-01-01"),
                To = DateTime.Parse("2022-01-05")
            };
            var expectedChartStartDate = DateTime.Parse("2022-01-02");
            var frequency = AggregationFrequency.Day;

            var sut = _fixture.Create<PositionChartDataGenerator>();

            var result = sut.ChartAggregatedProfit(data, dateRange, frequency);

            Assert.Equal(expectedChartStartDate, result.First().Time);
        }

        [Fact]
        public void ChartAggregatedPerformance_ChartsAggregatedPerformanceCorrectly()
        {
            var data = GenerateTestData(_fixture);
            var dateRange = new DateRangeParams
            {
                From = DateTime.Parse("2022-01-01"),
                To = DateTime.Parse("2022-01-05")
            };
            var frequency = AggregationFrequency.Day;

            var sut = _fixture.Create<PositionChartDataGenerator>();

            var result = sut.ChartAggregatedPerformance(data, dateRange, frequency);

            Assert.Collection(result, point =>
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
                Assert.Equal(150, point.Value);
            }, point =>
            {
                Assert.Equal(dateRange.From.AddDays(4), point.Time);
                Assert.Equal(0, point.Value);
            });
        }

        [Fact]
        public void ChartAggregatedPerformance_LimitsChartRangeToFirstAvailableTransaction()
        {
            var data = GenerateTestData(_fixture);
            var dateRange = new DateRangeParams
            {
                From = DateTime.Parse("2021-01-01"),
                To = DateTime.Parse("2022-01-05")
            };
            var expectedChartStartDate = DateTime.Parse("2022-01-02");
            var frequency = AggregationFrequency.Day;

            var sut = _fixture.Create<PositionChartDataGenerator>();

            var result = sut.ChartAggregatedPerformance(data, dateRange, frequency);

            Assert.Equal(expectedChartStartDate, result.First().Time);
        }

        private PositionPriceListData GenerateTestData(IFixture _fixture)
        {
            var transactions = new List<TransactionDto>
            {
                _fixture
                    .Build<TransactionDto>()
                    .With(t => t.Time, DateTime.Parse("2022-01-01"))
                    .With(t => t.Amount, 3)
                    .Create()
            };

            var instrumentPrices = new List<InstrumentPriceDto>
            {
                _fixture.Build<InstrumentPriceDto>()
                    .With(p => p.InstrumentId, transactions[0].Instrument.Id)
                    .With(p => p.Time, DateTime.Parse("2022-01-01"))
                    .With(p => p.Price, 100m)
                    .Create(),
                _fixture.Build<InstrumentPriceDto>()
                    .With(p => p.InstrumentId, transactions[0].Instrument.Id)
                    .With(p => p.Time, DateTime.Parse("2022-01-04"))
                    .With(p => p.Price, 150m)
                    .Create()
            };

            return _fixture.Build<PositionPriceListData>()
                .With(p => p.Transactions, transactions)
                .With(p => p.Prices, instrumentPrices)
                .With(p => p.PositionId, transactions[0].PositionId)
                .Create();
        }
    }
}
