using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Core;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Core.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.DTOs.Enums;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Domain.Models.ValueObjects;
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.Services;

public class ChartServiceTests
{
    public static IEnumerable<object[]> WellFormedData => new List<object[]>
    {
        new object[]
        {
            ChartType.Price,
            true,
            null,
            null,
            DateRangeUnit.Day,
            3,
            null
        },
        new object[]
        {
            ChartType.Profit,
            false,
            DateTime.Parse("2021-01-01"),
            DateTime.Parse("2022-06-01"),
            null,
            null,
            null
        },
        new object[]
        {
            ChartType.Performance,
            false,
            DateTime.Parse("2022-01-03 12:00"),
            DateTime.Parse("2022-01-06 18:35"),
            null,
            null,
            null
        },
        new object[]
        {
            ChartType.AggregatedProfit,
            true,
            null,
            null,
            DateRangeUnit.Year,
            1,
            AggregationFrequency.Week
        },
        new object[]
        {
            ChartType.AggregatedPerformance,
            true,
            null,
            null,
            DateRangeUnit.Week,
            2,
            AggregationFrequency.Hour
        },
        new object[]
        {
            ChartType.Price,
            true,
            null,
            null,
            DateRangeUnit.Month,
            4,
            null
        }
    };

    private readonly Mock<ICurrencyRepository> _currencyRepository;
    private readonly IFixture _fixture;
    private readonly Mock<IChartQueries> _chartQueries;
    private readonly Mock<IChartRepository> _chartRepository;
    private readonly Mock<IInstrumentRepository> _instrumentRepository;
    private readonly Mock<IPortfolioRepository> _portfolioRepository;
    private readonly Mock<IPositionRepository> _positionRepository;

    public ChartServiceTests()
    {
        _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
        _instrumentRepository = _fixture.CreateDefaultInstrumentRepositoryMock();
        _portfolioRepository = _fixture.CreateDefaultPortfolioRepositoryMock();
        _positionRepository = _fixture.CreateDefaultPositionRepositoryMock();
        _currencyRepository = _fixture.CreateDefaultCurrencyRepositoryMock();
        _chartQueries = _fixture.CreateDefaultChartQueriesMock();
        _chartRepository = _fixture.CreateDefaultChartRepositoryMock();
    }

    [Fact]
    public async Task GetAllChartsAsync_ReturnsAllCharts()
    {
        var charts = _fixture.CreateMany<ChartDto>();

        _chartQueries
            .Setup(m => m.GetChartsAsync())
            .ReturnsAsync(charts);

        var sut = _fixture.Create<ChartService>();

        var result = await sut.GetAllChartsAsync();

        Assert.Equal(OperationStatus.Ok, result.Status);
        Assert.Equal(charts, result.Response);
    }

    [Fact]
    public async Task GetChartAsync_ReturnsChart_WhenItExists()
    {
        var chart = _fixture.Create<ChartDto>();

        _chartQueries
            .Setup(m => m.GetChartAsync(chart.Id))
            .ReturnsAsync(chart);

        var sut = _fixture.Create<ChartService>();

        var result = await sut.GetChartAsync(chart.Id);

        Assert.Equal(OperationStatus.Ok, result.Status);
        Assert.Equal(chart, result.Response);
    }

    [Fact]
    public async Task GetChartAsync_ReturnsNotFound_WhenItDoesNotExist()
    {
        _chartQueries
            .Setup(m => m.GetChartAsync(It.IsAny<int>()))
            .ReturnsAsync((ChartDto)null);

        var sut = _fixture.Create<ChartService>();

        var result = await sut.GetChartAsync(_fixture.Create<int>());

        Assert.Equal(OperationStatus.NotFound, result.Status);
    }

