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
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Tests.Unit.Helpers;
using PortEval.Tests.Unit.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.Services
{
    public class PositionServiceTests
    {
        private IFixture _fixture;
        private Mock<IPortfolioRepository> _portfolioRepository;
        private Mock<IPositionRepository> _positionRepository;
        private Mock<IInstrumentRepository> _instrumentRepository;

        private Mock<IPositionQueries> _positionQueries;

        private Mock<ITransactionService> _transactionService;
        private Mock<IInstrumentPriceService> _priceService;
        private Mock<ICurrencyExchangeRateService> _exchangeRateService;

        private Mock<IPositionValueCalculator> _valueCalculator;
        private Mock<IPositionProfitCalculator> _profitCalculator;
        private Mock<IPositionPerformanceCalculator> _performanceCalculator;
        private Mock<IPositionBreakEvenPointCalculator> _bepCalculator;
        private Mock<IPositionStatisticsCalculator> _statisticsCalculator;

        private Mock<IPositionChartDataGenerator> _chartDataGenerator;
        private Mock<ICurrencyConverter> _currencyConverter;

        public PositionServiceTests()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            _portfolioRepository = _fixture.CreateDefaultPortfolioRepositoryMock();
            _positionRepository = _fixture.CreateDefaultPositionRepositoryMock();
            _instrumentRepository = _fixture.CreateDefaultInstrumentRepositoryMock();
            _positionQueries = _fixture.CreateDefaultPositionQueriesMock();

            _transactionService = _fixture.Freeze<Mock<ITransactionService>>();
            _transactionService
                .Setup(m => m.GetTransactionsAsync(It.IsAny<TransactionFilters>(), It.IsAny<DateRangeParams>()))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(_fixture.CreateMany<TransactionDto>()));

            _priceService = _fixture.Freeze<Mock<IInstrumentPriceService>>();
            _priceService
                .Setup(m => m.GetInstrumentPricesAsync(It.IsAny<int>(), It.IsAny<DateRangeParams>()))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(_fixture.CreateMany<InstrumentPriceDto>()));
            _priceService
                .Setup(m => m.GetInstrumentPricesAsync(It.IsAny<InstrumentDto>(), It.IsAny<DateRangeParams>()))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(_fixture.CreateMany<InstrumentPriceDto>()));
            _priceService
                .Setup(m => m.GetInstrumentPriceAsync(It.IsAny<int>(), It.IsAny<DateTime>()))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(_fixture.Create<InstrumentPriceDto>()));

            _exchangeRateService = _fixture.Freeze<Mock<ICurrencyExchangeRateService>>();
            _exchangeRateService
                .Setup(m => m.GetExchangeRatesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateRangeParams>()))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(_fixture.CreateMany<CurrencyExchangeRateDto>()));

            _valueCalculator = _fixture.Freeze<Mock<IPositionValueCalculator>>();
            _profitCalculator = _fixture.Freeze<Mock<IPositionProfitCalculator>>();
            _performanceCalculator = _fixture.Freeze<Mock<IPositionPerformanceCalculator>>();
            _bepCalculator = _fixture.Freeze<Mock<IPositionBreakEvenPointCalculator>>();
            _statisticsCalculator = _fixture.Freeze<Mock<IPositionStatisticsCalculator>>();

            _chartDataGenerator = _fixture.Freeze<Mock<IPositionChartDataGenerator>>();
            _currencyConverter = _fixture.Freeze<Mock<ICurrencyConverter>>();
            _currencyConverter
                .Setup(m => m.ConvertChartPoints(It.IsAny<IEnumerable<EntityChartPointDto>>(),
                    It.IsAny<IEnumerable<CurrencyExchangeRateDto>>()))
                .Returns<IEnumerable<EntityChartPointDto>, IEnumerable<CurrencyExchangeRateDto>>((points, rates) =>
                    points);
        }

        [Fact]
        public async Task GetAllPositionsAsync_ReturnsAllPositions()
        {
            var positions = _fixture.CreateMany<PositionDto>();

            _positionQueries
                .Setup(m => m.GetAllPositionsAsync())
                .ReturnsAsync(positions);

            var sut = _fixture.Create<PositionService>();
            var result = await sut.GetAllPositionsAsync();

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(positions, result.Response);
        }

        [Fact]
        public async Task GetPortfolioPositionsAsync_ReturnsPortfolioPositions_WhenPortfolioExists()
        {
            var portfolioId = _fixture.Create<int>();
            var positions = _fixture.CreateMany<PositionDto>();

            _positionQueries
                .Setup(m => m.GetPortfolioPositionsAsync(portfolioId))
                .ReturnsAsync(positions);

            var sut = _fixture.Create<PositionService>();
            var result = await sut.GetPortfolioPositionsAsync(portfolioId);

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(positions, result.Response);
        }

        [Fact]
        public async Task GetPortfolioPositionsAsync_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var portfolioId = _fixture.Create<int>();

            _portfolioRepository
                .Setup(m => m.ExistsAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            var sut = _fixture.Create<PositionService>();
            var result = await sut.GetPortfolioPositionsAsync(portfolioId);

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetPositionAsync_ReturnsPosition_WhenItExists()
        {
            var positionId = _fixture.Create<int>();
            var position = _fixture.Create<PositionDto>();

            _positionQueries
                .Setup(m => m.GetPositionAsync(positionId))
                .ReturnsAsync(position);

            var sut = _fixture.Create<PositionService>();
            var result = await sut.GetPositionAsync(positionId);

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(position, result.Response);
        }

        [Fact]
        public async Task GetPositionAsync_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var positionId = _fixture.Create<int>();

            _positionQueries
                .Setup(m => m.GetPositionAsync(positionId))
                .ReturnsAsync((PositionDto)null);

            var sut = _fixture.Create<PositionService>();
            var result = await sut.GetPositionAsync(positionId);

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetPositionValueAsync_ReturnsCorrectPositionValue_WhenPositionExists()
        {
            var value = _fixture.Create<decimal>();
            var time = _fixture.Create<DateTime>();

            _valueCalculator
                .Setup(c => c.CalculateValue(It.IsAny<IEnumerable<PositionPriceRangeData>>(), time))
                .Returns(value);

            var sut = _fixture.Create<PositionService>();
            var result = await sut.GetPositionValueAsync(_fixture.Create<int>(), time);

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(value, result.Response.Value);
            Assert.Equal(time, result.Response.Time);
        }

        [Fact]
        public async Task GetPositionValueAsync_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            _positionQueries
                .Setup(m => m.GetPositionAsync(It.IsAny<int>()))
                .ReturnsAsync((PositionDto)null);

            var sut = _fixture.Create<PositionService>();
            var result = await sut.GetPositionValueAsync(_fixture.Create<int>(), _fixture.Create<DateTime>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetPositionProfitAsync_ReturnsCorrectProfit_WhenPositionExists()
        {
            var profit = _fixture.Create<decimal>();
            var dateRange = _fixture.Create<DateRangeParams>();

            _profitCalculator
                .Setup(c => c.CalculateProfit(It.IsAny<IEnumerable<PositionPriceRangeData>>(), dateRange.From, dateRange.To))
                .Returns(profit);

            var sut = _fixture.Create<PositionService>();
            var result = await sut.GetPositionProfitAsync(_fixture.Create<int>(), dateRange);

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(profit, result.Response.Profit);
            Assert.Equal(dateRange.From, result.Response.From);
            Assert.Equal(dateRange.To, result.Response.To);
        }

        [Fact]
        public async Task GetPositionProfitAsync_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            _positionQueries
                .Setup(m => m.GetPositionAsync(It.IsAny<int>()))
                .ReturnsAsync((PositionDto)null);

            var sut = _fixture.Create<PositionService>();
            var result = await sut.GetPositionProfitAsync(_fixture.Create<int>(), _fixture.Create<DateRangeParams>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetPositionPerformanceAsync_ReturnsCorrectPerformance_WhenPositionExists()
        {
            var performance = _fixture.Create<decimal>();
            var dateRange = _fixture.Create<DateRangeParams>();

            _performanceCalculator
                .Setup(c => c.CalculatePerformance(It.IsAny<IEnumerable<PositionPriceRangeData>>(), dateRange.From, dateRange.To))
                .Returns(performance);

            var sut = _fixture.Create<PositionService>();
            var result = await sut.GetPositionPerformanceAsync(_fixture.Create<int>(), dateRange);

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(performance, result.Response.Performance);
            Assert.Equal(dateRange.From, result.Response.From);
            Assert.Equal(dateRange.To, result.Response.To);
        }

        [Fact]
        public async Task GetPositionPerformanceAsync_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            _positionQueries
                .Setup(m => m.GetPositionAsync(It.IsAny<int>()))
                .ReturnsAsync((PositionDto)null);

            var sut = _fixture.Create<PositionService>();
            var result = await sut.GetPositionProfitAsync(_fixture.Create<int>(), _fixture.Create<DateRangeParams>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetPositionBreakEvenPointAsync_ReturnsCorrectBreakEvenPoint_WhenPositionExists()
        {
            var breakEvenPoint = _fixture.Create<decimal>();
            var time = _fixture.Create<DateTime>();

            _bepCalculator
                .Setup(c => c.CalculatePositionBreakEvenPoint(It.IsAny<IEnumerable<TransactionDto>>()))
                .Returns(breakEvenPoint);

            var sut = _fixture.Create<PositionService>();
            var result = await sut.GetPositionBreakEvenPointAsync(_fixture.Create<int>(), time);

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(breakEvenPoint, result.Response.BreakEvenPoint);
            Assert.Equal(time, result.Response.Time);
        }

        [Fact]
        public async Task GetPositionBreakEvenPointAsync_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            _positionQueries
                .Setup(m => m.GetPositionAsync(It.IsAny<int>()))
                .ReturnsAsync((PositionDto)null);

            var sut = _fixture.Create<PositionService>();
            var result = await sut.GetPositionBreakEvenPointAsync(_fixture.Create<int>(), _fixture.Create<DateTime>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task ChartPositionValueAsync_ReturnsCorrectValueChart_WhenPositionExists()
        {
            var valueChart = _fixture.CreateMany<EntityChartPointDto>();
            var dateRange = _fixture.Create<DateRangeParams>();

            _chartDataGenerator
                .Setup(c => c.ChartValue(It.IsAny<PositionPriceListData>(), dateRange, It.IsAny<AggregationFrequency>()))
                .Returns(valueChart);

            var sut = _fixture.Create<PositionService>();
            var result = await sut.ChartPositionValueAsync(_fixture.Create<int>(), dateRange, _fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(valueChart, result.Response);
        }

        [Fact]
        public async Task ChartPositionValueAsync_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            _positionQueries
                .Setup(m => m.GetPositionAsync(It.IsAny<int>()))
                .ReturnsAsync((PositionDto)null);

            var sut = _fixture.Create<PositionService>();
            var result = await sut.ChartPositionValueAsync(_fixture.Create<int>(), _fixture.Create<DateRangeParams>(), _fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task ChartPositionProfitAsync_ReturnsCorrectProfitChart_WhenPositionExists()
        {
            var profitChart = _fixture.CreateMany<EntityChartPointDto>();
            var dateRange = _fixture.Create<DateRangeParams>();

            _chartDataGenerator
                .Setup(c => c.ChartProfit(It.IsAny<PositionPriceListData>(), dateRange, It.IsAny<AggregationFrequency>()))
                .Returns(profitChart);

            var sut = _fixture.Create<PositionService>();
            var result = await sut.ChartPositionProfitAsync(_fixture.Create<int>(), dateRange, _fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(profitChart, result.Response);
        }

        [Fact]
        public async Task ChartPositionProfitAsync_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            _positionQueries
                .Setup(m => m.GetPositionAsync(It.IsAny<int>()))
                .ReturnsAsync((PositionDto)null);

            var sut = _fixture.Create<PositionService>();
            var result = await sut.ChartPositionProfitAsync(_fixture.Create<int>(), _fixture.Create<DateRangeParams>(), _fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task ChartPositionPerformanceAsync_ReturnsCorrectPerformanceChart_WhenPositionExists()
        {
            var performanceChart = _fixture.CreateMany<EntityChartPointDto>();
            var dateRange = _fixture.Create<DateRangeParams>();

            _chartDataGenerator
                .Setup(c => c.ChartPerformance(It.IsAny<PositionPriceListData>(), dateRange, It.IsAny<AggregationFrequency>()))
                .Returns(performanceChart);

            var sut = _fixture.Create<PositionService>();
            var result = await sut.ChartPositionPerformanceAsync(_fixture.Create<int>(), dateRange, _fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(performanceChart, result.Response);
        }

        [Fact]
        public async Task ChartPositionPerformanceAsync_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            _positionQueries
                .Setup(m => m.GetPositionAsync(It.IsAny<int>()))
                .ReturnsAsync((PositionDto)null);

            var sut = _fixture.Create<PositionService>();
            var result = await sut.ChartPositionPerformanceAsync(_fixture.Create<int>(), _fixture.Create<DateRangeParams>(), _fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task ChartPositionAggregatedProfitAsync_ReturnsCorrectAggregatedProfit_WhenPositionExists()
        {
            var aggregatedProfitChart = _fixture.CreateMany<EntityChartPointDto>();
            var dateRange = _fixture.Create<DateRangeParams>();

            _chartDataGenerator
                .Setup(c => c.ChartAggregatedProfit(It.IsAny<PositionPriceListData>(), dateRange, It.IsAny<AggregationFrequency>()))
                .Returns(aggregatedProfitChart);

            var sut = _fixture.Create<PositionService>();
            var result = await sut.ChartPositionAggregatedProfitAsync(_fixture.Create<int>(), dateRange, _fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(aggregatedProfitChart, result.Response);
        }

        [Fact]
        public async Task ChartPositionAggregatedProfitAsync_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            _positionQueries
                .Setup(m => m.GetPositionAsync(It.IsAny<int>()))
                .ReturnsAsync((PositionDto)null);

            var sut = _fixture.Create<PositionService>();
            var result = await sut.ChartPositionAggregatedProfitAsync(_fixture.Create<int>(), _fixture.Create<DateRangeParams>(), _fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task ChartPositionAggregatedPerformanceAsync_ReturnsCorrectAggregatedPerformance_WhenPositionExists()
        {
            var aggregatedPerformanceChart = _fixture.CreateMany<EntityChartPointDto>();
            var dateRange = _fixture.Create<DateRangeParams>();

            _chartDataGenerator
                .Setup(c => c.ChartAggregatedPerformance(It.IsAny<PositionPriceListData>(), dateRange, It.IsAny<AggregationFrequency>()))
                .Returns(aggregatedPerformanceChart);

            var sut = _fixture.Create<PositionService>();
            var result = await sut.ChartPositionAggregatedPerformanceAsync(_fixture.Create<int>(), dateRange, _fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(aggregatedPerformanceChart, result.Response);
        }

        [Fact]
        public async Task ChartPositionAggregatedPerformanceAsync_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            _positionQueries
                .Setup(m => m.GetPositionAsync(It.IsAny<int>()))
                .ReturnsAsync((PositionDto)null);

            var sut = _fixture.Create<PositionService>();
            var result = await sut.ChartPositionAggregatedPerformanceAsync(_fixture.Create<int>(), _fixture.Create<DateRangeParams>(), _fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetPortfolioPositionsStatisticsAsync_ReturnsCorrectStatistics_WhenPortfolioExists()
        {
            var positions = _fixture.CreateMany<PositionDto>(2);
            var statistics = _fixture.Create<PositionStatisticsDto>();

            _positionQueries
                .Setup(m => m.GetPortfolioPositionsAsync(It.IsAny<int>()))
                .ReturnsAsync(positions);

            _statisticsCalculator
                .Setup(c => c.CalculateStatistics(It.IsAny<PositionPriceListData>(), It.IsAny<DateTime>()))
                .Returns(statistics);

            var sut = _fixture.Create<PositionService>();
            var result = await sut.GetPortfolioPositionsStatisticsAsync(_fixture.Create<int>());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Collection(result.Response, s => Assert.Equal(statistics, s), s => Assert.Equal(statistics, s));
        }

        [Fact]
        public async Task GetPortfolioPositionsStatisticsAsync_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            _portfolioRepository
                .Setup(m => m.ExistsAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            var sut = _fixture.Create<PositionService>();
            var result = await sut.GetPortfolioPositionsStatisticsAsync(_fixture.Create<int>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetPositionStatisticsAsync_ReturnsCorrectStatistics_WhenPositionExists()
        {
            var position = _fixture.Create<PositionDto>();
            var statistics = _fixture.Create<PositionStatisticsDto>();

            _positionQueries
                .Setup(m => m.GetPositionAsync(It.IsAny<int>()))
                .ReturnsAsync(position);

            _statisticsCalculator
                .Setup(c => c.CalculateStatistics(It.IsAny<PositionPriceListData>(), It.IsAny<DateTime>()))
                .Returns(statistics);

            var sut = _fixture.Create<PositionService>();
            var result = await sut.GetPositionStatisticsAsync(_fixture.Create<int>());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(statistics, result.Response);
        }

        [Fact]
        public async Task GetPositionStatisticsAsync_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            _positionQueries
                .Setup(m => m.GetPositionAsync(It.IsAny<int>()))
                .ReturnsAsync((PositionDto)null);

            var sut = _fixture.Create<PositionService>();
            var result = await sut.GetPositionStatisticsAsync(_fixture.Create<int>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task OpeningPosition_AddsMatchingPositionToRepository_WhenWellFormed()
        {
            var position = _fixture.Create<PositionDto>();

            var sut = _fixture.Create<PositionService>();

            await sut.OpenPositionAsync(position);

            _positionRepository.Verify(r => r.Add(It.Is<Position>(p =>
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
        public async Task OpeningPosition_ReturnsError_WhenParentPortfolioDoesNotExist()
        {
            var position = _fixture.Create<PositionDto>();

            _portfolioRepository
                .Setup(r => r.ExistsAsync(position.PortfolioId))
                .Returns(Task.FromResult(false));
            _portfolioRepository
                .Setup(r => r.FindAsync(position.PortfolioId))
                .Returns(Task.FromResult<Portfolio>(null));

            var sut = _fixture.Create<PositionService>();

            var response = await sut.OpenPositionAsync(position);
            Assert.Equal(OperationStatus.Error, response.Status);
        }

        [Fact]
        public async Task OpeningPosition_ReturnsError_WhenInstrumentDoesNotExist()
        {
            var position = _fixture.Create<PositionDto>();

            _instrumentRepository
                .Setup(r => r.ExistsAsync(position.InstrumentId))
                .Returns(Task.FromResult(false));
            _instrumentRepository
                .Setup(r => r.FindAsync(position.InstrumentId))
                .Returns(Task.FromResult<Instrument>(null));

            var sut = _fixture.Create<PositionService>();

            var response = await sut.OpenPositionAsync(position);
            Assert.Equal(OperationStatus.Error, response.Status);
        }

        [Fact]
        public async Task OpeningPosition_ReturnsError_WhenInstrumentTypeIsIndex()
        {
            var position = _fixture.Create<PositionDto>();

            _instrumentRepository
                .Setup(r => r.FindAsync(position.InstrumentId))
                .Returns(Task.FromResult(
                    new Instrument(
                        _fixture.Create<string>(),
                        _fixture.Create<string>(),
                        _fixture.Create<string>(),
                        InstrumentType.Index,
                        _fixture.Create<string>(),
                        _fixture.Create<string>()
                    )
                ));

            var sut = _fixture.Create<PositionService>();

            var response = await sut.OpenPositionAsync(position);
            Assert.Equal(OperationStatus.Error, response.Status);
        }

        [Fact]
        public async Task OpeningPosition_ReturnsError_WhenAmountIsNull()
        {
            var position = _fixture.Build<PositionDto>().With(p => p.Amount, (decimal?)null).Create();

            var sut = _fixture.Create<PositionService>();

            var response = await sut.OpenPositionAsync(position);
            Assert.Equal(OperationStatus.Error, response.Status);
        }

        [Fact]
        public async Task OpeningPosition_ReturnsError_WhenAmountIsZero()
        {
            var position = _fixture.Build<PositionDto>().With(p => p.Amount, 0).Create();

            var sut = _fixture.Create<PositionService>();

            var response = await sut.OpenPositionAsync(position);
            Assert.Equal(OperationStatus.Error, response.Status);
        }

        [Fact]
        public async Task OpeningPosition_ReturnsError_WhenPriceIsNull()
        {
            var position = _fixture.Build<PositionDto>().With(p => p.Price, (decimal?)null).Create();

            var sut = _fixture.Create<PositionService>();

            var response = await sut.OpenPositionAsync(position);
            Assert.Equal(OperationStatus.Error, response.Status);
        }

        [Fact]
        public async Task OpeningPosition_ReturnsError_WhenPriceIsZero()
        {
            var position = _fixture.Build<PositionDto>().With(p => p.Price, 0).Create();

            var sut = _fixture.Create<PositionService>();

            var response = await sut.OpenPositionAsync(position);
            Assert.Equal(OperationStatus.Error, response.Status);
        }

        [Fact]
        public async Task OpeningPosition_ReturnsError_WhenTimeIsNull()
        {
            var position = _fixture.Build<PositionDto>().With(p => p.Time, (DateTime?)null).Create();

            var sut = _fixture.Create<PositionService>();

            var response = await sut.OpenPositionAsync(position);
            Assert.Equal(OperationStatus.Error, response.Status);
        }

        [Fact]
        public async Task OpeningPosition_ReturnsError_WhenTimeIsBeforeMinimumStartTime()
        {
            var position = _fixture.Build<PositionDto>()
                .With(p => p.Time, PortEvalConstants.FinancialDataStartTime.AddDays(-1)).Create();

            var sut = _fixture.Create<PositionService>();

            var response = await sut.OpenPositionAsync(position);
            Assert.Equal(OperationStatus.Error, response.Status);
        }

        [Fact]
        public async Task UpdatingPosition_UpdatesNote_WhenWellFormed()
        {
            var position = _fixture.Create<PositionDto>();

            var sut = _fixture.Create<PositionService>();

            await sut.UpdatePositionAsync(position);

            _positionRepository.Verify(r => r.Update(It.Is<Position>(p => p.Note == position.Note)));
        }

        [Fact]
        public async Task UpdatingPosition_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var position = _fixture.Create<PositionDto>();

            _positionRepository
                .Setup(r => r.FindAsync(position.Id))
                .Returns(Task.FromResult<Position>(null));
            _positionRepository
                .Setup(r => r.ExistsAsync(position.Id))
                .Returns(Task.FromResult(false));

            var sut = _fixture.Create<PositionService>();

            var response = await sut.UpdatePositionAsync(position);
            Assert.Equal(OperationStatus.NotFound, response.Status);
        }

        [Fact]
        public async Task DeletingPosition_DeletesPosition_WhenPositionExists()
        {
            var positionId = _fixture.Create<int>();

            var sut = _fixture.Create<PositionService>();

            await sut.RemovePositionAsync(positionId);

            _positionRepository.Verify(r => r.DeleteAsync(positionId), Times.Once());
        }

        [Fact]
        public async Task DeletingPosition_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var positionId = _fixture.Create<int>();

            _positionRepository
                .Setup(r => r.ExistsAsync(positionId))
                .Returns(Task.FromResult(false));
            _positionRepository
                .Setup(r => r.FindAsync(positionId))
                .Returns(Task.FromResult<Position>(null));

            var sut = _fixture.Create<PositionService>();

            var response = await sut.RemovePositionAsync(positionId);
            Assert.Equal(OperationStatus.NotFound, response.Status);
        }
    }
}