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
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Domain.Models.ValueObjects;
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.Services
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

        [Fact]
        public async Task GetAllChartsAsync_ReturnsAllCharts()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var charts = fixture.CreateMany<ChartDto>();

            var chartQueries = fixture.CreateDefaultChartQueriesMock();
            chartQueries
                .Setup(m => m.GetChartsAsync())
                .ReturnsAsync(charts);

            var sut = fixture.Create<ChartService>();

            var result = await sut.GetAllChartsAsync();

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(charts, result.Response);
        }

        [Fact]
        public async Task GetChartAsync_ReturnsChart_WhenItExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Create<ChartDto>();

            var chartQueries = fixture.CreateDefaultChartQueriesMock();
            chartQueries
                .Setup(m => m.GetChartAsync(chart.Id))
                .ReturnsAsync(chart);

            var sut = fixture.Create<ChartService>();

            var result = await sut.GetChartAsync(chart.Id);

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(chart, result.Response);
        }

        [Fact]
        public async Task GetChartAsync_ReturnsNotFound_WhenItDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chartQueries = fixture.CreateDefaultChartQueriesMock();
            chartQueries
                .Setup(m => m.GetChartAsync(It.IsAny<int>()))
                .ReturnsAsync((ChartDto)null);

            var sut = fixture.Create<ChartService>();

            var result = await sut.GetChartAsync(fixture.Create<int>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

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
            chartRepository
                .Setup(m => m.Add(It.IsAny<Chart>()))
                .Returns<Chart>(c => c);
            CreateEntityRepositoryMocks(fixture);
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
        public async Task CreatingCurrencyChart_ReturnsError_WhenCurrencyDoesNotExist(ChartType type)
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.Type, type)
                .Create();

            fixture.CreateDefaultChartRepositoryMock();
            CreateEntityRepositoryMocks(fixture);
            var currencyRepository = fixture.CreateDefaultCurrencyRepositoryMock();
            currencyRepository
                .Setup(r => r.ExistsAsync(chart.CurrencyCode))
                .ReturnsAsync(false);
            currencyRepository
                .Setup(r => r.FindAsync(chart.CurrencyCode))
                .ReturnsAsync((Currency)null);

            var sut = fixture.Create<ChartService>();

            var response = await sut.CreateChartAsync(chart);
        }

        [Theory]
        [InlineData(ChartType.AggregatedProfit)]
        [InlineData(ChartType.AggregatedPerformance)]
        public async Task CreatingAggregatedChart_ReturnsError_WhenAggregationFrequencyIsNotProvided(ChartType type)
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.Type, type)
                .With(c => c.Frequency, (AggregationFrequency?)null)
                .Create();

            fixture.CreateDefaultChartRepositoryMock();
            CreateEntityRepositoryMocks(fixture);

            var sut = fixture.Create<ChartService>();

            var response = await sut.CreateChartAsync(chart);
            Assert.Equal(OperationStatus.Error, response.Status);
        }

        [Fact]
        public async Task CreatingToDateRangeChart_ReturnsError_WhenToDateRangeIsNotProvided()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.IsToDate, true)
                .With(c => c.ToDateRange, (ToDateRange)null)
                .Create();

            fixture.CreateDefaultChartRepositoryMock();
            CreateEntityRepositoryMocks(fixture);

            var sut = fixture.Create<ChartService>();

            var response = await sut.CreateChartAsync(chart);
            Assert.Equal(OperationStatus.Error, response.Status);
        }

        [Fact]
        public async Task CreatingCustomDateRangeChart_ReturnsError_WhenDateRangeStartIsNotProvided()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.IsToDate, false)
                .With(c => c.DateRangeStart, (DateTime?)null)
                .Create();

            fixture.CreateDefaultChartRepositoryMock();
            CreateEntityRepositoryMocks(fixture);

            var sut = fixture.Create<ChartService>();

            var response = await sut.CreateChartAsync(chart);
            Assert.Equal(OperationStatus.Error, response.Status);
        }

        [Fact]
        public async Task CreatingCustomDateRangeChart_ReturnsError_WhenDateRangeEndIsNotProvided()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.IsToDate, false)
                .With(c => c.DateRangeEnd, (DateTime?)null)
                .Create();

            fixture.CreateDefaultChartRepositoryMock();
            CreateEntityRepositoryMocks(fixture);

            var sut = fixture.Create<ChartService>();

            var response = await sut.CreateChartAsync(chart);
            Assert.Equal(OperationStatus.Error, response.Status);
        }

        [Fact]
        public async Task CreatingChartWithPortfolioLine_ReturnsError_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Create<ChartDto>();
            var portfolioLine = fixture.Build<ChartLineDto>()
                .With(line => line.Type, ChartLineType.Portfolio)
                .Create();
            chart.Lines.Add(portfolioLine);

            fixture.CreateDefaultChartRepositoryMock();
            CreateEntityRepositoryMocks(fixture);
            var portfolioRepository = fixture.CreateDefaultPortfolioRepositoryMock();
            portfolioRepository
                .Setup(r => r.ExistsAsync((int)portfolioLine.PortfolioId))
                .ReturnsAsync(false);
            portfolioRepository
                .Setup(r => r.FindAsync((int)portfolioLine.PortfolioId))
                .ReturnsAsync((Portfolio)null);

            var sut = fixture.Create<ChartService>();

            var response = await sut.CreateChartAsync(chart);
            Assert.Equal(OperationStatus.Error, response.Status);
        }

        [Fact]
        public async Task CreatingChartWithPositionLine_ReturnsError_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Create<ChartDto>();
            var positionLine = fixture.Build<ChartLineDto>()
                .With(line => line.Type, ChartLineType.Position)
                .Create();
            chart.Lines.Add(positionLine);

            fixture.CreateDefaultChartRepositoryMock();
            CreateEntityRepositoryMocks(fixture);
            var positionRepository = fixture.CreateDefaultPositionRepositoryMock();
            positionRepository
                .Setup(r => r.ExistsAsync((int)positionLine.PositionId))
                .ReturnsAsync(false);
            positionRepository
                .Setup(r => r.FindAsync((int)positionLine.PositionId))
                .ReturnsAsync((Position)null);

            var sut = fixture.Create<ChartService>();

            var response = await sut.CreateChartAsync(chart);
            Assert.Equal(OperationStatus.Error, response.Status);
        }

        [Fact]
        public async Task CreatingChartWithInstrumentLine_ReturnsError_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Create<ChartDto>();
            var instrumentLine = fixture.Build<ChartLineDto>()
                .With(line => line.Type, ChartLineType.Instrument)
                .Create();
            chart.Lines.Add(instrumentLine);

            fixture.CreateDefaultChartRepositoryMock();
            CreateEntityRepositoryMocks(fixture);
            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(r => r.ExistsAsync((int)instrumentLine.InstrumentId))
                .ReturnsAsync(false);
            instrumentRepository
                .Setup(r => r.FindAsync((int)instrumentLine.InstrumentId))
                .ReturnsAsync((Instrument)null);

            var sut = fixture.Create<ChartService>();

            var response = await sut.CreateChartAsync(chart);
            Assert.Equal(OperationStatus.Error, response.Status);
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
            chartRepository
                .Setup(r => r.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new Chart(id, fixture.Create<string>()));
            chartRepository
                .Setup(r => r.Update(It.IsAny<Chart>()))
                .Returns<Chart>(c => c);
            CreateEntityRepositoryMocks(fixture);

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
        public async Task UpdatingCurrencyChart_ReturnsError_WhenCurrencyDoesNotExist(ChartType type)
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.Type, type)
                .Create();

            var chartRepository = fixture.CreateDefaultChartRepositoryMock();
            chartRepository
                .Setup(r => r.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new Chart(id, fixture.Create<string>()));
            CreateEntityRepositoryMocks(fixture);
            var currencyRepository = fixture.CreateDefaultCurrencyRepositoryMock();
            currencyRepository
                .Setup(r => r.ExistsAsync(chart.CurrencyCode))
                .ReturnsAsync(false);
            currencyRepository
                .Setup(r => r.FindAsync(chart.CurrencyCode))
                .ReturnsAsync((Currency)null);

            var sut = fixture.Create<ChartService>();

            var response = await sut.UpdateChartAsync(chart);
            Assert.Equal(OperationStatus.Error, response.Status);
        }

        [Theory]
        [InlineData(ChartType.AggregatedProfit)]
        [InlineData(ChartType.AggregatedPerformance)]
        public async Task UpdatingAggregatedChart_ReturnsError_WhenAggregationFrequencyIsNotProvided(ChartType type)
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.Type, type)
                .With(c => c.Frequency, (AggregationFrequency?)null)
                .Create();

            var chartRepository = fixture.CreateDefaultChartRepositoryMock();
            chartRepository
                .Setup(r => r.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new Chart(id, fixture.Create<string>()));
            CreateEntityRepositoryMocks(fixture);

            var sut = fixture.Create<ChartService>();

            var response = await sut.UpdateChartAsync(chart);
            Assert.Equal(OperationStatus.Error, response.Status);
        }

        [Fact]
        public async Task UpdatingToDateRangeChart_ReturnsError_WhenToDateRangeIsNotProvided()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.IsToDate, true)
                .With(c => c.ToDateRange, (ToDateRange)null)
                .Create();

            var chartRepository = fixture.CreateDefaultChartRepositoryMock();
            chartRepository
                .Setup(r => r.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new Chart(id, fixture.Create<string>()));
            CreateEntityRepositoryMocks(fixture);

            var sut = fixture.Create<ChartService>();

            var response = await sut.UpdateChartAsync(chart);
            Assert.Equal(OperationStatus.Error, response.Status);
        }

        [Fact]
        public async Task UpdatingCustomDateRangeChart_ReturnsError_WhenDateRangeStartIsNotProvided()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.IsToDate, false)
                .With(c => c.DateRangeStart, (DateTime?)null)
                .Create();

            var chartRepository = fixture.CreateDefaultChartRepositoryMock();
            chartRepository
                .Setup(r => r.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new Chart(id, fixture.Create<string>()));
            CreateEntityRepositoryMocks(fixture);

            var sut = fixture.Create<ChartService>();

            var response = await sut.UpdateChartAsync(chart);
            Assert.Equal(OperationStatus.Error, response.Status);
        }

        [Fact]
        public async Task UpdatingCustomDateRangeChart_ReturnsError_WhenDateRangeEndIsNotProvided()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Build<ChartDto>()
                .With(c => c.IsToDate, false)
                .With(c => c.DateRangeEnd, (DateTime?)null)
                .Create();

            var chartRepository = fixture.CreateDefaultChartRepositoryMock();
            chartRepository
                .Setup(r => r.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new Chart(id, fixture.Create<string>()));
            CreateEntityRepositoryMocks(fixture);

            var sut = fixture.Create<ChartService>();

            var response = await sut.UpdateChartAsync(chart);
            Assert.Equal(OperationStatus.Error, response.Status);
        }

        [Fact]
        public async Task UpdatingChartWithPortfolioLine_ReturnsError_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Create<ChartDto>();
            var portfolioLine = fixture.Build<ChartLineDto>()
                .With(line => line.Type, ChartLineType.Portfolio)
                .Create();
            chart.Lines.Add(portfolioLine);

            var chartRepository = fixture.CreateDefaultChartRepositoryMock();
            chartRepository
                .Setup(r => r.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new Chart(id, fixture.Create<string>()));
            CreateEntityRepositoryMocks(fixture);
            var portfolioRepository = fixture.CreateDefaultPortfolioRepositoryMock();
            portfolioRepository
                .Setup(r => r.ExistsAsync((int)portfolioLine.PortfolioId))
                .ReturnsAsync(false);
            portfolioRepository
                .Setup(r => r.FindAsync((int)portfolioLine.PortfolioId))
                .ReturnsAsync((Portfolio)null);

            var sut = fixture.Create<ChartService>();

            var response = await sut.UpdateChartAsync(chart);
            Assert.Equal(OperationStatus.Error, response.Status);
        }

        [Fact]
        public async Task UpdatingChartWithPositionLine_ReturnsError_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Create<ChartDto>();
            var positionLine = fixture.Build<ChartLineDto>()
                .With(line => line.Type, ChartLineType.Position)
                .Create();
            chart.Lines.Add(positionLine);

            var chartRepository = fixture.CreateDefaultChartRepositoryMock();
            chartRepository
                .Setup(r => r.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new Chart(id, fixture.Create<string>()));
            CreateEntityRepositoryMocks(fixture);
            var positionRepository = fixture.CreateDefaultPositionRepositoryMock();
            positionRepository
                .Setup(r => r.ExistsAsync((int)positionLine.PositionId))
                .ReturnsAsync(false);
            positionRepository
                .Setup(r => r.FindAsync((int)positionLine.PositionId))
                .ReturnsAsync((Position)null);

            var sut = fixture.Create<ChartService>();

            var response = await sut.UpdateChartAsync(chart);
            Assert.Equal(OperationStatus.Error, response.Status);
        }

        [Fact]
        public async Task UpdatingChartWithInstrumentLine_ReturnsError_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Create<ChartDto>();
            var instrumentLine = fixture.Build<ChartLineDto>()
                .With(line => line.Type, ChartLineType.Instrument)
                .Create();
            chart.Lines.Add(instrumentLine);

            var chartRepository = fixture.CreateDefaultChartRepositoryMock();
            chartRepository
                .Setup(r => r.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new Chart(id, fixture.Create<string>()));
            CreateEntityRepositoryMocks(fixture);
            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(r => r.ExistsAsync((int)instrumentLine.InstrumentId))
                .ReturnsAsync(false);
            instrumentRepository
                .Setup(r => r.FindAsync((int)instrumentLine.InstrumentId))
                .ReturnsAsync((Instrument)null);

            var sut = fixture.Create<ChartService>();

            var response = await sut.UpdateChartAsync(chart);
            Assert.Equal(OperationStatus.Error, response.Status);
        }

        [Fact]
        public async Task UpdatingChart_ReturnsNotFound_WhenChartDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Create<ChartDto>();
            var chartRepository = fixture.CreateDefaultChartRepositoryMock();
            chartRepository
                .Setup(r => r.ExistsAsync(chart.Id))
                .ReturnsAsync(false);
            chartRepository
                .Setup(r => r.FindAsync(chart.Id))
                .ReturnsAsync((Chart)null);
            CreateEntityRepositoryMocks(fixture);

            var sut = fixture.Create<ChartService>();

            var response = await sut.UpdateChartAsync(chart);
            Assert.Equal(OperationStatus.NotFound, response.Status);
        }

        [Fact]
        public async Task DeletingChart_DeletesChart_WhenChartExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var id = fixture.Create<int>();
            var chartRepository = fixture.CreateDefaultChartRepositoryMock();
            chartRepository
                .Setup(m => m.ExistsAsync(It.IsAny<int>()))
                .ReturnsAsync(true);
            chartRepository
                .Setup(r => r.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new Chart(id, fixture.Create<string>()));
            CreateEntityRepositoryMocks(fixture);

            var sut = fixture.Create<ChartService>();

            await sut.DeleteChartAsync(id);

            chartRepository.Verify(r => r.DeleteAsync(id), Times.Once());
        }

        [Fact]
        public async Task DeletingChart_ReturnsNotFound_WhenChartDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var id = fixture.Create<int>();
            var chartRepository = fixture.CreateDefaultChartRepositoryMock();
            chartRepository
                .Setup(r => r.ExistsAsync(id))
                .ReturnsAsync(false);
            chartRepository
                .Setup(r => r.FindAsync(id))
                .ReturnsAsync((Chart)null);
            CreateEntityRepositoryMocks(fixture);

            var sut = fixture.Create<ChartService>();

            var response = await sut.DeleteChartAsync(id);
            Assert.Equal(OperationStatus.NotFound, response.Status);
        }

        private void CreateEntityRepositoryMocks(IFixture fixture)
        {
            var portfolioRepository = fixture.CreateDefaultPortfolioRepositoryMock();
            portfolioRepository
                .Setup(m => m.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new Portfolio(id, fixture.Create<string>(), fixture.Create<string>(),
                    fixture.Create<string>()));

            var positionRepository = fixture.CreateDefaultPositionRepositoryMock();
            positionRepository
                .Setup(m => m.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new Position(id, fixture.Create<int>(), fixture.Create<int>(), fixture.Create<string>()));

            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(m => m.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new Instrument(
                    id,
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<InstrumentType>(),
                    fixture.Create<string>(),
                    fixture.Create<string>())
                );

            var currencyRepository = fixture.CreateDefaultCurrencyRepositoryMock();
            currencyRepository
                .Setup(m => m.FindAsync(It.IsAny<string>()))
                .ReturnsAsync((string code) => new Currency(code, fixture.Create<string>(), fixture.Create<string>()));
            currencyRepository
                .Setup(m => m.ExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
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
}