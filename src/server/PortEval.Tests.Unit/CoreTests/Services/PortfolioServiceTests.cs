using System;
using System.Collections.Generic;
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
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Tests.Unit.Helpers;
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.Services;

public class PortfolioServiceTests
{
    private readonly Mock<ICurrencyConverter> _currencyConverter;
    private readonly Mock<ICurrencyRepository> _currencyRepository;
    private readonly Mock<ICurrencyExchangeRateService> _exchangeRateService;
    private readonly IFixture _fixture;

    private readonly Mock<IPortfolioChartDataGenerator> _chartDataGenerator;
    private readonly Mock<IPositionPerformanceCalculator> _performanceCalculator;

    private readonly Mock<IPortfolioQueries> _portfolioQueries;
    private readonly Mock<IPortfolioRepository> _portfolioRepository;

    private readonly Mock<IPositionService> _positionService;
    private readonly Mock<IPositionProfitCalculator> _profitCalculator;
    private readonly Mock<IPortfolioStatisticsCalculator> _statisticsCalculator;

    private readonly Mock<IPositionValueCalculator> _valueCalculator;

    public PortfolioServiceTests()
    {
        _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
        _portfolioRepository = _fixture.CreateDefaultPortfolioRepositoryMock();
        _currencyRepository = _fixture.CreateDefaultCurrencyRepositoryMock();
        _portfolioQueries = _fixture.CreateDefaultPortfolioQueriesMock();
        _positionService = _fixture.Freeze<Mock<IPositionService>>();
        _positionService
            .Setup(m => m.GetPortfolioPositionsAsync(It.IsAny<int>()))
            .ReturnsAsync(
                OperationResponseHelper.GenerateSuccessfulOperationResponse(_fixture.CreateMany<PositionDto>()));
        _positionService
            .Setup(m => m.GetPortfolioPositionsAsync(It.IsAny<PortfolioDto>()))
            .ReturnsAsync(
                OperationResponseHelper.GenerateSuccessfulOperationResponse(_fixture.CreateMany<PositionDto>()));
        _positionService
            .Setup(m => m.GetPositionPriceListDataAsync(It.IsAny<PositionDto>(), It.IsAny<DateRangeParams>()))
            .ReturnsAsync(
                OperationResponseHelper.GenerateSuccessfulOperationResponse(_fixture.Create<PositionPriceListData>()));
        _positionService
            .Setup(m => m.GetPositionPriceRangeDataAsync(It.IsAny<PositionDto>(), It.IsAny<DateRangeParams>()))
            .ReturnsAsync(
                OperationResponseHelper.GenerateSuccessfulOperationResponse(_fixture.Create<PositionPriceRangeData>()));

        _exchangeRateService = _fixture.Freeze<Mock<ICurrencyExchangeRateService>>();
        _exchangeRateService
            .Setup(m => m.GetExchangeRatesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateRangeParams>()))
            .ReturnsAsync(
                OperationResponseHelper.GenerateSuccessfulOperationResponse(
                    _fixture.CreateMany<CurrencyExchangeRateDto>()));

