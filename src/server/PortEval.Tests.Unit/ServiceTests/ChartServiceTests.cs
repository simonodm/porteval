using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.DTOs.Enums;
using PortEval.Application.Services;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Domain.Models.ValueObjects;
using PortEval.Tests.Extensions;
using Xunit;

namespace PortEval.Tests.UnitTests.Services
{
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
                DateRangeUnit.DAY,
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
                DateRangeUnit.YEAR,
                1,
                AggregationFrequency.Week
            },
            new object[]
            {
                ChartType.AggregatedPerformance,
                true,
                null,
                null,
                DateRangeUnit.WEEK,
                2,
                AggregationFrequency.Hour
            },
            new object[]
            {
                ChartType.Price,
                true,
                null,
                null,
                DateRangeUnit.MONTH,
                4,
                null
            }
        };

        [Theory]
        [MemberData(nameof(WellFormedData))]
        public async Task CreatingChart_AddsChartToRepository_WhenWellFormed(ChartType type, bool isToDate,
            DateTime? startDt, DateTime? endDt,
            DateRangeUnit? toDateRangeUnit, int? toDateRangeValue, AggregationFrequency? frequency)
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
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

            var chartRepository = fixture.CreateDefaultChartRepositoryMock();
            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultCurrencyRepositoryMock();
            var sut = fixture.Create<ChartService>();

            await sut.CreateChartAsync(chart);

            chartRepository.Verify(r => r.Add(It.Is<Chart>(c =>
                ChartDtoMatchesChartEntity(chart, c)
            )));
        }

        [Theory]
        [InlineData(ChartType.Price)]
        [InlineData(ChartType.Profit)]
        [InlineData(ChartType.AggregatedProfit)]
        public async Task CreatingCurrencyChart_ThrowsException_WhenCurrencyDoesNotExist(ChartType type)
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.Type, type)
                .Create();

            fixture.CreateDefaultChartRepositoryMock();
            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            var currencyRepository = fixture.CreateDefaultCurrencyRepositoryMock();
            currencyRepository
                .Setup(r => r.Exists(chart.CurrencyCode))
                .ReturnsAsync(false);
            currencyRepository
                .Setup(r => r.FindAsync(chart.CurrencyCode))
                .ReturnsAsync((Currency)null);

            var sut = fixture.Create<ChartService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await sut.CreateChartAsync(chart));
        }

        [Theory]
        [InlineData(ChartType.AggregatedProfit)]
        [InlineData(ChartType.AggregatedPerformance)]
        public async Task CreatingAggregatedChart_ThrowsException_WhenAggregationFrequencyIsNotProvided(ChartType type)
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.Type, type)
                .With(c => c.Frequency, (AggregationFrequency?)null)
                .Create();

            fixture.CreateDefaultChartRepositoryMock();
            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultCurrencyRepositoryMock();

            var sut = fixture.Create<ChartService>();

            await Assert.ThrowsAsync<OperationNotAllowedException>(async () => await sut.CreateChartAsync(chart));
        }

        [Fact]
        public async Task CreatingToDateRangeChart_ThrowsException_WhenToDateRangeIsNotProvided()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.IsToDate, true)
                .With(c => c.ToDateRange, (ToDateRange)null)
                .Create();

            fixture.CreateDefaultChartRepositoryMock();
            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultCurrencyRepositoryMock();

            var sut = fixture.Create<ChartService>();

            await Assert.ThrowsAsync<OperationNotAllowedException>(async () => await sut.CreateChartAsync(chart));
        }

        [Fact]
        public async Task CreatingCustomDateRangeChart_ThrowsException_WhenDateRangeStartIsNotProvided()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.IsToDate, false)
                .With(c => c.DateRangeStart, (DateTime?)null)
                .Create();

            fixture.CreateDefaultChartRepositoryMock();
            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultCurrencyRepositoryMock();

            var sut = fixture.Create<ChartService>();

            await Assert.ThrowsAsync<OperationNotAllowedException>(async () => await sut.CreateChartAsync(chart));
        }

        [Fact]
        public async Task CreatingCustomDateRangeChart_ThrowsException_WhenDateRangeEndIsNotProvided()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.IsToDate, false)
                .With(c => c.DateRangeEnd, (DateTime?)null)
                .Create();

            fixture.CreateDefaultChartRepositoryMock();
            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultCurrencyRepositoryMock();

            var sut = fixture.Create<ChartService>();

            await Assert.ThrowsAsync<OperationNotAllowedException>(async () => await sut.CreateChartAsync(chart));
        }

        [Fact]
        public async Task CreatingChartWithPortfolioLine_ThrowsException_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Create<ChartDto>();
            var portfolioLine = fixture.Build<ChartLineDto>()
                .With(line => line.Type, ChartLineType.Portfolio)
                .Create();
            chart.Lines.Add(portfolioLine);

            fixture.CreateDefaultChartRepositoryMock();
            var portfolioRepository = fixture.CreateDefaultPortfolioRepositoryMock();
            portfolioRepository
                .Setup(r => r.Exists((int)portfolioLine.PortfolioId))
                .ReturnsAsync(false);
            portfolioRepository
                .Setup(r => r.FindAsync((int)portfolioLine.PortfolioId))
                .ReturnsAsync((Portfolio)null);
            fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultCurrencyRepositoryMock();

            var sut = fixture.Create<ChartService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await sut.CreateChartAsync(chart));
        }

        [Fact]
        public async Task CreatingChartWithPositionLine_ThrowsException_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Create<ChartDto>();
            var positionLine = fixture.Build<ChartLineDto>()
                .With(line => line.Type, ChartLineType.Position)
                .Create();
            chart.Lines.Add(positionLine);

            fixture.CreateDefaultChartRepositoryMock();
            fixture.CreateDefaultPortfolioRepositoryMock();
            var positionRepository = fixture.CreateDefaultPositionRepositoryMock();
            positionRepository
                .Setup(r => r.Exists((int)positionLine.PositionId))
                .ReturnsAsync(false);
            positionRepository
                .Setup(r => r.FindAsync((int)positionLine.PositionId))
                .ReturnsAsync((Position)null);
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultCurrencyRepositoryMock();

            var sut = fixture.Create<ChartService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await sut.CreateChartAsync(chart));
        }

        [Fact]
        public async Task CreatingChartWithInstrumentLine_ThrowsException_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Create<ChartDto>();
            var instrumentLine = fixture.Build<ChartLineDto>()
                .With(line => line.Type, ChartLineType.Instrument)
                .Create();
            chart.Lines.Add(instrumentLine);

            fixture.CreateDefaultChartRepositoryMock();
            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(r => r.Exists((int)instrumentLine.InstrumentId))
                .ReturnsAsync(false);
            instrumentRepository
                .Setup(r => r.FindAsync((int)instrumentLine.InstrumentId))
                .ReturnsAsync((Instrument)null);

            fixture.CreateDefaultCurrencyRepositoryMock();

            var sut = fixture.Create<ChartService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await sut.CreateChartAsync(chart));
        }

        [Theory]
        [MemberData(nameof(WellFormedData))]
        public async Task UpdatingChart_UpdatesChartInRepository_WhenWellFormed(ChartType type, bool isToDate,
            DateTime? startDt, DateTime? endDt,
            DateRangeUnit? toDateRangeUnit, int? toDateRangeValue, AggregationFrequency? frequency)
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
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

            var chartRepository = fixture.CreateDefaultChartRepositoryMock();
            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultCurrencyRepositoryMock();
            var sut = fixture.Create<ChartService>();

            await sut.UpdateChartAsync(chart);

            chartRepository.Verify(r => r.Update(It.Is<Chart>(c =>
                ChartDtoMatchesChartEntity(chart, c)
            )));
        }

        [Theory]
        [InlineData(ChartType.Price)]
        [InlineData(ChartType.Profit)]
        [InlineData(ChartType.AggregatedProfit)]
        public async Task UpdatingCurrencyChart_ThrowsException_WhenCurrencyDoesNotExist(ChartType type)
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.Type, type)
                .Create();

            fixture.CreateDefaultChartRepositoryMock();
            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            var currencyRepository = fixture.CreateDefaultCurrencyRepositoryMock();
            currencyRepository
                .Setup(r => r.Exists(chart.CurrencyCode))
                .ReturnsAsync(false);
            currencyRepository
                .Setup(r => r.FindAsync(chart.CurrencyCode))
                .ReturnsAsync((Currency)null);

            var sut = fixture.Create<ChartService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await sut.UpdateChartAsync(chart));
        }

        [Theory]
        [InlineData(ChartType.AggregatedProfit)]
        [InlineData(ChartType.AggregatedPerformance)]
        public async Task UpdatingAggregatedChart_ThrowsException_WhenAggregationFrequencyIsNotProvided(ChartType type)
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.Type, type)
                .With(c => c.Frequency, (AggregationFrequency?)null)
                .Create();

            fixture.CreateDefaultChartRepositoryMock();
            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultCurrencyRepositoryMock();

            var sut = fixture.Create<ChartService>();

            await Assert.ThrowsAsync<OperationNotAllowedException>(async () => await sut.UpdateChartAsync(chart));
        }

        [Fact]
        public async Task UpdatingToDateRangeChart_ThrowsException_WhenToDateRangeIsNotProvided()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.IsToDate, true)
                .With(c => c.ToDateRange, (ToDateRange)null)
                .Create();

            fixture.CreateDefaultChartRepositoryMock();
            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultCurrencyRepositoryMock();

            var sut = fixture.Create<ChartService>();

            await Assert.ThrowsAsync<OperationNotAllowedException>(async () => await sut.UpdateChartAsync(chart));
        }

        [Fact]
        public async Task UpdatingCustomDateRangeChart_ThrowsException_WhenDateRangeStartIsNotProvided()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.IsToDate, false)
                .With(c => c.DateRangeStart, (DateTime?)null)
                .Create();

            fixture.CreateDefaultChartRepositoryMock();
            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultCurrencyRepositoryMock();

            var sut = fixture.Create<ChartService>();

            await Assert.ThrowsAsync<OperationNotAllowedException>(async () => await sut.UpdateChartAsync(chart));
        }

        [Fact]
        public async Task UpdatingCustomDateRangeChart_ThrowsException_WhenDateRangeEndIsNotProvided()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.IsToDate, false)
                .With(c => c.DateRangeEnd, (DateTime?)null)
                .Create();

            fixture.CreateDefaultChartRepositoryMock();
            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultCurrencyRepositoryMock();

            var sut = fixture.Create<ChartService>();

            await Assert.ThrowsAsync<OperationNotAllowedException>(async () => await sut.UpdateChartAsync(chart));
        }

        [Fact]
        public async Task UpdatingChartWithPortfolioLine_ThrowsException_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Create<ChartDto>();
            var portfolioLine = fixture.Build<ChartLineDto>()
                .With(line => line.Type, ChartLineType.Portfolio)
                .Create();
            chart.Lines.Add(portfolioLine);

            fixture.CreateDefaultChartRepositoryMock();
            var portfolioRepository = fixture.CreateDefaultPortfolioRepositoryMock();
            portfolioRepository
                .Setup(r => r.Exists((int)portfolioLine.PortfolioId))
                .ReturnsAsync(false);
            portfolioRepository
                .Setup(r => r.FindAsync((int)portfolioLine.PortfolioId))
                .ReturnsAsync((Portfolio)null);
            fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultCurrencyRepositoryMock();

            var sut = fixture.Create<ChartService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await sut.UpdateChartAsync(chart));
        }

        [Fact]
        public async Task UpdatingChartWithPositionLine_ThrowsException_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Create<ChartDto>();
            var positionLine = fixture.Build<ChartLineDto>()
                .With(line => line.Type, ChartLineType.Position)
                .Create();
            chart.Lines.Add(positionLine);

            fixture.CreateDefaultChartRepositoryMock();
            fixture.CreateDefaultPortfolioRepositoryMock();
            var positionRepository = fixture.CreateDefaultPositionRepositoryMock();
            positionRepository
                .Setup(r => r.Exists((int)positionLine.PositionId))
                .ReturnsAsync(false);
            positionRepository
                .Setup(r => r.FindAsync((int)positionLine.PositionId))
                .ReturnsAsync((Position)null);
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultCurrencyRepositoryMock();

            var sut = fixture.Create<ChartService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await sut.UpdateChartAsync(chart));
        }

        [Fact]
        public async Task UpdatingChartWithInstrumentLine_ThrowsException_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Create<ChartDto>();
            var instrumentLine = fixture.Build<ChartLineDto>()
                .With(line => line.Type, ChartLineType.Instrument)
                .Create();
            chart.Lines.Add(instrumentLine);

            fixture.CreateDefaultChartRepositoryMock();
            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(r => r.Exists((int)instrumentLine.InstrumentId))
                .ReturnsAsync(false);
            instrumentRepository
                .Setup(r => r.FindAsync((int)instrumentLine.InstrumentId))
                .ReturnsAsync((Instrument)null);

            fixture.CreateDefaultCurrencyRepositoryMock();

            var sut = fixture.Create<ChartService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await sut.UpdateChartAsync(chart));
        }

        [Fact]
        public async Task UpdatingChart_ThrowsException_WhenChartDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Create<ChartDto>();
            var chartRepository = fixture.CreateDefaultChartRepositoryMock();
            chartRepository
                .Setup(r => r.Exists(chart.Id))
                .ReturnsAsync(false);
            chartRepository
                .Setup(r => r.FindAsync(chart.Id))
                .ReturnsAsync((Chart)null);
            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultCurrencyRepositoryMock();

            var sut = fixture.Create<ChartService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await sut.UpdateChartAsync(chart));
        }

        [Fact]
        public async Task DeletingChart_DeletesChart_WhenChartExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var id = fixture.Create<int>();
            var chartRepository = fixture.CreateDefaultChartRepositoryMock();
            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultCurrencyRepositoryMock();

            var sut = fixture.Create<ChartService>();

            await sut.DeleteChartAsync(id);

            chartRepository.Verify(r => r.Delete(id), Times.Once());
        }

        [Fact]
        public async Task DeletingChart_ThrowsException_WhenChartDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var id = fixture.Create<int>();
            var chartRepository = fixture.CreateDefaultChartRepositoryMock();
            chartRepository
                .Setup(r => r.Exists(id))
                .ReturnsAsync(false);
            chartRepository
                .Setup(r => r.FindAsync(id))
                .ReturnsAsync((Chart)null);
            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultCurrencyRepositoryMock();

            var sut = fixture.Create<ChartService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await sut.DeleteChartAsync(id));
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
            return type == ChartType.AggregatedProfit || type == ChartType.AggregatedPerformance;
        }

        private bool IsCurrencyType(ChartType type)
        {
            return type == ChartType.Price || type == ChartType.Profit || type == ChartType.AggregatedProfit;
        }
    }
}