    [Theory]
    [MemberData(nameof(WellFormedData))]
    public async Task CreatingChart_AddsChartToRepository_WhenWellFormed(ChartType type, bool isToDate,
        DateTime? startDt, DateTime? endDt,
        DateRangeUnit? toDateRangeUnit, int? toDateRangeValue, AggregationFrequency? frequency)
    {
        var chart = _fixture.Build<ChartDto>()
            .With(c => c.Type, type)
            .With(c => c.IsToDate, isToDate)
            .With(c => c.DateRangeStart, startDt)
            .With(c => c.DateRangeEnd, endDt)
            .With(c => c.Frequency, frequency)
            .With(c => c.ToDateRange,
                toDateRangeUnit == null || toDateRangeValue == null
                    ? null
                    : new ToDateRange((DateRangeUnit)toDateRangeUnit, (int)toDateRangeValue))
            .Create();

        var sut = _fixture.Create<ChartService>();

        await sut.CreateChartAsync(chart);

        _chartRepository.Verify(r => r.Add(It.Is<Chart>(c =>
            ChartDtoMatchesChartEntity(chart, c)
        )));
    }

    [Theory]
    [InlineData(ChartType.Price)]
    [InlineData(ChartType.Profit)]
    [InlineData(ChartType.AggregatedProfit)]
    public async Task CreatingCurrencyChart_ReturnsError_WhenCurrencyDoesNotExist(ChartType type)
    {
        var chart = _fixture.Build<ChartDto>()
            .With(c => c.Type, type)
            .Create();

        _currencyRepository
            .Setup(r => r.ExistsAsync(chart.CurrencyCode))
            .ReturnsAsync(false);
        _currencyRepository
            .Setup(r => r.FindAsync(chart.CurrencyCode))
            .ReturnsAsync((Currency)null);

        var sut = _fixture.Create<ChartService>();

        var response = await sut.CreateChartAsync(chart);
        Assert.Equal(OperationStatus.Error, response.Status);
    }

    [Theory]
    [InlineData(ChartType.AggregatedProfit)]
    [InlineData(ChartType.AggregatedPerformance)]
    public async Task CreatingAggregatedChart_ReturnsError_WhenAggregationFrequencyIsNotProvided(ChartType type)
    {
        var chart = _fixture.Build<ChartDto>()
            .With(c => c.Type, type)
            .With(c => c.Frequency, (AggregationFrequency?)null)
            .Create();

        var sut = _fixture.Create<ChartService>();

        var response = await sut.CreateChartAsync(chart);
        Assert.Equal(OperationStatus.Error, response.Status);
    }

    [Fact]
    public async Task CreatingToDateRangeChart_ReturnsError_WhenToDateRangeIsNotProvided()
    {
        var chart = _fixture.Build<ChartDto>()
            .With(c => c.IsToDate, true)
            .With(c => c.ToDateRange, (ToDateRange)null)
            .Create();

        var sut = _fixture.Create<ChartService>();

        var response = await sut.CreateChartAsync(chart);
        Assert.Equal(OperationStatus.Error, response.Status);
    }

    [Fact]
    public async Task CreatingCustomDateRangeChart_ReturnsError_WhenDateRangeStartIsNotProvided()
    {
        var chart = _fixture.Build<ChartDto>()
            .With(c => c.IsToDate, false)
            .With(c => c.DateRangeStart, (DateTime?)null)
            .Create();

        var sut = _fixture.Create<ChartService>();

        var response = await sut.CreateChartAsync(chart);
        Assert.Equal(OperationStatus.Error, response.Status);
    }

    [Fact]
    public async Task CreatingCustomDateRangeChart_ReturnsError_WhenDateRangeEndIsNotProvided()
    {
        var chart = _fixture.Build<ChartDto>()
            .With(c => c.IsToDate, false)
            .With(c => c.DateRangeEnd, (DateTime?)null)
            .Create();

        var sut = _fixture.Create<ChartService>();

        var response = await sut.CreateChartAsync(chart);
        Assert.Equal(OperationStatus.Error, response.Status);
    }

    [Fact]
    public async Task CreatingChartWithPortfolioLine_ReturnsError_WhenPortfolioDoesNotExist()
    {
        var chart = _fixture.Create<ChartDto>();
        var portfolioLine = _fixture.Build<ChartLineDto>()
            .With(line => line.Type, ChartLineType.Portfolio)
            .Create();
        chart.Lines.Add(portfolioLine);

        _portfolioRepository
            .Setup(r => r.ExistsAsync((int)portfolioLine.PortfolioId))
            .ReturnsAsync(false);
        _portfolioRepository
            .Setup(r => r.FindAsync((int)portfolioLine.PortfolioId))
            .ReturnsAsync((Portfolio)null);

        var sut = _fixture.Create<ChartService>();

        var response = await sut.CreateChartAsync(chart);
        Assert.Equal(OperationStatus.Error, response.Status);
    }