        _valueCalculator = _fixture.Freeze<Mock<IPositionValueCalculator>>();
        _profitCalculator = _fixture.Freeze<Mock<IPositionProfitCalculator>>();
        _performanceCalculator = _fixture.Freeze<Mock<IPositionPerformanceCalculator>>();
        _statisticsCalculator = _fixture.Freeze<Mock<IPortfolioStatisticsCalculator>>();
        _chartDataGenerator = _fixture.Freeze<Mock<IPortfolioChartDataGenerator>>();
        _currencyConverter = _fixture.Freeze<Mock<ICurrencyConverter>>();
        _currencyConverter
            .Setup(m => m.ConvertChartPoints(It.IsAny<IEnumerable<EntityChartPointDto>>(),
                It.IsAny<IEnumerable<CurrencyExchangeRateDto>>()))
            .Returns<IEnumerable<EntityChartPointDto>, IEnumerable<CurrencyExchangeRateDto>>((points, _) =>
                points);
        _currencyConverter
            .Setup(m => m.ConvertInstrumentPrices(It.IsAny<IEnumerable<InstrumentPriceDto>>(),
                It.IsAny<IEnumerable<CurrencyExchangeRateDto>>()))
            .Returns<IEnumerable<InstrumentPriceDto>, IEnumerable<CurrencyExchangeRateDto>>((prices, _) =>
                prices);
        _currencyConverter
            .Setup(m => m.ConvertTransactions(It.IsAny<IEnumerable<TransactionDto>>(),
                It.IsAny<IEnumerable<CurrencyExchangeRateDto>>()))
            .Returns<IEnumerable<TransactionDto>, IEnumerable<CurrencyExchangeRateDto>>((transactions, _) =>
                transactions);
    }

    [Fact]
    public async Task GetAllPortfoliosAsync_ReturnsAllPortfolios()
    {
        var portfolios = _fixture.CreateMany<PortfolioDto>();

        _portfolioQueries
            .Setup(q => q.GetAllPortfoliosAsync())
            .ReturnsAsync(portfolios);

        var sut = _fixture.Create<PortfolioService>();
        var result = await sut.GetAllPortfoliosAsync();

        Assert.Equal(OperationStatus.Ok, result.Status);
        Assert.Equal(portfolios, result.Response);
    }

    [Fact]
    public async Task GetPortfolioAsync_ReturnsPortfolio_WhenPortfolioExists()
    {
        var portfolio = _fixture.Create<PortfolioDto>();

        _portfolioQueries
            .Setup(m => m.GetPortfolioAsync(portfolio.Id))
            .ReturnsAsync(portfolio);

        var sut = _fixture.Create<PortfolioService>();
        var result = await sut.GetPortfolioAsync(portfolio.Id);

        Assert.Equal(OperationStatus.Ok, result.Status);
        Assert.Equal(portfolio, result.Response);
    }

    [Fact]
    public async Task GetPortfolioAsync_ReturnsNotFound_WhenPortfolioDoesNotExist()
    {
        var portfolio = _fixture.Create<PortfolioDto>();

        _portfolioQueries
            .Setup(m => m.GetPortfolioAsync(portfolio.Id))
            .ReturnsAsync((PortfolioDto)null);

        var sut = _fixture.Create<PortfolioService>();
        var result = await sut.GetPortfolioAsync(portfolio.Id);

        Assert.Equal(OperationStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task GetPortfolioValueAsync_ReturnsCorrectValue_WhenPortfolioExists()
    {
        var value = _fixture.Create<decimal>();
        var time = _fixture.Create<DateTime>();

        _valueCalculator
            .Setup(c => c.CalculateValue(It.IsAny<IEnumerable<PositionPriceRangeData>>(), time))
            .Returns(value);

        var sut = _fixture.Create<PortfolioService>();
        var result = await sut.GetPortfolioValueAsync(_fixture.Create<int>(), time);

        Assert.Equal(OperationStatus.Ok, result.Status);
        Assert.Equal(value, result.Response.Value);
        Assert.Equal(time, result.Response.Time);
    }

    [Fact]
    public async Task GetPortfolioValueAsync_ReturnsNotFound_WhenPortfolioDoesNotExist()
    {
        _portfolioQueries
            .Setup(m => m.GetPortfolioAsync(It.IsAny<int>()))
            .ReturnsAsync((PortfolioDto)null);

        var sut = _fixture.Create<PortfolioService>();
        var result = await sut.GetPortfolioValueAsync(_fixture.Create<int>(), _fixture.Create<DateTime>());

        Assert.Equal(OperationStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task GetPortfolioProfitAsync_ReturnsCorrectProfit_WhenPortfolioExists()
    {
        var profit = _fixture.Create<decimal>();
        var dateRange = _fixture.Create<DateRangeParams>();

        _profitCalculator
            .Setup(c => c.CalculateProfit(It.IsAny<IEnumerable<PositionPriceRangeData>>(), dateRange.From,
                dateRange.To))
            .Returns(profit);

        var sut = _fixture.Create<PortfolioService>();
        var result = await sut.GetPortfolioProfitAsync(_fixture.Create<int>(), dateRange);

        Assert.Equal(OperationStatus.Ok, result.Status);
        Assert.Equal(profit, result.Response.Profit);
        Assert.Equal(dateRange.From, result.Response.From);
        Assert.Equal(dateRange.To, result.Response.To);
    }

    [Fact]
    public async Task GetPortfolioProfitAsync_ReturnsNotFound_WhenPortfolioDoesNotExist()
    {
        _portfolioQueries
            .Setup(m => m.GetPortfolioAsync(It.IsAny<int>()))
            .ReturnsAsync((PortfolioDto)null);

        var sut = _fixture.Create<PortfolioService>();
        var result = await sut.GetPortfolioProfitAsync(_fixture.Create<int>(), _fixture.Create<DateRangeParams>());

        Assert.Equal(OperationStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task GetPortfolioPerformanceAsync_ReturnsCorrectPerformance_WhenPortfolioExists()
    {
        var performance = _fixture.Create<decimal>();
        var dateRange = _fixture.Create<DateRangeParams>();

        _performanceCalculator
            .Setup(c => c.CalculatePerformance(It.IsAny<IEnumerable<PositionPriceRangeData>>(), dateRange.From,
                dateRange.To))
            .Returns(performance);

        var sut = _fixture.Create<PortfolioService>();
        var result = await sut.GetPortfolioPerformanceAsync(_fixture.Create<int>(), dateRange);

        Assert.Equal(OperationStatus.Ok, result.Status);
        Assert.Equal(performance, result.Response.Performance);
        Assert.Equal(dateRange.From, result.Response.From);
        Assert.Equal(dateRange.To, result.Response.To);
    }

    [Fact]
    public async Task GetPortfolioPerformanceAsync_ReturnsNotFound_WhenPortfolioDoesNotExist()
    {
        _portfolioQueries
            .Setup(m => m.GetPortfolioAsync(It.IsAny<int>()))
            .ReturnsAsync((PortfolioDto)null);

        var sut = _fixture.Create<PortfolioService>();
        var result = await sut.GetPortfolioPerformanceAsync(_fixture.Create<int>(), _fixture.Create<DateRangeParams>());

        Assert.Equal(OperationStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task ChartPortfolioValueAsync_ReturnsCorrectValueChart_WhenPortfolioExists()
    {
        var valueChart = _fixture.CreateMany<EntityChartPointDto>();

        _chartDataGenerator
            .Setup(c => c.ChartValue(It.IsAny<PortfolioPositionsPriceListData>(), It.IsAny<DateRangeParams>(),
                It.IsAny<AggregationFrequency>()))
            .Returns(valueChart);

        var sut = _fixture.Create<PortfolioService>();
        var result = await sut.ChartPortfolioValueAsync(_fixture.Create<int>(), _fixture.Create<DateRangeParams>(),
            _fixture.Create<AggregationFrequency>());

        Assert.Equal(OperationStatus.Ok, result.Status);
        Assert.Equal(valueChart, result.Response);
    }

    [Fact]
    public async Task ChartPortfolioValueAsync_ReturnsNotFound_WhenPortfolioDoesNotExist()
    {
        _portfolioQueries
            .Setup(m => m.GetPortfolioAsync(It.IsAny<int>()))
            .ReturnsAsync((PortfolioDto)null);

        var sut = _fixture.Create<PortfolioService>();
        var result = await sut.ChartPortfolioValueAsync(_fixture.Create<int>(), _fixture.Create<DateRangeParams>(),
            _fixture.Create<AggregationFrequency>());

        Assert.Equal(OperationStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task ChartPortfolioProfitAsync_ReturnsCorrectProfitChart_WhenPortfolioExists()
    {
        var profitChart = _fixture.CreateMany<EntityChartPointDto>();

        _chartDataGenerator
            .Setup(c => c.ChartProfit(It.IsAny<PortfolioPositionsPriceListData>(), It.IsAny<DateRangeParams>(),
                It.IsAny<AggregationFrequency>()))
            .Returns(profitChart);

        var sut = _fixture.Create<PortfolioService>();
        var result = await sut.ChartPortfolioProfitAsync(_fixture.Create<int>(), _fixture.Create<DateRangeParams>(),
            _fixture.Create<AggregationFrequency>());

        Assert.Equal(OperationStatus.Ok, result.Status);
        Assert.Equal(profitChart, result.Response);
    }

    [Fact]
    public async Task ChartPortfolioProfitAsync_ReturnsNotFound_WhenPortfolioDoesNotExist()
    {
        _portfolioQueries
            .Setup(m => m.GetPortfolioAsync(It.IsAny<int>()))
            .ReturnsAsync((PortfolioDto)null);

        var sut = _fixture.Create<PortfolioService>();
        var result = await sut.ChartPortfolioProfitAsync(_fixture.Create<int>(), _fixture.Create<DateRangeParams>(),
            _fixture.Create<AggregationFrequency>());

        Assert.Equal(OperationStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task ChartPortfolioPerformanceAsync_ReturnsCorrectPerformanceChart_WhenPortfolioExists()
    {
        var performanceChart = _fixture.CreateMany<EntityChartPointDto>();

        _chartDataGenerator
            .Setup(c => c.ChartPerformance(It.IsAny<PortfolioPositionsPriceListData>(), It.IsAny<DateRangeParams>(),
                It.IsAny<AggregationFrequency>()))
            .Returns(performanceChart);

        var sut = _fixture.Create<PortfolioService>();
        var result = await sut.ChartPortfolioPerformanceAsync(_fixture.Create<int>(),
            _fixture.Create<DateRangeParams>(), _fixture.Create<AggregationFrequency>());

        Assert.Equal(OperationStatus.Ok, result.Status);
        Assert.Equal(performanceChart, result.Response);
    }

    [Fact]
    public async Task ChartPortfolioPerformanceAsync_ReturnsNotFound_WhenPortfolioDoesNotExist()
    {
        _portfolioQueries
            .Setup(m => m.GetPortfolioAsync(It.IsAny<int>()))
            .ReturnsAsync((PortfolioDto)null);

        var sut = _fixture.Create<PortfolioService>();
        var result = await sut.ChartPortfolioPerformanceAsync(_fixture.Create<int>(),
            _fixture.Create<DateRangeParams>(), _fixture.Create<AggregationFrequency>());

        Assert.Equal(OperationStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task ChartPortfolioAggregatedProfitAsync_ReturnsCorrectAggregatedProfitChart_WhenPortfolioExists()
    {
        var aggregatedProfitChart = _fixture.CreateMany<EntityChartPointDto>();

        _chartDataGenerator
            .Setup(c => c.ChartAggregatedProfit(It.IsAny<PortfolioPositionsPriceListData>(),
                It.IsAny<DateRangeParams>(), It.IsAny<AggregationFrequency>()))
            .Returns(aggregatedProfitChart);

        var sut = _fixture.Create<PortfolioService>();
        var result = await sut.ChartPortfolioAggregatedProfitAsync(_fixture.Create<int>(),
            _fixture.Create<DateRangeParams>(), _fixture.Create<AggregationFrequency>());

        Assert.Equal(OperationStatus.Ok, result.Status);
        Assert.Equal(aggregatedProfitChart, result.Response);
    }

    [Fact]
    public async Task ChartPortfolioAggregatedProfitAsync_ReturnsNotFound_WhenPortfolioDoesNotExist()
    {
        _portfolioQueries
            .Setup(m => m.GetPortfolioAsync(It.IsAny<int>()))
            .ReturnsAsync((PortfolioDto)null);

        var sut = _fixture.Create<PortfolioService>();
        var result = await sut.ChartPortfolioAggregatedProfitAsync(_fixture.Create<int>(),
            _fixture.Create<DateRangeParams>(), _fixture.Create<AggregationFrequency>());

        Assert.Equal(OperationStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task
        ChartPortfolioAggregatedPerformanceAsync_ReturnsCorrectAggregatedPerformanceChart_WhenPortfolioExists()
    {
        var aggregatedPerformanceChart = _fixture.CreateMany<EntityChartPointDto>();

        _chartDataGenerator
            .Setup(c => c.ChartAggregatedPerformance(It.IsAny<PortfolioPositionsPriceListData>(),
                It.IsAny<DateRangeParams>(), It.IsAny<AggregationFrequency>()))
            .Returns(aggregatedPerformanceChart);

        var sut = _fixture.Create<PortfolioService>();
        var result = await sut.ChartPortfolioAggregatedPerformanceAsync(_fixture.Create<int>(),
            _fixture.Create<DateRangeParams>(), _fixture.Create<AggregationFrequency>());

        Assert.Equal(OperationStatus.Ok, result.Status);
        Assert.Equal(aggregatedPerformanceChart, result.Response);
    }

    [Fact]
    public async Task ChartPortfolioAggregatedPerformanceAsync_ReturnsNotFound_WhenPortfolioDoesNotExist()
    {
        _portfolioQueries
            .Setup(m => m.GetPortfolioAsync(It.IsAny<int>()))
            .ReturnsAsync((PortfolioDto)null);

        var sut = _fixture.Create<PortfolioService>();
        var result = await sut.ChartPortfolioAggregatedPerformanceAsync(_fixture.Create<int>(),
            _fixture.Create<DateRangeParams>(), _fixture.Create<AggregationFrequency>());

        Assert.Equal(OperationStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task GetAllPortfoliosStatisticsAsync_ReturnsAllPortfoliosStatistics()
    {
        var portfolios = _fixture.CreateMany<PortfolioDto>(2);
        var statisticsDto = _fixture.Create<EntityStatisticsDto>();

        _portfolioQueries
            .Setup(m => m.GetAllPortfoliosAsync())
            .ReturnsAsync(portfolios);

        _statisticsCalculator
            .Setup(m => m.CalculateStatistics(It.IsAny<PortfolioPositionsPriceListData>(), It.IsAny<DateTime>()))
            .Returns(statisticsDto);

        var sut = _fixture.Create<PortfolioService>();
        var result = await sut.GetAllPortfoliosStatisticsAsync();

        Assert.Equal(OperationStatus.Ok, result.Status);
        Assert.Collection(result.Response, s => Assert.Equal(statisticsDto, s), s => Assert.Equal(statisticsDto, s));
    }

    [Fact]
    public async Task GetPortfolioStatisticsDto_ReturnsPortfolioStatistics_WhenPortfolioExists()
    {
        var portfolio = _fixture.Create<PortfolioDto>();
        var statisticsDto = _fixture.Create<EntityStatisticsDto>();

        _portfolioQueries
            .Setup(m => m.GetPortfolioAsync(It.IsAny<int>()))
            .ReturnsAsync(portfolio);

        _statisticsCalculator
            .Setup(m => m.CalculateStatistics(It.IsAny<PortfolioPositionsPriceListData>(), It.IsAny<DateTime>()))
            .Returns(statisticsDto);

        var sut = _fixture.Create<PortfolioService>();
        var result = await sut.GetPortfolioStatisticsAsync(_fixture.Create<int>());

        Assert.Equal(OperationStatus.Ok, result.Status);
        Assert.Equal(statisticsDto, result.Response);
    }

    [Fact]
    public async Task CreatingPortfolio_AddsPortfolioToRepository_WhenWellFormed()
    {
        var portfolioDto = _fixture.Create<PortfolioDto>();

        var sut = _fixture.Create<PortfolioService>();

        await sut.CreatePortfolioAsync(portfolioDto);

        _portfolioRepository.Verify(r => r.Add(It.Is<Portfolio>(p =>
            p.Name == portfolioDto.Name &&
            p.Note == portfolioDto.Note &&
            p.CurrencyCode == portfolioDto.CurrencyCode
        )), Times.Once());
    }

    [Fact]
    public async Task CreatingPortfolio_ReturnsError_WhenCurrencyDoesNotExist()
    {
        var portfolioDto = _fixture.Create<PortfolioDto>();

        _currencyRepository
            .Setup(r => r.ExistsAsync(portfolioDto.CurrencyCode))
            .Returns(Task.FromResult(false));

        var sut = _fixture.Create<PortfolioService>();
        var response = await sut.CreatePortfolioAsync(portfolioDto);

        Assert.Equal(OperationStatus.Error, response.Status);
    }

    [Fact]
    public async Task UpdatingPortfolio_UpdatesPortfolio_WhenWellFormed()
    {
        var portfolioDto = _fixture.Create<PortfolioDto>();

        var sut = _fixture.Create<PortfolioService>();

        await sut.UpdatePortfolioAsync(portfolioDto);

        _portfolioRepository.Verify(r => r.Update(It.Is<Portfolio>(p =>
            p.Name == portfolioDto.Name &&
            p.Note == portfolioDto.Note &&
            p.CurrencyCode == portfolioDto.CurrencyCode
        )), Times.Once());
    }

    [Fact]
    public async Task UpdatingPortfolio_ReturnsError_WhenCurrencyDoesNotExist()
    {
        var portfolioDto = _fixture.Create<PortfolioDto>();

        _currencyRepository
            .Setup(r => r.ExistsAsync(portfolioDto.CurrencyCode))
            .Returns(Task.FromResult(false));

        var sut = _fixture.Create<PortfolioService>();
        var result = await sut.UpdatePortfolioAsync(portfolioDto);

        Assert.Equal(OperationStatus.Error, result.Status);
    }

    [Fact]
    public async Task UpdatingPortfolio_ReturnsNotFound_WhenPortfolioDoesNotExist()
    {
        var portfolioDto = _fixture.Create<PortfolioDto>();

        _portfolioRepository
            .Setup(r => r.ExistsAsync(portfolioDto.Id))
            .Returns(Task.FromResult(false));
        _portfolioRepository
            .Setup(r => r.FindAsync(portfolioDto.Id))
            .Returns(Task.FromResult<Portfolio>(null));

        var sut = _fixture.Create<PortfolioService>();
        var result = await sut.UpdatePortfolioAsync(portfolioDto);

        Assert.Equal(OperationStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task DeletingPortfolio_DeletesPortfolio_WhenPortfolioExists()
    {
        var portfolioId = _fixture.Create<int>();

        var sut = _fixture.Create<PortfolioService>();

        await sut.DeletePortfolioAsync(portfolioId);

        _portfolioRepository.Verify(r => r.DeleteAsync(portfolioId), Times.Once());
    }

    [Fact]
    public async Task DeletingPortfolio_ThrowsException_WhenPortfolioDoesNotExist()
    {
        var portfolioId = _fixture.Create<int>();

        _portfolioRepository
            .Setup(r => r.ExistsAsync(portfolioId))
            .Returns(Task.FromResult(false));

        var sut = _fixture.Create<PortfolioService>();
        var result = await sut.DeletePortfolioAsync(portfolioId);

        Assert.Equal(OperationStatus.NotFound, result.Status);
    }
}