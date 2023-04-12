using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Core;
using PortEval.Application.Core.Common;
using PortEval.Application.Core.Interfaces.Calculators;
using PortEval.Application.Core.Interfaces.ChartDataGenerators;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Core.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.Services
{
    public class PositionServiceTests
    {
        [Fact]
        public async Task GetAllPositionsAsync_ReturnsAllPositions()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positions = fixture.CreateMany<PositionDto>();

            var positionQueries = fixture.CreateDefaultPositionQueriesMock();
            positionQueries
                .Setup(m => m.GetAllPositionsAsync())
                .ReturnsAsync(positions);

            var sut = fixture.Create<PositionService>();
            var result = await sut.GetAllPositionsAsync();

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(positions, result.Response);
        }

        [Fact]
        public async Task GetPortfolioPositionsAsync_ReturnsPortfolioPositions_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();
            var positions = fixture.CreateMany<PositionDto>();

            var positionQueries = fixture.CreateDefaultPositionQueriesMock();
            positionQueries
                .Setup(m => m.GetPortfolioPositionsAsync(portfolioId))
                .ReturnsAsync(positions);

            var sut = fixture.Create<PositionService>();
            var result = await sut.GetPortfolioPositionsAsync(portfolioId);

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(positions, result.Response);
        }

        [Fact]
        public async Task GetPortfolioPositionsAsync_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioId = fixture.Create<int>();

            var positionQueries = fixture.CreateDefaultPositionQueriesMock();
            positionQueries
                .Setup(m => m.GetPortfolioPositionsAsync(portfolioId))
                .ReturnsAsync((PositionDto[]) null);

            var sut = fixture.Create<PositionService>();
            var result = await sut.GetPortfolioPositionsAsync(portfolioId);

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetPositionAsync_ReturnsPosition_WhenItExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionId = fixture.Create<int>();
            var position = fixture.Create<PositionDto>();

            var positionQueries = fixture.CreateDefaultPositionQueriesMock();
            positionQueries
                .Setup(m => m.GetPositionAsync(positionId))
                .ReturnsAsync(position);

            var sut = fixture.Create<PositionService>();
            var result = await sut.GetPositionAsync(positionId);

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(position, result.Response);
        }

        [Fact]
        public async Task GetPositionAsync_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionId = fixture.Create<int>();

            var positionQueries = fixture.CreateDefaultPositionQueriesMock();
            positionQueries
                .Setup(m => m.GetPositionAsync(positionId))
                .ReturnsAsync((PositionDto) null);

            var sut = fixture.Create<PositionService>();
            var result = await sut.GetPositionAsync(positionId);

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetPositionValueAsync_ReturnsCorrectPositionValue_WhenPositionExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var value = fixture.Create<decimal>();
            var time = fixture.Create<DateTime>();

            var valueCalculator = fixture.Freeze<Mock<IPositionValueCalculator>>();
            valueCalculator
                .Setup(c => c.CalculateValue(It.IsAny<IEnumerable<PositionPriceRangeData>>(), time))
                .Returns(value);

            var sut = fixture.Create<PositionService>();
            var result = await sut.GetPositionValueAsync(fixture.Create<int>(), time);

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(value, result.Response.Value);
            Assert.Equal(time, result.Response.Time);
        }

        [Fact]
        public async Task GetPositionValueAsync_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionQueries = fixture.CreateDefaultPositionQueriesMock();
            positionQueries
                .Setup(m => m.GetPositionAsync(It.IsAny<int>()))
                .ReturnsAsync((PositionDto) null);

            var sut = fixture.Create<PositionService>();
            var result = await sut.GetPositionValueAsync(fixture.Create<int>(), fixture.Create<DateTime>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetPositionProfitAsync_ReturnsCorrectProfit_WhenPositionExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var profit = fixture.Create<decimal>();
            var dateRange = fixture.Create<DateRangeParams>();

            var profitCalculator = fixture.Freeze<Mock<IPositionProfitCalculator>>();
            profitCalculator
                .Setup(c => c.CalculateProfit(It.IsAny<IEnumerable<PositionPriceRangeData>>(), dateRange.From, dateRange.To))
                .Returns(profit);

            var sut = fixture.Create<PositionService>();
            var result = await sut.GetPositionProfitAsync(fixture.Create<int>(), dateRange);

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(profit, result.Response.Profit);
            Assert.Equal(dateRange.From, result.Response.From);
            Assert.Equal(dateRange.To, result.Response.To);
        }

        [Fact]
        public async Task GetPositionProfitAsync_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionQueries = fixture.CreateDefaultPositionQueriesMock();
            positionQueries
                .Setup(m => m.GetPositionAsync(It.IsAny<int>()))
                .ReturnsAsync((PositionDto) null);

            var sut = fixture.Create<PositionService>();
            var result = await sut.GetPositionProfitAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetPositionPerformanceAsync_ReturnsCorrectPerformance_WhenPositionExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var performance = fixture.Create<decimal>();
            var dateRange = fixture.Create<DateRangeParams>();

            var performanceCalculator = fixture.Freeze<Mock<IPositionPerformanceCalculator>>();
            performanceCalculator
                .Setup(c => c.CalculatePerformance(It.IsAny<IEnumerable<PositionPriceRangeData>>(), dateRange.From, dateRange.To))
                .Returns(performance);

            var sut = fixture.Create<PositionService>();
            var result = await sut.GetPositionProfitAsync(fixture.Create<int>(), dateRange);

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(performance, result.Response.Profit);
            Assert.Equal(dateRange.From, result.Response.From);
            Assert.Equal(dateRange.To, result.Response.To);
        }

        [Fact]
        public async Task GetPositionPerformanceAsync_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionQueries = fixture.CreateDefaultPositionQueriesMock();
            positionQueries
                .Setup(m => m.GetPositionAsync(It.IsAny<int>()))
                .ReturnsAsync((PositionDto) null);

            var sut = fixture.Create<PositionService>();
            var result = await sut.GetPositionProfitAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetPositionBreakEvenPointAsync_ReturnsCorrectBreakEvenPoint_WhenPositionExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var breakEvenPoint = fixture.Create<decimal>();
            var time = fixture.Create<DateTime>();

            var breakEvenPointCalculator = fixture.Freeze<Mock<IPositionBreakEvenPointCalculator>>();
            breakEvenPointCalculator
                .Setup(c => c.CalculatePositionBreakEvenPoint(It.IsAny<IEnumerable<TransactionDto>>()))
                .Returns(breakEvenPoint);

            var sut = fixture.Create<PositionService>();
            var result = await sut.GetPositionBreakEvenPointAsync(fixture.Create<int>(), time);

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(breakEvenPoint, result.Response.BreakEvenPoint);
            Assert.Equal(time, result.Response.Time);
        }

        [Fact]
        public async Task GetPositionBreakEvenPointAsync_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionQueries = fixture.CreateDefaultPositionQueriesMock();
            positionQueries
                .Setup(m => m.GetPositionAsync(It.IsAny<int>()))
                .ReturnsAsync((PositionDto) null);

            var sut = fixture.Create<PositionService>();
            var result = await sut.GetPositionBreakEvenPointAsync(fixture.Create<int>(), fixture.Create<DateTime>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task ChartPositionValueAsync_ReturnsCorrectValueChart_WhenPositionExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var valueChart = fixture.CreateMany<EntityChartPointDto>();
            var dateRange = fixture.Create<DateRangeParams>();

            var chartDataGenerator = fixture.Freeze<Mock<IPositionChartDataGenerator>>();
            chartDataGenerator
                .Setup(c => c.ChartValue(It.IsAny<PositionPriceListData>(), dateRange, It.IsAny<AggregationFrequency>()))
                .Returns(valueChart);

            var sut = fixture.Create<PositionService>();
            var result = await sut.ChartPositionValueAsync(fixture.Create<int>(), dateRange, fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(valueChart, result.Response);
        }

        [Fact]
        public async Task ChartPositionValueAsync_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionQueries = fixture.CreateDefaultPositionQueriesMock();
            positionQueries
                .Setup(m => m.GetPositionAsync(It.IsAny<int>()))
                .ReturnsAsync((PositionDto) null);

            var sut = fixture.Create<PositionService>();
            var result = await sut.ChartPositionValueAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>(), fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task ChartPositionProfitAsync_ReturnsCorrectProfitChart_WhenPositionExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var profitChart = fixture.CreateMany<EntityChartPointDto>();
            var dateRange = fixture.Create<DateRangeParams>();

            var chartDataGenerator = fixture.Freeze<Mock<IPositionChartDataGenerator>>();
            chartDataGenerator
                .Setup(c => c.ChartProfit(It.IsAny<PositionPriceListData>(), dateRange, It.IsAny<AggregationFrequency>()))
                .Returns(profitChart);

            var sut = fixture.Create<PositionService>();
            var result = await sut.ChartPositionProfitAsync(fixture.Create<int>(), dateRange, fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(profitChart, result.Response);
        }

        [Fact]
        public async Task ChartPositionProfitAsync_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionQueries = fixture.CreateDefaultPositionQueriesMock();
            positionQueries
                .Setup(m => m.GetPositionAsync(It.IsAny<int>()))
                .ReturnsAsync((PositionDto) null);

            var sut = fixture.Create<PositionService>();
            var result = await sut.ChartPositionProfitAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>(), fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task ChartPositionPerformanceAsync_ReturnsCorrectPerformanceChart_WhenPositionExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var performanceChart = fixture.CreateMany<EntityChartPointDto>();
            var dateRange = fixture.Create<DateRangeParams>();

            var chartDataGenerator = fixture.Freeze<Mock<IPositionChartDataGenerator>>();
            chartDataGenerator
                .Setup(c => c.ChartPerformance(It.IsAny<PositionPriceListData>(), dateRange, It.IsAny<AggregationFrequency>()))
                .Returns(performanceChart);

            var sut = fixture.Create<PositionService>();
            var result = await sut.ChartPositionPerformanceAsync(fixture.Create<int>(), dateRange, fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(performanceChart, result.Response);
        }

        [Fact]
        public async Task ChartPositionPerformanceAsync_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionQueries = fixture.CreateDefaultPositionQueriesMock();
            positionQueries
                .Setup(m => m.GetPositionAsync(It.IsAny<int>()))
                .ReturnsAsync((PositionDto) null);

            var sut = fixture.Create<PositionService>();
            var result = await sut.ChartPositionPerformanceAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>(), fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task ChartPositionAggregatedProfitAsync_ReturnsCorrectAggregatedProfit_WhenPositionExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var aggregatedProfitChart = fixture.CreateMany<EntityChartPointDto>();
            var dateRange = fixture.Create<DateRangeParams>();

            var chartDataGenerator = fixture.Freeze<Mock<IPositionChartDataGenerator>>();
            chartDataGenerator
                .Setup(c => c.ChartAggregatedProfit(It.IsAny<PositionPriceListData>(), dateRange, It.IsAny<AggregationFrequency>()))
                .Returns(aggregatedProfitChart);

            var sut = fixture.Create<PositionService>();
            var result = await sut.ChartPositionAggregatedProfitAsync(fixture.Create<int>(), dateRange, fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(aggregatedProfitChart, result.Response);
        }

        [Fact]
        public async Task ChartPositionAggregatedProfitAsync_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionQueries = fixture.CreateDefaultPositionQueriesMock();
            positionQueries
                .Setup(m => m.GetPositionAsync(It.IsAny<int>()))
                .ReturnsAsync((PositionDto) null);

            var sut = fixture.Create<PositionService>();
            var result = await sut.ChartPositionAggregatedProfitAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>(), fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task ChartPositionAggregatedPerformanceAsync_ReturnsCorrectAggregatedPerformance_WhenPositionExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var aggregatedPerformanceChart = fixture.CreateMany<EntityChartPointDto>();
            var dateRange = fixture.Create<DateRangeParams>();

            var chartDataGenerator = fixture.Freeze<Mock<IPositionChartDataGenerator>>();
            chartDataGenerator
                .Setup(c => c.ChartAggregatedPerformance(It.IsAny<PositionPriceListData>(), dateRange, It.IsAny<AggregationFrequency>()))
                .Returns(aggregatedPerformanceChart);

            var sut = fixture.Create<PositionService>();
            var result = await sut.ChartPositionAggregatedPerformanceAsync(fixture.Create<int>(), dateRange, fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(aggregatedPerformanceChart, result.Response);
        }

        [Fact]
        public async Task ChartPositionAggregatedPerformanceAsync_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionQueries = fixture.CreateDefaultPositionQueriesMock();
            positionQueries
                .Setup(m => m.GetPositionAsync(It.IsAny<int>()))
                .ReturnsAsync((PositionDto) null);

            var sut = fixture.Create<PositionService>();
            var result = await sut.ChartPositionAggregatedPerformanceAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>(), fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetPortfolioPositionsStatisticsAsync_ReturnsCorrectStatistics_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positions = fixture.CreateMany<PositionDto>(2);
            var statistics = fixture.Create<PositionStatisticsDto>();

            var positionQueries = fixture.CreateDefaultPositionQueriesMock();
            positionQueries  
                .Setup(m => m.GetPortfolioPositionsAsync(It.IsAny<int>()))
                .ReturnsAsync(positions);

            var statisticsCalculator = fixture.Freeze<Mock<IPositionStatisticsCalculator>>();
            statisticsCalculator
                .Setup(c => c.CalculateStatistics(It.IsAny<PositionPriceListData>(), It.IsAny<DateTime>()))
                .Returns(statistics);

            var sut = fixture.Create<PositionService>();
            var result = await sut.GetPortfolioPositionsStatisticsAsync(fixture.Create<int>());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Collection(result.Response, s => Assert.Equal(statistics, s), s => Assert.Equal(statistics, s));
        }

        [Fact]
        public async Task GetPortfolioPositionsStatisticsAsync_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolioQueries = fixture.CreateDefaultPortfolioQueriesMock();
            portfolioQueries
                .Setup(m => m.GetPortfolioAsync(It.IsAny<int>()))
                .ReturnsAsync((PortfolioDto) null);

            var sut = fixture.Create<PositionService>();
            var result = await sut.GetPortfolioPositionsStatisticsAsync(fixture.Create<int>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetPositionStatisticsAsync_ReturnsCorrectStatistics_WhenPositionExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Create<PositionDto>();
            var statistics = fixture.Create<PositionStatisticsDto>();

            var positionQueries = fixture.CreateDefaultPositionQueriesMock();
            positionQueries
                .Setup(m => m.GetPositionAsync(It.IsAny<int>()))
                .ReturnsAsync(position);

            var statisticsCalculator = fixture.Freeze<Mock<IPositionStatisticsCalculator>>();
            statisticsCalculator
                .Setup(c => c.CalculateStatistics(It.IsAny<PositionPriceListData>(), It.IsAny<DateTime>()))
                .Returns(statistics);

            var sut = fixture.Create<PositionService>();
            var result = await sut.GetPositionStatisticsAsync(fixture.Create<int>());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(statistics, result.Response);
        }

        [Fact]
        public async Task GetPositionStatisticsAsync_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionQueries = fixture.CreateDefaultPositionQueriesMock();
            positionQueries
                .Setup(m => m.GetPositionAsync(It.IsAny<int>()))
                .ReturnsAsync((PositionDto) null);

            var sut = fixture.Create<PositionService>();
            var result = await sut.GetPositionStatisticsAsync(fixture.Create<int>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task OpeningPosition_AddsMatchingPositionToRepository_WhenWellFormed()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Create<PositionDto>();

            fixture.CreateDefaultPortfolioRepositoryMock();
            var positionRepository = fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.Freeze<Mock<IInstrumentPriceService>>();

            var sut = fixture.Create<PositionService>();

            await sut.OpenPositionAsync(position);

            positionRepository.Verify(r => r.Add(It.Is<Position>(p =>
                p.InstrumentId == position.InstrumentId &&
                p.PortfolioId == position.PortfolioId &&
                p.Note == position.Note &&
                p.Transactions.Count == 1 &&
                p.Transactions.First().Amount == position.Amount &&
                p.Transactions.First().Price == position.Price &&
                p.Transactions.First().Time == position.Time
            )), Times.Once());
        }

        [Fact]
        public async Task OpeningPosition_ThrowsException_WhenParentPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Create<PositionDto>();

            var portfolioRepository = fixture.CreateDefaultPortfolioRepositoryMock();
            portfolioRepository
                .Setup(r => r.ExistsAsync(position.PortfolioId))
                .Returns(Task.FromResult(false));
            portfolioRepository
                .Setup(r => r.FindAsync(position.PortfolioId))
                .Returns(Task.FromResult<Portfolio>(null));
            fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.Freeze<Mock<IInstrumentPriceService>>();

            var sut = fixture.Create<PositionService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await sut.OpenPositionAsync(position));
        }

        [Fact]
        public async Task OpeningPosition_ThrowsException_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Create<PositionDto>();

            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(r => r.ExistsAsync(position.InstrumentId))
                .Returns(Task.FromResult(false));
            instrumentRepository
                .Setup(r => r.FindAsync(position.InstrumentId))
                .Returns(Task.FromResult<Instrument>(null));
            fixture.Freeze<Mock<IInstrumentPriceService>>();

            var sut = fixture.Create<PositionService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await sut.OpenPositionAsync(position));
        }

        [Fact]
        public async Task OpeningPosition_ThrowsException_WhenInstrumentTypeIsIndex()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Create<PositionDto>();

            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(r => r.FindAsync(position.InstrumentId))
                .Returns(Task.FromResult(
                    new Instrument(
                        fixture.Create<string>(),
                        fixture.Create<string>(),
                        fixture.Create<string>(),
                        InstrumentType.Index,
                        fixture.Create<string>(),
                        fixture.Create<string>()
                    )
                ));
            fixture.Freeze<Mock<IInstrumentPriceService>>();

            var sut = fixture.Create<PositionService>();

            await Assert.ThrowsAsync<OperationNotAllowedException>(async () => await sut.OpenPositionAsync(position));
        }

        [Fact]
        public async Task OpeningPosition_ThrowsException_WhenAmountIsNull()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Build<PositionDto>().With(p => p.Amount, (decimal?)null).Create();

            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.Freeze<Mock<IInstrumentPriceService>>();

            var sut = fixture.Create<PositionService>();

            await Assert.ThrowsAsync<OperationNotAllowedException>(async () => await sut.OpenPositionAsync(position));
        }

        [Fact]
        public async Task OpeningPosition_ThrowsException_WhenAmountIsZero()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Build<PositionDto>().With(p => p.Amount, 0).Create();

            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.Freeze<Mock<IInstrumentPriceService>>();

            var sut = fixture.Create<PositionService>();

            await Assert.ThrowsAsync<OperationNotAllowedException>(async () => await sut.OpenPositionAsync(position));
        }

        [Fact]
        public async Task OpeningPosition_ThrowsException_WhenPriceIsNull()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Build<PositionDto>().With(p => p.Price, (decimal?)null).Create();

            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.Freeze<Mock<IInstrumentPriceService>>();

            var sut = fixture.Create<PositionService>();

            await Assert.ThrowsAsync<OperationNotAllowedException>(async () => await sut.OpenPositionAsync(position));
        }

        [Fact]
        public async Task OpeningPosition_ThrowsException_WhenPriceIsZero()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Build<PositionDto>().With(p => p.Price, 0).Create();

            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.Freeze<Mock<IInstrumentPriceService>>();

            var sut = fixture.Create<PositionService>();

            await Assert.ThrowsAsync<OperationNotAllowedException>(async () => await sut.OpenPositionAsync(position));
        }

        [Fact]
        public async Task OpeningPosition_ThrowsException_WhenTimeIsNull()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Build<PositionDto>().With(p => p.Time, (DateTime?)null).Create();

            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.Freeze<Mock<IInstrumentPriceService>>();

            var sut = fixture.Create<PositionService>();

            await Assert.ThrowsAsync<OperationNotAllowedException>(async () => await sut.OpenPositionAsync(position));
        }

        [Fact]
        public async Task OpeningPosition_ThrowsException_WhenTimeIsBeforeMinimumStartTime()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Build<PositionDto>()
                .With(p => p.Time, PortEvalConstants.FinancialDataStartTime.AddDays(-1)).Create();

            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.Freeze<Mock<IInstrumentPriceService>>();

            var sut = fixture.Create<PositionService>();

            await Assert.ThrowsAsync<OperationNotAllowedException>(async () => await sut.OpenPositionAsync(position));
        }

        [Fact]
        public async Task UpdatingPosition_UpdatesNote_WhenWellFormed()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Create<PositionDto>();

            fixture.CreateDefaultPortfolioRepositoryMock();
            var positionRepository = fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.Freeze<Mock<IInstrumentPriceService>>();

            var sut = fixture.Create<PositionService>();

            await sut.UpdatePositionAsync(position);

            positionRepository.Verify(r => r.Update(It.Is<Position>(p => p.Note == position.Note)));
        }

        [Fact]
        public async Task UpdatingPosition_ThrowsException_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Create<PositionDto>();

            fixture.CreateDefaultPortfolioRepositoryMock();
            var positionRepository = fixture.CreateDefaultPositionRepositoryMock();
            positionRepository
                .Setup(r => r.FindAsync(position.Id))
                .Returns(Task.FromResult<Position>(null));
            positionRepository
                .Setup(r => r.ExistsAsync(position.Id))
                .Returns(Task.FromResult(false));
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.Freeze<Mock<IInstrumentPriceService>>();

            var sut = fixture.Create<PositionService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await sut.UpdatePositionAsync(position));
        }

        [Fact]
        public async Task DeletingPosition_DeletesPosition_WhenPositionExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionId = fixture.Create<int>();

            fixture.CreateDefaultPortfolioRepositoryMock();
            var positionRepository = fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.Freeze<Mock<IInstrumentPriceService>>();

            var sut = fixture.Create<PositionService>();

            await sut.RemovePositionAsync(positionId);

            positionRepository.Verify(r => r.DeleteAsync(positionId), Times.Once());
        }

        [Fact]
        public async Task DeletingPosition_ThrowsException_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionId = fixture.Create<int>();

            fixture.CreateDefaultPortfolioRepositoryMock();
            var positionRepository = fixture.CreateDefaultPositionRepositoryMock();
            positionRepository
                .Setup(r => r.ExistsAsync(positionId))
                .Returns(Task.FromResult(false));
            positionRepository
                .Setup(r => r.FindAsync(positionId))
                .Returns(Task.FromResult<Position>(null));
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.Freeze<Mock<IInstrumentPriceService>>();

            var sut = fixture.Create<PositionService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await sut.RemovePositionAsync(positionId));
        }
    }
}