    [Fact]
    public async Task CreatingChartWithPositionLine_ReturnsError_WhenPositionDoesNotExist()
    {
        var chart = _fixture.Create<ChartDto>();
        var positionLine = _fixture.Build<ChartLineDto>()
            .With(line => line.Type, ChartLineType.Position)
            .Create();
        chart.Lines.Add(positionLine);

        _positionRepository
            .Setup(r => r.ExistsAsync((int)positionLine.PositionId))
            .ReturnsAsync(false);
        _positionRepository
            .Setup(r => r.FindAsync((int)positionLine.PositionId))
            .ReturnsAsync((Position)null);

        var sut = _fixture.Create<ChartService>();

        var response = await sut.CreateChartAsync(chart);
        Assert.Equal(OperationStatus.Error, response.Status);
    }

    [Fact]
    public async Task CreatingChartWithInstrumentLine_ReturnsError_WhenInstrumentDoesNotExist()
    {
        var chart = _fixture.Create<ChartDto>();
        var instrumentLine = _fixture.Build<ChartLineDto>()
            .With(line => line.Type, ChartLineType.Instrument)
            .Create();
        chart.Lines.Add(instrumentLine);

        _instrumentRepository
            .Setup(r => r.ExistsAsync((int)instrumentLine.InstrumentId))
            .ReturnsAsync(false);
        _instrumentRepository
            .Setup(r => r.FindAsync((int)instrumentLine.InstrumentId))
            .ReturnsAsync((Instrument)null);

        var sut = _fixture.Create<ChartService>();

        var response = await sut.CreateChartAsync(chart);
        Assert.Equal(OperationStatus.Error, response.Status);
    }

    [Theory]
    [MemberData(nameof(WellFormedData))]
    public async Task UpdatingChart_UpdatesChartInRepository_WhenWellFormed(ChartType type, bool isToDate,
        DateTime? startDt, DateTime? endDt,
        DateRangeUnit? toDateRangeUnit, int? toDateRangeValue, AggregationFrequency? frequency)
    {
        var chart = _fixture.Build<ChartDto>()
            .With(c => c.Type, type)
            .With(c => c.IsToDate, isToDate)
            .With(c => c.DateRangeStart, startDt)
            .With(c => c.DateRangeEnd, endDt)
            .With(c => c.Frequency, frequency)
            .With(c => c.ToDateRange,
                toDateRangeUnit == null || toDateRangeValue == null
                    ? null
                    : new ToDateRange((DateRangeUnit)toDateRangeUnit, (int)toDateRangeValue))
            .Create();

        var sut = _fixture.Create<ChartService>();

        await sut.UpdateChartAsync(chart);

        _chartRepository.Verify(r => r.Update(It.Is<Chart>(c =>
            ChartDtoMatchesChartEntity(chart, c)
        )));
    }

    [Theory]
    [InlineData(ChartType.Price)]
    [InlineData(ChartType.Profit)]
    [InlineData(ChartType.AggregatedProfit)]
    public async Task UpdatingCurrencyChart_ReturnsError_WhenCurrencyDoesNotExist(ChartType type)
    {
        var chart = _fixture.Build<ChartDto>()
            .With(c => c.Type, type)
            .Create();


        _currencyRepository
            .Setup(r => r.ExistsAsync(chart.CurrencyCode))
            .ReturnsAsync(false);
        _currencyRepository
            .Setup(r => r.FindAsync(chart.CurrencyCode))
            .ReturnsAsync((Currency)null);

        var sut = _fixture.Create<ChartService>();

        var response = await sut.UpdateChartAsync(chart);
        Assert.Equal(OperationStatus.Error, response.Status);
    }

    [Theory]
    [InlineData(ChartType.AggregatedProfit)]
    [InlineData(ChartType.AggregatedPerformance)]
    public async Task UpdatingAggregatedChart_ReturnsError_WhenAggregationFrequencyIsNotProvided(ChartType type)
    {
        var chart = _fixture.Build<ChartDto>()
            .With(c => c.Type, type)
            .With(c => c.Frequency, (AggregationFrequency?)null)
            .Create();

        var sut = _fixture.Create<ChartService>();

        var response = await sut.UpdateChartAsync(chart);
        Assert.Equal(OperationStatus.Error, response.Status);
    }

