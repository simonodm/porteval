using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Core;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.Services
{
    public class InstrumentServiceTests
    {
        private IFixture _fixture;
        private Mock<IInstrumentQueries> _instrumentQueries;
        private Mock<IInstrumentRepository> _instrumentRepository;
        private Mock<IExchangeQueries> _exchangeQueries;
        private Mock<IExchangeRepository> _exchangeRepository;
        private Mock<ICurrencyRepository> _currencyRepository;
        private Mock<IInstrumentPriceService> _priceService;
        private Mock<ICurrencyExchangeRateService> _exchangeRatesService;
        private Mock<IInstrumentProfitCalculator> _profitCalculator;
        private Mock<IInstrumentPerformanceCalculator> _performanceCalculator;
        private Mock<IInstrumentChartDataGenerator> _chartDataGenerator;
        private Mock<ICurrencyConverter> _currencyConverter;

        public InstrumentServiceTests()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            _instrumentQueries = _fixture.CreateDefaultInstrumentQueriesMock();
            _instrumentRepository = _fixture.CreateDefaultInstrumentRepositoryMock();
            _exchangeQueries = _fixture.CreateDefaultExchangeQueriesMock();
            _exchangeRepository = _fixture.CreateDefaultExchangeRepositoryMock();
            _currencyRepository = _fixture.CreateDefaultCurrencyRepositoryMock();

            _priceService = _fixture.Freeze<Mock<IInstrumentPriceService>>();
            _priceService
                .Setup(m => m.GetInstrumentPriceAsync(It.IsAny<int>(), It.IsAny<DateTime>()))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(_fixture.Create<InstrumentPriceDto>()));

            _exchangeRatesService = _fixture.Freeze<Mock<ICurrencyExchangeRateService>>();
            _exchangeRatesService
                .Setup(m => m.GetExchangeRatesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateRangeParams>()))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(_fixture.CreateMany<CurrencyExchangeRateDto>()));

            _profitCalculator = _fixture.Freeze<Mock<IInstrumentProfitCalculator>>();
            _performanceCalculator = _fixture.Freeze<Mock<IInstrumentPerformanceCalculator>>();
            _chartDataGenerator = _fixture.Freeze<Mock<IInstrumentChartDataGenerator>>();

            _currencyConverter = _fixture.Freeze<Mock<ICurrencyConverter>>();
            _currencyConverter
                .Setup(m => m.ConvertChartPoints(It.IsAny<IEnumerable<EntityChartPointDto>>(),
                    It.IsAny<IEnumerable<CurrencyExchangeRateDto>>()))
                .Returns<IEnumerable<EntityChartPointDto>, IEnumerable<CurrencyExchangeRateDto>>((points, rates) =>
                    points);

        }

        [Fact]
        public async Task GetAllInstrumentsAsync_ReturnsAllInstruments()
        {
            var instruments = _fixture.CreateMany<InstrumentDto>();

            _instrumentQueries
                .Setup(m => m.GetAllInstrumentsAsync())
                .ReturnsAsync(instruments);

            var sut = _fixture.Create<InstrumentService>();

            var result = await sut.GetAllInstrumentsAsync();

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(instruments, result.Response);
        }

        [Fact]
        public async Task GetInstrumentsPageAsync_ReturnsInstrumentsPage()
        {
            var totalCount = 30;
            var takeCount = 15;
            var instruments = _fixture.CreateMany<InstrumentDto>(totalCount);
            var instrumentsToTake = instruments.Skip(takeCount).Take(takeCount);

            _instrumentQueries
                .Setup(m => m.GetInstrumentPageAsync(It.IsAny<PaginationParams>()))
                .ReturnsAsync(instrumentsToTake);

            var sut = _fixture.Create<InstrumentService>();

            var result = await sut.GetInstrumentsPageAsync(_fixture.Build<PaginationParams>().With(p => p.Page, 2).With(p => p.Limit, takeCount).Create());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(instrumentsToTake, result.Response.Data);
        }

        [Fact]
        public async Task GetInstrumentAsync_ReturnsInstrument_WhenInstrumentExists()
        {
            var instrument = _fixture.Create<InstrumentDto>();

            _instrumentQueries
                .Setup(m => m.GetInstrumentAsync(It.IsAny<int>()))
                .ReturnsAsync(instrument);

            var sut = _fixture.Create<InstrumentService>();
            var result = await sut.GetInstrumentAsync(instrument.Id);

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(instrument, result.Response);
        }

        [Fact]
        public async Task GetInstrumentAsync_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            _instrumentQueries
                .Setup(m => m.GetInstrumentAsync(It.IsAny<int>()))
                .ReturnsAsync((InstrumentDto)null);

            var sut = _fixture.Create<InstrumentService>();
            var result = await sut.GetInstrumentAsync(_fixture.Create<int>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetKnownExchangesAsync_ReturnsKnownExchanges()
        {
            var exchanges = _fixture.CreateMany<ExchangeDto>();

            _exchangeQueries
                .Setup(m => m.GetKnownExchangesAsync())
                .ReturnsAsync(exchanges);

            var sut = _fixture.Create<InstrumentService>();
            var result = await sut.GetKnownExchangesAsync();

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(exchanges, result.Response);
        }

        [Fact]
        public async Task GetInstrumentProfitAsync_ReturnsCorrectProfit_WhenInstrumentExists()
        {
            var profit = _fixture.Create<decimal>();
            var dateRange = _fixture.Create<DateRangeParams>();

            _profitCalculator
                .Setup(m => m.CalculateProfit(It.IsAny<decimal>(), It.IsAny<decimal>()))
                .Returns(profit);

            var sut = _fixture.Create<InstrumentService>();
            var profitResult = await sut.GetInstrumentProfitAsync(_fixture.Create<int>(), dateRange);

            Assert.Equal(OperationStatus.Ok, profitResult.Status);
            Assert.Equal(dateRange.From, profitResult.Response.From);
            Assert.Equal(dateRange.To, profitResult.Response.To);
            Assert.Equal(profit, profitResult.Response.Profit);
        }

        [Fact]
        public async Task GetInstrumentProfitAsync_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            _instrumentQueries
                .Setup(m => m.GetInstrumentAsync(It.IsAny<int>()))
                .ReturnsAsync((InstrumentDto)null);

            var sut = _fixture.Create<InstrumentService>();
            var profitResult = await sut.GetInstrumentProfitAsync(_fixture.Create<int>(), _fixture.Create<DateRangeParams>());

            Assert.Equal(OperationStatus.NotFound, profitResult.Status);
        }

        [Fact]
        public async Task GetInstrumentPerformanceAsync_ReturnsCorrectPerformance_WhenInstrumentExists()
        {
            var performance = _fixture.Create<decimal>();
            var dateRange = _fixture.Create<DateRangeParams>();

            _performanceCalculator
                .Setup(m => m.CalculatePerformance(It.IsAny<decimal>(), It.IsAny<decimal>()))
                .Returns(performance);

            var sut = _fixture.Create<InstrumentService>();
            var performanceResult = await sut.GetInstrumentPerformanceAsync(_fixture.Create<int>(), dateRange);

            Assert.Equal(OperationStatus.Ok, performanceResult.Status);
            Assert.Equal(dateRange.From, performanceResult.Response.From);
            Assert.Equal(dateRange.To, performanceResult.Response.To);
            Assert.Equal(performance, performanceResult.Response.Performance);
        }

        [Fact]
        public async Task GetInstrumentPerformanceAsync_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            _instrumentQueries
                .Setup(m => m.GetInstrumentAsync(It.IsAny<int>()))
                .ReturnsAsync((InstrumentDto)null);

            var sut = _fixture.Create<InstrumentService>();
            var performanceResult = await sut.GetInstrumentPerformanceAsync(_fixture.Create<int>(), _fixture.Create<DateRangeParams>());

            Assert.Equal(OperationStatus.NotFound, performanceResult.Status);
        }

        [Fact]
        public async Task ChartInstrumentPricesAsync_ReturnsCorrectPriceChart_WhenInstrumentExists()
        {
            var chartData = _fixture.CreateMany<EntityChartPointDto>();

            _chartDataGenerator
                .Setup(m => m.ChartPrices(It.IsAny<IEnumerable<InstrumentPriceDto>>(), It.IsAny<DateRangeParams>(), It.IsAny<AggregationFrequency>()))
                .Returns(chartData);

            var sut = _fixture.Create<InstrumentService>();
            var chartResult = await sut.ChartInstrumentPricesAsync(_fixture.Create<int>(), _fixture.Create<DateRangeParams>(), _fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.Ok, chartResult.Status);
            Assert.Equal(chartData, chartResult.Response);
        }

        [Fact]
        public async Task ChartInstrumentPricesAsync_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            _instrumentQueries
                .Setup(m => m.GetInstrumentAsync(It.IsAny<int>()))
                .ReturnsAsync((InstrumentDto)null);

            var sut = _fixture.Create<InstrumentService>();
            var chartResult = await sut.ChartInstrumentPricesAsync(_fixture.Create<int>(), _fixture.Create<DateRangeParams>(), _fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.NotFound, chartResult.Status);
        }

        [Fact]
        public async Task ChartInstrumentProfitAsync_ReturnsCorrectProfitChart_WhenInstrumentExists()
        {
            var chartData = _fixture.CreateMany<EntityChartPointDto>();

            _chartDataGenerator
                .Setup(m => m.ChartProfit(It.IsAny<IEnumerable<InstrumentPriceDto>>(), It.IsAny<DateRangeParams>(), It.IsAny<AggregationFrequency>()))
                .Returns(chartData);

            var sut = _fixture.Create<InstrumentService>();
            var chartResult = await sut.ChartInstrumentProfitAsync(_fixture.Create<int>(), _fixture.Create<DateRangeParams>(), _fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.Ok, chartResult.Status);
            Assert.Equal(chartData, chartResult.Response);
        }

        [Fact]
        public async Task ChartInstrumentProfitAsync_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            _instrumentQueries
                .Setup(m => m.GetInstrumentAsync(It.IsAny<int>()))
                .ReturnsAsync((InstrumentDto)null);

            var sut = _fixture.Create<InstrumentService>();
            var chartResult = await sut.ChartInstrumentProfitAsync(_fixture.Create<int>(), _fixture.Create<DateRangeParams>(), _fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.NotFound, chartResult.Status);
        }

        [Fact]
        public async Task ChartInstrumentPerformanceAsync_ReturnsCorrectPerformanceChart_WhenInstrumentExists()
        {
            var chartData = _fixture.CreateMany<EntityChartPointDto>();

            _chartDataGenerator
                .Setup(m => m.ChartPerformance(It.IsAny<IEnumerable<InstrumentPriceDto>>(), It.IsAny<DateRangeParams>(), It.IsAny<AggregationFrequency>()))
                .Returns(chartData);

            var sut = _fixture.Create<InstrumentService>();
            var chartResult = await sut.ChartInstrumentPerformanceAsync(_fixture.Create<int>(), _fixture.Create<DateRangeParams>(), _fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.Ok, chartResult.Status);
            Assert.Equal(chartData, chartResult.Response);
        }

        [Fact]
        public async Task ChartInstrumentPerformanceAsync_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            _instrumentQueries
                .Setup(m => m.GetInstrumentAsync(It.IsAny<int>()))
                .ReturnsAsync((InstrumentDto)null);

            var sut = _fixture.Create<InstrumentService>();
            var chartResult = await sut.ChartInstrumentPerformanceAsync(_fixture.Create<int>(), _fixture.Create<DateRangeParams>(), _fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.NotFound, chartResult.Status);
        }

        [Fact]
        public async Task ChartInstrumentAggregatedProfitAsync_ReturnsCorrectAggregatedProfitChart_WhenInstrumentExists()
        {
            var chartData = _fixture.CreateMany<EntityChartPointDto>();

            _chartDataGenerator
                .Setup(m => m.ChartAggregatedProfit(It.IsAny<IEnumerable<InstrumentPriceDto>>(), It.IsAny<DateRangeParams>(), It.IsAny<AggregationFrequency>()))
                .Returns(chartData);

            var sut = _fixture.Create<InstrumentService>();
            var chartResult = await sut.ChartInstrumentAggregatedProfitAsync(_fixture.Create<int>(), _fixture.Create<DateRangeParams>(), _fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.Ok, chartResult.Status);
            Assert.Equal(chartData, chartResult.Response);
        }

        [Fact]
        public async Task ChartInstrumentAggregatedProfitAsync_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            _instrumentQueries
                .Setup(m => m.GetInstrumentAsync(It.IsAny<int>()))
                .ReturnsAsync((InstrumentDto)null);

            var sut = _fixture.Create<InstrumentService>();
            var chartResult = await sut.ChartInstrumentAggregatedProfitAsync(_fixture.Create<int>(), _fixture.Create<DateRangeParams>(), _fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.NotFound, chartResult.Status);
        }

        [Fact]
        public async Task ChartInstrumentAggregatedPerformanceAsync_ReturnsCorrectAggregatedPerformanceChart_WhenInstrumentExists()
        {
            var chartData = _fixture.CreateMany<EntityChartPointDto>();

            _chartDataGenerator
                .Setup(m => m.ChartAggregatedPerformance(It.IsAny<IEnumerable<InstrumentPriceDto>>(), It.IsAny<DateRangeParams>(), It.IsAny<AggregationFrequency>()))
                .Returns(chartData);

            var sut = _fixture.Create<InstrumentService>();
            var chartResult = await sut.ChartInstrumentAggregatedPerformanceAsync(_fixture.Create<int>(), _fixture.Create<DateRangeParams>(), _fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.Ok, chartResult.Status);
            Assert.Equal(chartData, chartResult.Response);
        }

        [Fact]
        public async Task CreatingInstrument_AddsInstrumentToInstrumentRepository_WhenWellFormed()
        {
            var instrument = _fixture.Create<InstrumentDto>();

            var sut = _fixture.Create<InstrumentService>();

            await sut.CreateInstrumentAsync(instrument);

            _instrumentRepository.Verify(
                r => r.Add(It.Is<Instrument>(i =>
                    i.Name == instrument.Name &&
                    i.Exchange == instrument.Exchange &&
                    i.CurrencyCode == instrument.CurrencyCode &&
                    i.Symbol == instrument.Symbol &&
                    i.Type == instrument.Type &&
                    i.Note == instrument.Note
                )),
                Times.Once());
        }

        [Fact]
        public async Task CreatingInstrument_CreatesItsExchange_WhenExchangeDoesNotExist()
        {
            var instrument = _fixture.Create<InstrumentDto>();

            _exchangeRepository
                .Setup(e => e.ExistsAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(false));

            var sut = _fixture.Create<InstrumentService>();
            await sut.CreateInstrumentAsync(instrument);

            _exchangeRepository.Verify(
                r => r.Add(It.Is<Exchange>(e => e.Symbol == instrument.Exchange)),
                Times.Once()
            );
        }

        [Fact]
        public async Task CreatingInstrument_DoesNotCreateExchange_WhenExchangeExists()
        {
            var instrument = _fixture.Create<InstrumentDto>();

            var sut = _fixture.Create<InstrumentService>();
            await sut.CreateInstrumentAsync(instrument);

            _exchangeRepository.Verify(
                r => r.Add(It.IsAny<Exchange>()),
                Times.Never()
            );
        }

        [Fact]
        public async Task CreatingInstrument_ReturnsError_WhenCurrencyDoesNotExist()
        {
            var instrument = _fixture.Create<InstrumentDto>();

            _currencyRepository
                .Setup(c => c.ExistsAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(false));

            var sut = _fixture.Create<InstrumentService>();

            var response = await sut.CreateInstrumentAsync(instrument);
            Assert.Equal(OperationStatus.Error, response.Status);
        }

        [Fact]
        public async Task UpdatingInstrument_UpdatesInstrumentInRepository_WhenInstrumentExists()
        {
            var instrument = _fixture.Create<Instrument>();
            var updatedInstrumentDto = _fixture.Create<InstrumentDto>();
            updatedInstrumentDto.Id = instrument.Id;

            _instrumentRepository
                .Setup(i => i.FindAsync(updatedInstrumentDto.Id))
                .Returns(Task.FromResult(instrument));

            var sut = _fixture.Create<InstrumentService>();
            await sut.UpdateInstrumentAsync(updatedInstrumentDto);

            _instrumentRepository.Verify(r => r.Update(
                    It.Is<Instrument>(i =>
                        i.Id == updatedInstrumentDto.Id &&
                        i.Name == updatedInstrumentDto.Name &&
                        i.Symbol == instrument.Symbol &&
                        i.Exchange == updatedInstrumentDto.Exchange &&
                        i.Note == updatedInstrumentDto.Note)),
                Times.Once()
            );
        }

        [Fact]
        public async Task UpdatingInstrument_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var updatedInstrumentDto = _fixture.Create<InstrumentDto>();

            _instrumentRepository
                .Setup(i => i.FindAsync(updatedInstrumentDto.Id))
                .Returns(Task.FromResult<Instrument>(null));
            _instrumentRepository
                .Setup(i => i.ExistsAsync(updatedInstrumentDto.Id))
                .Returns(Task.FromResult(false));

            var sut = _fixture.Create<InstrumentService>();
            var response = await sut.UpdateInstrumentAsync(updatedInstrumentDto);
            Assert.Equal(OperationStatus.NotFound, response.Status);
        }

        [Fact]
        public async Task UpdatingInstrument_AddsExchangeToRepository_WhenExchangeDoesNotExist()
        {
            var instrument = _fixture.Create<Instrument>();
            var updatedInstrumentDto = _fixture.Create<InstrumentDto>();
            updatedInstrumentDto.Id = instrument.Id;

            _instrumentRepository
                .Setup(i => i.FindAsync(updatedInstrumentDto.Id))
                .Returns(Task.FromResult(instrument));

            _exchangeRepository
                .Setup(e => e.ExistsAsync(updatedInstrumentDto.Exchange))
                .Returns(Task.FromResult(false));

            var sut = _fixture.Create<InstrumentService>();
            await sut.UpdateInstrumentAsync(updatedInstrumentDto);

            _exchangeRepository.Verify(r => r.Add(It.Is<Exchange>(e => e.Symbol == updatedInstrumentDto.Exchange)),
                Times.Once());
        }

        [Fact]
        public async Task UpdatingInstrument_DoesNotAddExchangeToRepository_WhenExchangeExists()
        {
            var instrument = _fixture.Create<Instrument>();
            var updatedInstrumentDto = _fixture.Create<InstrumentDto>();
            updatedInstrumentDto.Id = instrument.Id;

            _instrumentRepository
                .Setup(i => i.FindAsync(instrument.Id))
                .Returns(Task.FromResult(instrument));

            var sut = _fixture.Create<InstrumentService>();
            await sut.UpdateInstrumentAsync(updatedInstrumentDto);

            _exchangeRepository.Verify(r => r.Add(It.IsAny<Exchange>()), Times.Never());
        }

        [Fact]
        public async Task DeletingInstrument_DeletesInstrumentFromRepository_WhenInstrumentExists()
        {
            var instrumentId = _fixture.Create<int>();

            var sut = _fixture.Create<InstrumentService>();

            await sut.DeleteAsync(instrumentId);

            _instrumentRepository.Verify(r => r.DeleteAsync(instrumentId), Times.Once());
        }

        [Fact]
        public async Task DeletingInstrument_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var instrumentId = _fixture.Create<int>();

            _instrumentRepository
                .Setup(i => i.ExistsAsync(instrumentId))
                .Returns(Task.FromResult(false));

            var sut = _fixture.Create<InstrumentService>();

            var response = await sut.DeleteAsync(instrumentId);
            Assert.Equal(OperationStatus.NotFound, response.Status);
        }
    }
}