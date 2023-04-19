using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Core.Common.ChartDataGenerators;
using PortEval.Application.Core.Interfaces.Calculators;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.Common;

public class InstrumentChartDataGeneratorTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IInstrumentPerformanceCalculator> _performanceCalculator;
    private readonly Mock<IInstrumentProfitCalculator> _profitCalculator;


    public InstrumentChartDataGeneratorTests()
    {
        _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
        _profitCalculator = _fixture.Freeze<Mock<IInstrumentProfitCalculator>>();
        _performanceCalculator = _fixture.Freeze<Mock<IInstrumentPerformanceCalculator>>();
    }

    [Fact]
    public void ChartPrices_ChartsPricesCorrectly()
    {
        var prices = new List<InstrumentPriceDto>
        {
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-01")).Create(),
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-02")).Create(),
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-03")).Create(),
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-04")).Create(),
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-05")).Create()
        };
        var frequency = AggregationFrequency.Day;
        var dateRange = new DateRangeParams
        {
            From = prices[0].Time,
            To = prices[4].Time
        };

        var sut = _fixture.Create<InstrumentChartDataGenerator>();

        var result = sut.ChartPrices(prices, dateRange, frequency);

        Assert.Collection(result, point =>
        {
            Assert.Equal(prices[0].Time, point.Time);
            Assert.Equal(prices[0].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[1].Time, point.Time);
            Assert.Equal(prices[1].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[2].Time, point.Time);
            Assert.Equal(prices[2].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[3].Time, point.Time);
            Assert.Equal(prices[3].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[4].Time, point.Time);
            Assert.Equal(prices[4].Price, point.Value);
        });
    }

    [Fact]
    public void ChartPrices_ChartsPricesCorrectly_WhenOnePriceAppliesToMultipleChartPoints()
    {
        var prices = new List<InstrumentPriceDto>
        {
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-01")).Create()
        };
        var frequency = AggregationFrequency.Day;
        var dateRange = new DateRangeParams
        {
            From = prices[0].Time,
            To = DateTime.Parse("2022-01-05")
        };

        var sut = _fixture.Create<InstrumentChartDataGenerator>();

        var result = sut.ChartPrices(prices, dateRange, frequency);

        Assert.Collection(result, point =>
        {
            Assert.Equal(prices[0].Time, point.Time);
            Assert.Equal(prices[0].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[0].Time.AddDays(1), point.Time);
            Assert.Equal(prices[0].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[0].Time.AddDays(2), point.Time);
            Assert.Equal(prices[0].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[0].Time.AddDays(3), point.Time);
            Assert.Equal(prices[0].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[0].Time.AddDays(4), point.Time);
            Assert.Equal(prices[0].Price, point.Value);
        });
    }

    [Fact]
    public void ChartPrices_LimitsChartRange_WhenTheEarliestPriceIsAfterTheFirstExpectedChartPoint()
    {
        var prices = new List<InstrumentPriceDto>
        {
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-03")).Create()
        };
        var frequency = AggregationFrequency.Day;
        var dateRange = new DateRangeParams
        {
            From = DateTime.Parse("2022-01-01"),
            To = DateTime.Parse("2022-01-05")
        };

        var sut = _fixture.Create<InstrumentChartDataGenerator>();

        var result = sut.ChartPrices(prices, dateRange, frequency);

        Assert.Collection(result, point =>
        {
            Assert.Equal(prices[0].Time, point.Time);
            Assert.Equal(prices[0].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[0].Time.AddDays(1), point.Time);
            Assert.Equal(prices[0].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[0].Time.AddDays(2), point.Time);
            Assert.Equal(prices[0].Price, point.Value);
        });
    }

    [Fact]
    public void ChartProfit_ChartsProfitCorrectly()
    {
        var prices = new List<InstrumentPriceDto>
        {
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-01")).Create(),
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-02")).Create(),
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-03")).Create(),
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-04")).Create(),
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-05")).Create()
        };
        var frequency = AggregationFrequency.Day;
        var dateRange = new DateRangeParams
        {
            From = prices[0].Time,
            To = prices[4].Time
        };

        _profitCalculator
            .Setup(c => c.CalculateProfit(It.IsAny<decimal>(), It.IsAny<decimal>()))
            .Returns<decimal, decimal>((first, second) => second - first);

        var sut = _fixture.Create<InstrumentChartDataGenerator>();

        var result = sut.ChartProfit(prices, dateRange, frequency);

        Assert.Collection(result, point =>
        {
            Assert.Equal(prices[0].Time, point.Time);
            Assert.Equal(0m, point.Value);
        }, point =>
        {
            Assert.Equal(prices[1].Time, point.Time);
            Assert.Equal(prices[1].Price - prices[0].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[2].Time, point.Time);
            Assert.Equal(prices[2].Price - prices[0].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[3].Time, point.Time);
            Assert.Equal(prices[3].Price - prices[0].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[4].Time, point.Time);
            Assert.Equal(prices[4].Price - prices[0].Price, point.Value);
        });
    }

    [Fact]
    public void ChartProfit_ChartsProfitCorrectly_WhenOnePriceAppliesToMultipleChartPoints()
    {
        var prices = new List<InstrumentPriceDto>
        {
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-01")).Create(),
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-02")).Create()
        };
        var frequency = AggregationFrequency.Day;
        var dateRange = new DateRangeParams
        {
            From = prices[0].Time,
            To = DateTime.Parse("2022-01-05")
        };

        _profitCalculator
            .Setup(c => c.CalculateProfit(It.IsAny<decimal>(), It.IsAny<decimal>()))
            .Returns<decimal, decimal>((first, second) => second - first);

        var sut = _fixture.Create<InstrumentChartDataGenerator>();

        var result = sut.ChartProfit(prices, dateRange, frequency);

        Assert.Collection(result, point =>
        {
            Assert.Equal(prices[0].Time, point.Time);
            Assert.Equal(0m, point.Value);
        }, point =>
        {
            Assert.Equal(prices[0].Time.AddDays(1), point.Time);
            Assert.Equal(prices[1].Price - prices[0].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[0].Time.AddDays(2), point.Time);
            Assert.Equal(prices[1].Price - prices[0].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[0].Time.AddDays(3), point.Time);
            Assert.Equal(prices[1].Price - prices[0].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[0].Time.AddDays(4), point.Time);
            Assert.Equal(prices[1].Price - prices[0].Price, point.Value);
        });
    }

    [Fact]
    public void ChartProfit_LimitsChartRange_WhenTheEarliestPriceIsAfterTheFirstExpectedChartPoint()
    {
        var prices = new List<InstrumentPriceDto>
        {
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-03")).Create()
        };
        var frequency = AggregationFrequency.Day;
        var dateRange = new DateRangeParams
        {
            From = DateTime.Parse("2022-01-01"),
            To = DateTime.Parse("2022-01-05")
        };

        _profitCalculator
            .Setup(c => c.CalculateProfit(It.IsAny<decimal>(), It.IsAny<decimal>()))
            .Returns<decimal, decimal>((first, second) => second - first);

        var sut = _fixture.Create<InstrumentChartDataGenerator>();

        var result = sut.ChartProfit(prices, dateRange, frequency);

        Assert.Collection(result, point =>
        {
            Assert.Equal(prices[0].Time, point.Time);
            Assert.Equal(0m, point.Value);
        }, point =>
        {
            Assert.Equal(prices[0].Time.AddDays(1), point.Time);
            Assert.Equal(0m, point.Value);
        }, point =>
        {
            Assert.Equal(prices[0].Time.AddDays(2), point.Time);
            Assert.Equal(0m, point.Value);
        });
    }

    [Fact]
    public void ChartPerformance_ChartsPerformanceCorrectly()
    {
        var prices = new List<InstrumentPriceDto>
        {
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-01")).Create(),
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-02")).Create(),
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-03")).Create(),
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-04")).Create(),
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-05")).Create()
        };
        var frequency = AggregationFrequency.Day;
        var dateRange = new DateRangeParams
        {
            From = prices[0].Time,
            To = prices[4].Time
        };

        _performanceCalculator
            .Setup(c => c.CalculatePerformance(It.IsAny<decimal>(), It.IsAny<decimal>()))
            .Returns<decimal, decimal>((first, second) => second / first);

        var sut = _fixture.Create<InstrumentChartDataGenerator>();

        var result = sut.ChartPerformance(prices, dateRange, frequency);

        Assert.Collection(result, point =>
        {
            Assert.Equal(prices[0].Time, point.Time);
            Assert.Equal(prices[0].Price / prices[0].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[1].Time, point.Time);
            Assert.Equal(prices[1].Price / prices[0].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[2].Time, point.Time);
            Assert.Equal(prices[2].Price / prices[0].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[3].Time, point.Time);
            Assert.Equal(prices[3].Price / prices[0].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[4].Time, point.Time);
            Assert.Equal(prices[4].Price / prices[0].Price, point.Value);
        });
    }

    [Fact]
    public void ChartPerformance_ChartsPerformanceCorrectly_WhenOnePriceAppliesToMultipleChartPoints()
    {
        var prices = new List<InstrumentPriceDto>
        {
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-01")).Create(),
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-02")).Create()
        };
        var frequency = AggregationFrequency.Day;
        var dateRange = new DateRangeParams
        {
            From = prices[0].Time,
            To = DateTime.Parse("2022-01-05")
        };

        _performanceCalculator
            .Setup(c => c.CalculatePerformance(It.IsAny<decimal>(), It.IsAny<decimal>()))
            .Returns<decimal, decimal>((first, second) => second / first);

        var sut = _fixture.Create<InstrumentChartDataGenerator>();

        var result = sut.ChartPerformance(prices, dateRange, frequency);

        Assert.Collection(result, point =>
        {
            Assert.Equal(prices[0].Time, point.Time);
            Assert.Equal(prices[0].Price / prices[0].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[0].Time.AddDays(1), point.Time);
            Assert.Equal(prices[1].Price / prices[0].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[0].Time.AddDays(2), point.Time);
            Assert.Equal(prices[1].Price / prices[0].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[0].Time.AddDays(3), point.Time);
            Assert.Equal(prices[1].Price / prices[0].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[0].Time.AddDays(4), point.Time);
            Assert.Equal(prices[1].Price / prices[0].Price, point.Value);
        });
    }

    [Fact]
    public void ChartPerformance_LimitsChartRange_WhenTheEarliestPriceIsAfterTheFirstExpectedChartPoint()
    {
        var prices = new List<InstrumentPriceDto>
        {
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-03")).Create()
        };
        var frequency = AggregationFrequency.Day;
        var dateRange = new DateRangeParams
        {
            From = DateTime.Parse("2022-01-01"),
            To = DateTime.Parse("2022-01-05")
        };

        _performanceCalculator
            .Setup(c => c.CalculatePerformance(It.IsAny<decimal>(), It.IsAny<decimal>()))
            .Returns<decimal, decimal>((first, second) => second / first);

        var sut = _fixture.Create<InstrumentChartDataGenerator>();

        var result = sut.ChartPerformance(prices, dateRange, frequency);

        Assert.Collection(result, point =>
        {
            Assert.Equal(prices[0].Time, point.Time);
            Assert.Equal(1m, point.Value);
        }, point =>
        {
            Assert.Equal(prices[0].Time.AddDays(1), point.Time);
            Assert.Equal(1m, point.Value);
        }, point =>
        {
            Assert.Equal(prices[0].Time.AddDays(2), point.Time);
            Assert.Equal(1m, point.Value);
        });
    }

    [Fact]
    public void ChartAggregatedProfit_ChartsAggregatedProfitCorrectly()
    {
        var prices = new List<InstrumentPriceDto>
        {
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-01")).Create(),
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-02")).Create(),
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-03")).Create(),
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-04")).Create(),
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-05")).Create()
        };
        var frequency = AggregationFrequency.Day;
        var dateRange = new DateRangeParams
        {
            From = prices[0].Time,
            To = prices[4].Time
        };

        _profitCalculator
            .Setup(c => c.CalculateProfit(It.IsAny<decimal>(), It.IsAny<decimal>()))
            .Returns<decimal, decimal>((first, second) => second - first);

        var sut = _fixture.Create<InstrumentChartDataGenerator>();

        var result = sut.ChartAggregatedProfit(prices, dateRange, frequency);

        Assert.Collection(result, point =>
        {
            Assert.Equal(prices[1].Time, point.Time);
            Assert.Equal(prices[1].Price - prices[0].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[2].Time, point.Time);
            Assert.Equal(prices[2].Price - prices[1].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[3].Time, point.Time);
            Assert.Equal(prices[3].Price - prices[2].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[4].Time, point.Time);
            Assert.Equal(prices[4].Price - prices[3].Price, point.Value);
        });
    }

    [Fact]
    public void ChartAggregatedProfit_ChartsAggregatedProfitCorrectly_WhenOnePriceAppliesToMultipleChartPoints()
    {
        var prices = new List<InstrumentPriceDto>
        {
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-01")).Create(),
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-02")).Create()
        };
        var frequency = AggregationFrequency.Day;
        var dateRange = new DateRangeParams
        {
            From = prices[0].Time,
            To = DateTime.Parse("2022-01-05")
        };

        _profitCalculator
            .Setup(c => c.CalculateProfit(It.IsAny<decimal>(), It.IsAny<decimal>()))
            .Returns<decimal, decimal>((first, second) => second - first);

        var sut = _fixture.Create<InstrumentChartDataGenerator>();

        var result = sut.ChartAggregatedProfit(prices, dateRange, frequency);

        Assert.Collection(result, point =>
        {
            Assert.Equal(prices[0].Time.AddDays(1), point.Time);
            Assert.Equal(prices[1].Price - prices[0].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[0].Time.AddDays(2), point.Time);
            Assert.Equal(0m, point.Value);
        }, point =>
        {
            Assert.Equal(prices[0].Time.AddDays(3), point.Time);
            Assert.Equal(0m, point.Value);
        }, point =>
        {
            Assert.Equal(prices[0].Time.AddDays(4), point.Time);
            Assert.Equal(0m, point.Value);
        });
    }

    [Fact]
    public void ChartAggregatedProfit_LimitsChartRange_WhenTheEarliestPriceIsAfterTheFirstExpectedChartPoint()
    {
        var prices = new List<InstrumentPriceDto>
        {
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-03")).Create()
        };
        var frequency = AggregationFrequency.Day;
        var dateRange = new DateRangeParams
        {
            From = DateTime.Parse("2022-01-01"),
            To = DateTime.Parse("2022-01-05")
        };

        _profitCalculator
            .Setup(c => c.CalculateProfit(It.IsAny<decimal>(), It.IsAny<decimal>()))
            .Returns<decimal, decimal>((first, second) => second - first);

        var sut = _fixture.Create<InstrumentChartDataGenerator>();

        var result = sut.ChartAggregatedProfit(prices, dateRange, frequency);

        Assert.Collection(result, point =>
        {
            Assert.Equal(prices[0].Time.AddDays(1), point.Time);
            Assert.Equal(0m, point.Value);
        }, point =>
        {
            Assert.Equal(prices[0].Time.AddDays(2), point.Time);
            Assert.Equal(0m, point.Value);
        });
    }

    [Fact]
    public void ChartAggregatedPerformance_ChartsAggregatedPerformanceCorrectly()
    {
        var prices = new List<InstrumentPriceDto>
        {
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-01")).Create(),
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-02")).Create(),
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-03")).Create(),
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-04")).Create(),
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-05")).Create()
        };
        var frequency = AggregationFrequency.Day;
        var dateRange = new DateRangeParams
        {
            From = prices[0].Time,
            To = prices[4].Time
        };

        _performanceCalculator
            .Setup(c => c.CalculatePerformance(It.IsAny<decimal>(), It.IsAny<decimal>()))
            .Returns<decimal, decimal>((first, second) => first != 0 ? second / first : 0);

        var sut = _fixture.Create<InstrumentChartDataGenerator>();

        var result = sut.ChartAggregatedPerformance(prices, dateRange, frequency);

        Assert.Collection(result, point =>
        {
            Assert.Equal(prices[1].Time, point.Time);
            Assert.Equal(prices[1].Price / prices[0].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[2].Time, point.Time);
            Assert.Equal(prices[2].Price / prices[1].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[3].Time, point.Time);
            Assert.Equal(prices[3].Price / prices[2].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[4].Time, point.Time);
            Assert.Equal(prices[4].Price / prices[3].Price, point.Value);
        });
    }

    [Fact]
    public void
        ChartAggregatedPerformance_ChartsAggregatedPerformanceCorrectly_WhenOnePriceAppliesToMultipleChartPoints()
    {
        var prices = new List<InstrumentPriceDto>
        {
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-01")).Create(),
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-02")).Create()
        };
        var frequency = AggregationFrequency.Day;
        var dateRange = new DateRangeParams
        {
            From = prices[0].Time,
            To = DateTime.Parse("2022-01-05")
        };

        _performanceCalculator
            .Setup(c => c.CalculatePerformance(It.IsAny<decimal>(), It.IsAny<decimal>()))
            .Returns<decimal, decimal>((first, second) => first != 0 ? second / first : 0m);

        var sut = _fixture.Create<InstrumentChartDataGenerator>();

        var result = sut.ChartAggregatedPerformance(prices, dateRange, frequency);

        Assert.Collection(result, point =>
        {
            Assert.Equal(prices[0].Time.AddDays(1), point.Time);
            Assert.Equal(prices[1].Price / prices[0].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[0].Time.AddDays(2), point.Time);
            Assert.Equal(prices[1].Price / prices[1].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[0].Time.AddDays(3), point.Time);
            Assert.Equal(prices[1].Price / prices[1].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[0].Time.AddDays(4), point.Time);
            Assert.Equal(prices[1].Price / prices[1].Price, point.Value);
        });
    }

    [Fact]
    public void ChartAggregatedPerformance_LimitsChartRange_WhenTheEarliestPriceIsAfterTheFirstExpectedChartPoint()
    {
        var prices = new List<InstrumentPriceDto>
        {
            _fixture.Build<InstrumentPriceDto>().With(p => p.Time, DateTime.Parse("2022-01-03")).Create()
        };
        var frequency = AggregationFrequency.Day;
        var dateRange = new DateRangeParams
        {
            From = DateTime.Parse("2022-01-01"),
            To = DateTime.Parse("2022-01-05")
        };

        _performanceCalculator
            .Setup(c => c.CalculatePerformance(It.IsAny<decimal>(), It.IsAny<decimal>()))
            .Returns<decimal, decimal>((first, second) => first != 0 ? second / first : 0m);

        var sut = _fixture.Create<InstrumentChartDataGenerator>();

        var result = sut.ChartAggregatedPerformance(prices, dateRange, frequency);

        Assert.Collection(result, point =>
        {
            Assert.Equal(prices[0].Time.AddDays(1), point.Time);
            Assert.Equal(prices[0].Price / prices[0].Price, point.Value);
        }, point =>
        {
            Assert.Equal(prices[0].Time.AddDays(2), point.Time);
            Assert.Equal(prices[0].Price / prices[0].Price, point.Value);
        });
    }
}