    [Fact]
    public async Task UpdatingToDateRangeChart_ReturnsError_WhenToDateRangeIsNotProvided()
    {
        var chart = _fixture.Build<ChartDto>()
            .With(c => c.IsToDate, true)
            .With(c => c.ToDateRange, (ToDateRange)null)
            .Create();

        var sut = _fixture.Create<ChartService>();

        var response = await sut.UpdateChartAsync(chart);
        Assert.Equal(OperationStatus.Error, response.Status);
    }

    [Fact]
    public async Task UpdatingCustomDateRangeChart_ReturnsError_WhenDateRangeStartIsNotProvided()
    {
        var chart = _fixture.Build<ChartDto>()
            .With(c => c.IsToDate, false)
            .With(c => c.DateRangeStart, (DateTime?)null)
            .Create();

        var sut = _fixture.Create<ChartService>();

        var response = await sut.UpdateChartAsync(chart);
        Assert.Equal(OperationStatus.Error, response.Status);
    }

    [Fact]
    public async Task UpdatingCustomDateRangeChart_ReturnsError_WhenDateRangeEndIsNotProvided()
    {
        var chart = _fixture.Build<ChartDto>()
            .With(c => c.IsToDate, false)
            .With(c => c.DateRangeEnd, (DateTime?)null)
            .Create();

        var sut = _fixture.Create<ChartService>();

        var response = await sut.UpdateChartAsync(chart);
        Assert.Equal(OperationStatus.Error, response.Status);
    }

    [Fact]
    public async Task UpdatingChartWithPortfolioLine_ReturnsError_WhenPortfolioDoesNotExist()
    {
        var chart = _fixture.Create<ChartDto>();
        var portfolioLine = _fixture.Build<ChartLineDto>()
            .With(line => line.Type, ChartLineType.Portfolio)
            .Create();
        chart.Lines.Add(portfolioLine);

        _portfolioRepository
            .Setup(r => r.ExistsAsync((int)portfolioLine.PortfolioId))
            .ReturnsAsync(false);
        _portfolioRepository
            .Setup(r => r.FindAsync((int)portfolioLine.PortfolioId))
            .ReturnsAsync((Portfolio)null);

        var sut = _fixture.Create<ChartService>();

        var response = await sut.UpdateChartAsync(chart);
        Assert.Equal(OperationStatus.Error, response.Status);
    }

    [Fact]
    public async Task UpdatingChartWithPositionLine_ReturnsError_WhenPositionDoesNotExist()
    {
        var chart = _fixture.Create<ChartDto>();
        var positionLine = _fixture.Build<ChartLineDto>()
            .With(line => line.Type, ChartLineType.Position)
            .Create();
        chart.Lines.Add(positionLine);

        _positionRepository
            .Setup(r => r.ExistsAsync((int)positionLine.PositionId))
            .ReturnsAsync(false);
        _positionRepository
            .Setup(r => r.FindAsync((int)positionLine.PositionId))
            .ReturnsAsync((Position)null);

        var sut = _fixture.Create<ChartService>();

        var response = await sut.UpdateChartAsync(chart);
        Assert.Equal(OperationStatus.Error, response.Status);
    }

    [Fact]
    public async Task UpdatingChartWithInstrumentLine_ReturnsError_WhenInstrumentDoesNotExist()
    {
        var chart = _fixture.Create<ChartDto>();
        var instrumentLine = _fixture.Build<ChartLineDto>()
            .With(line => line.Type, ChartLineType.Instrument)
            .Create();
        chart.Lines.Add(instrumentLine);

        _instrumentRepository
            .Setup(r => r.ExistsAsync((int)instrumentLine.InstrumentId))
            .ReturnsAsync(false);
        _instrumentRepository
            .Setup(r => r.FindAsync((int)instrumentLine.InstrumentId))
            .ReturnsAsync((Instrument)null);

        var sut = _fixture.Create<ChartService>();

        var response = await sut.UpdateChartAsync(chart);
        Assert.Equal(OperationStatus.Error, response.Status);
    }

    [Fact]
    public async Task UpdatingChart_ReturnsNotFound_WhenChartDoesNotExist()
    {
        var chart = _fixture.Create<ChartDto>();

        _chartRepository
            .Setup(r => r.ExistsAsync(chart.Id))
            .ReturnsAsync(false);
        _chartRepository
            .Setup(r => r.FindAsync(chart.Id))
            .ReturnsAsync((Chart)null);

        var sut = _fixture.Create<ChartService>();

        var response = await sut.UpdateChartAsync(chart);
        Assert.Equal(OperationStatus.NotFound, response.Status);
    }

    [Fact]
    public async Task DeletingChart_DeletesChart_WhenChartExists()
    {
        var id = _fixture.Create<int>();

        var sut = _fixture.Create<ChartService>();

        await sut.DeleteChartAsync(id);

        _chartRepository.Verify(r => r.DeleteAsync(id), Times.Once());
    }

    [Fact]
    public async Task DeletingChart_ReturnsNotFound_WhenChartDoesNotExist()
    {
        var id = _fixture.Create<int>();
        _chartRepository
            .Setup(r => r.ExistsAsync(id))
            .ReturnsAsync(false);
        _chartRepository
            .Setup(r => r.FindAsync(id))
            .ReturnsAsync((Chart)null);

        var sut = _fixture.Create<ChartService>();

        var response = await sut.DeleteChartAsync(id);
        Assert.Equal(OperationStatus.NotFound, response.Status);
    }


    private bool LineDtosMatchLineEntities(IReadOnlyList<ChartLineDto> dtos, IReadOnlyList<ChartLine> entities)
    {
        if (dtos.Count != entities.Count) return false;

        for (var i = 0; i < dtos.Count; i++)
        {
            var dto = dtos[i];
            var entity = entities[i];
            if (!LineDtoMatchesLineEntity(dto, entity)) return false;
        }

        return true;
    }

    private bool LineDtoMatchesLineEntity(ChartLineDto dto, ChartLine entity)
    {
        var entityAsInstrumentLine = entity as ChartLineInstrument;
        var entityAsPortfolioLine = entity as ChartLinePortfolio;
        var entityAsPositionLine = entity as ChartLinePosition;

        return dto.Color == entity.Color
               && dto.Dash == entity.Dash
               && dto.Width == entity.Width
               && (entityAsInstrumentLine == null || dto.InstrumentId == entityAsInstrumentLine.InstrumentId)
               && (entityAsPortfolioLine == null || dto.PortfolioId == entityAsPortfolioLine.PortfolioId)
               && (entityAsPositionLine == null || dto.PositionId == entityAsPositionLine.PositionId);
    }

    private bool ChartDtoMatchesChartEntity(ChartDto dto, Chart entity)
    {
        return entity.Name == dto.Name &&
               (!IsCurrencyType(dto.Type) || entity.TypeConfiguration.CurrencyCode == dto.CurrencyCode) &&
               (!IsAggregatedType(dto.Type) || entity.TypeConfiguration.Frequency == dto.Frequency) &&
               entity.TypeConfiguration.Type == dto.Type &&
               (entity.DateRange.IsToDate || entity.DateRange.Start == dto.DateRangeStart) &&
               (entity.DateRange.IsToDate || entity.DateRange.End == dto.DateRangeEnd) &&
               entity.DateRange.IsToDate == (dto.IsToDate != null && (bool)dto.IsToDate) &&
               (!entity.DateRange.IsToDate || entity.DateRange.ToDateRange.Unit == dto.ToDateRange.Unit) &&
               (!entity.DateRange.IsToDate || entity.DateRange.ToDateRange.Value == dto.ToDateRange.Value) &&
               LineDtosMatchLineEntities(dto.Lines, entity.Lines.ToList());
    }

    private bool IsAggregatedType(ChartType type)
    {
        return type is ChartType.AggregatedProfit or ChartType.AggregatedPerformance;
    }

    private bool IsCurrencyType(ChartType type)
    {
        return type is ChartType.Price or ChartType.Profit or ChartType.AggregatedProfit;
    }
}