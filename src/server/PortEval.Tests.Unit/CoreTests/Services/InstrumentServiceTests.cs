using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Core;
using PortEval.Application.Core.Interfaces.Calculators;
using PortEval.Application.Core.Interfaces.ChartDataGenerators;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Core.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.Services
{
    public class InstrumentServiceTests
    {
        [Fact]
        public async Task GetAllInstrumentsAsync_ReturnsAllInstruments()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instruments = fixture.CreateMany<InstrumentDto>();

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.GetAllInstrumentsAsync())
                .ReturnsAsync(instruments);

            var sut = fixture.Create<InstrumentService>();

            var result = await sut.GetAllInstrumentsAsync();

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(instruments, result.Response);
        }

        [Fact]
        public async Task GetInstrumentsPageAsync_ReturnsInstrumentsPage()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var totalCount = 30;
            var takeCount = 15;
            var instruments = fixture.CreateMany<InstrumentDto>(totalCount);
            var instrumentsToTake = instruments.Skip(takeCount).Take(takeCount);
            
            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.GetInstrumentPageAsync(It.IsAny<PaginationParams>()))
                .ReturnsAsync(instrumentsToTake);

            var sut = fixture.Create<InstrumentService>();

            var result = await sut.GetInstrumentsPageAsync(fixture.Build<PaginationParams>().With(p => p.Page, 2).With(p => p.Limit, takeCount).Create());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(instrumentsToTake, result.Response.Data);
        }

        [Fact]
        public async Task GetInstrumentAsync_ReturnsInstrument_WhenInstrumentExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrument = fixture.Create<InstrumentDto>();

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.GetInstrumentAsync(It.IsAny<int>()))
                .ReturnsAsync(instrument);

            var sut = fixture.Create<InstrumentService>();
            var result = await sut.GetInstrumentAsync(instrument.Id);

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(instrument, result.Response);
        }

        [Fact]
        public async Task GetInstrumentAsync_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.GetInstrumentAsync(It.IsAny<int>()))
                .ReturnsAsync((InstrumentDto)null);

            var sut = fixture.Create<InstrumentService>();
            var result = await sut.GetInstrumentAsync(fixture.Create<int>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetKnownExchangesAsync_ReturnsKnownExchanges()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var exchanges = fixture.CreateMany<ExchangeDto>();

            var exchangeQueries = fixture.Freeze<Mock<IExchangeQueries>>();
            exchangeQueries
                .Setup(m => m.GetKnownExchangesAsync())
                .ReturnsAsync(exchanges);

            var sut = fixture.Create<InstrumentService>();
            var result = await sut.GetKnownExchangesAsync();

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(exchanges, result.Response);
        }

        [Fact]
        public async Task GetInstrumentProfitAsync_ReturnsCorrectProfit_WhenInstrumentExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var profit = fixture.Create<decimal>();
            var dateRange = fixture.Create<DateRangeParams>();

            var profitCalculator = fixture.Freeze<Mock<IInstrumentProfitCalculator>>();
            profitCalculator               
                .Setup(m => m.CalculateProfit(It.IsAny<decimal>(), It.IsAny<decimal>()))
                .Returns(profit);

            var sut = fixture.Create<InstrumentService>();
            var profitResult = await sut.GetInstrumentProfitAsync(fixture.Create<int>(), dateRange);

            Assert.Equal(OperationStatus.Ok, profitResult.Status);
            Assert.Equal(dateRange.From, profitResult.Response.From);
            Assert.Equal(dateRange.To, profitResult.Response.To);
            Assert.Equal(profit, profitResult.Response.Profit);
        }

        [Fact]
        public async Task GetInstrumentProfitAsync_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.GetInstrumentAsync(It.IsAny<int>()))
                .ReturnsAsync((InstrumentDto)null);

            var sut = fixture.Create<InstrumentService>();
            var profitResult = await sut.GetInstrumentProfitAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>());

            Assert.Equal(OperationStatus.NotFound, profitResult.Status);
        }

        [Fact]
        public async Task GetInstrumentPerformanceAsync_ReturnsCorrectPerformance_WhenInstrumentExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var performance = fixture.Create<decimal>();
            var dateRange = fixture.Create<DateRangeParams>();

            var performanceCalculator = fixture.Freeze<Mock<IInstrumentPerformanceCalculator>>();
            performanceCalculator
                .Setup(m => m.CalculatePerformance(It.IsAny<decimal>(), It.IsAny<decimal>()))
                .Returns(performance);

            var sut = fixture.Create<InstrumentService>();
            var performanceResult = await sut.GetInstrumentPerformanceAsync(fixture.Create<int>(), dateRange);

            Assert.Equal(OperationStatus.Ok, performanceResult.Status);
            Assert.Equal(dateRange.From, performanceResult.Response.From);
            Assert.Equal(dateRange.To, performanceResult.Response.To);
            Assert.Equal(performance, performanceResult.Response.Performance);
        }

        [Fact]
        public async Task GetInstrumentPerformanceAsync_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.GetInstrumentAsync(It.IsAny<int>()))
                .ReturnsAsync((InstrumentDto)null);

            var sut = fixture.Create<InstrumentService>();
            var performanceResult = await sut.GetInstrumentPerformanceAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>());

            Assert.Equal(OperationStatus.NotFound, performanceResult.Status);
        }

        [Fact]
        public async Task ChartInstrumentPricesAsync_ReturnsCorrectPriceChart_WhenInstrumentExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chartData = fixture.CreateMany<EntityChartPointDto>();

            var chartGenerator = fixture.Freeze<Mock<IInstrumentChartDataGenerator>>();
            chartGenerator
                .Setup(m => m.ChartPrices(It.IsAny<IEnumerable<InstrumentPriceDto>>(), It.IsAny<DateRangeParams>(), It.IsAny<AggregationFrequency>()))
                .Returns(chartData);

            var sut = fixture.Create<InstrumentService>();
            var chartResult = await sut.ChartInstrumentPricesAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>(), fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.Ok, chartResult.Status);
            Assert.Equal(chartData, chartResult.Response);
        }

        [Fact]
        public async Task ChartInstrumentPricesAsync_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.GetInstrumentAsync(It.IsAny<int>()))
                .ReturnsAsync((InstrumentDto)null);

            var sut = fixture.Create<InstrumentService>();
            var chartResult = await sut.ChartInstrumentPricesAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>(), fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.NotFound, chartResult.Status);
        }

        [Fact]
        public async Task ChartInstrumentProfitAsync_ReturnsCorrectProfitChart_WhenInstrumentExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chartData = fixture.CreateMany<EntityChartPointDto>();

            var chartGenerator = fixture.Freeze<Mock<IInstrumentChartDataGenerator>>();
            chartGenerator
                .Setup(m => m.ChartProfit(It.IsAny<IEnumerable<InstrumentPriceDto>>(), It.IsAny<DateRangeParams>(), It.IsAny<AggregationFrequency>()))
                .Returns(chartData);

            var sut = fixture.Create<InstrumentService>();
            var chartResult = await sut.ChartInstrumentProfitAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>(), fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.Ok, chartResult.Status);
            Assert.Equal(chartData, chartResult.Response);
        }

        [Fact]
        public async Task ChartInstrumentProfitAsync_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.GetInstrumentAsync(It.IsAny<int>()))
                .ReturnsAsync((InstrumentDto)null);

            var sut = fixture.Create<InstrumentService>();
            var chartResult = await sut.ChartInstrumentProfitAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>(), fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.NotFound, chartResult.Status);
        }

        [Fact]
        public async Task ChartInstrumentPerformanceAsync_ReturnsCorrectPerformanceChart_WhenInstrumentExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chartData = fixture.CreateMany<EntityChartPointDto>();

            var chartGenerator = fixture.Freeze<Mock<IInstrumentChartDataGenerator>>();
            chartGenerator
                .Setup(m => m.ChartPerformance(It.IsAny<IEnumerable<InstrumentPriceDto>>(), It.IsAny<DateRangeParams>(), It.IsAny<AggregationFrequency>()))
                .Returns(chartData);

            var sut = fixture.Create<InstrumentService>();
            var chartResult = await sut.ChartInstrumentPerformanceAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>(), fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.Ok, chartResult.Status);
            Assert.Equal(chartData, chartResult.Response);
        }

        [Fact]
        public async Task ChartInstrumentPerformanceAsync_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.GetInstrumentAsync(It.IsAny<int>()))
                .ReturnsAsync((InstrumentDto)null);

            var sut = fixture.Create<InstrumentService>();
            var chartResult = await sut.ChartInstrumentPerformanceAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>(), fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.NotFound, chartResult.Status);
        }

        [Fact]
        public async Task ChartInstrumentAggregatedProfitAsync_ReturnsCorrectAggregatedProfitChart_WhenInstrumentExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chartData = fixture.CreateMany<EntityChartPointDto>();

            var chartGenerator = fixture.Freeze<Mock<IInstrumentChartDataGenerator>>();
            chartGenerator
                .Setup(m => m.ChartAggregatedProfit(It.IsAny<IEnumerable<InstrumentPriceDto>>(), It.IsAny<DateRangeParams>(), It.IsAny<AggregationFrequency>()))
                .Returns(chartData);

            var sut = fixture.Create<InstrumentService>();
            var chartResult = await sut.ChartInstrumentAggregatedProfitAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>(), fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.Ok, chartResult.Status);
            Assert.Equal(chartData, chartResult.Response);
        }

        [Fact]
        public async Task ChartInstrumentAggregatedProfitAsync_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.GetInstrumentAsync(It.IsAny<int>()))
                .ReturnsAsync((InstrumentDto)null);

            var sut = fixture.Create<InstrumentService>();
            var chartResult = await sut.ChartInstrumentAggregatedProfitAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>(), fixture.Create<AggregationFrequency>());

            Assert.Equal(OperationStatus.NotFound, chartResult.Status);
        }

        [Fact]
        public async Task ChartInstrumentAggregatedPerformanceAsync_ReturnsCorrectAggregatedPerformanceChart_WhenInstrumentExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chartData = fixture.CreateMany<EntityChartPointDto>();

            var chartGenerator = fixture.Freeze<Mock<IInstrumentChartDataGenerator>>();
            chartGenerator
                .Setup(m => m.ChartAggregatedPerformance(It.IsAny<IEnumerable<InstrumentPriceDto>>(), It.IsAny<DateRangeParams>(), It.IsAny<AggregationFrequency>()))
                .Returns(chartData);

            var sut = fixture.Create<InstrumentService>();
            var chartResult = await sut.ChartInstrumentAggregatedPerformanceAsync(fixture.Create<int>(), fixture.Create<DateRangeParams>(), fixture.Create<AggregationFrequency>());
            
            Assert.Equal(OperationStatus.Ok, chartResult.Status);
            Assert.Equal(chartData, chartResult.Response);
        }

        [Fact]
        public async Task CreatingInstrument_AddsInstrumentToInstrumentRepository_WhenWellFormed()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrument = fixture.Create<InstrumentDto>();

            var instrumentRepository = fixture.Freeze<Mock<IInstrumentRepository>>();
            fixture.Freeze<Mock<ICurrencyRepository>>();
            fixture.Freeze<Mock<IExchangeRepository>>();

            var sut = fixture.Create<InstrumentService>();

            await sut.CreateInstrumentAsync(instrument);

            instrumentRepository.Verify(
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
        public async Task CreatingInstrument_ReturnsCreatedInstrument_WhenSuccessfullyCreated()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentDto = fixture.Create<InstrumentDto>();

            fixture.Freeze<Mock<IInstrumentRepository>>();
            fixture.Freeze<Mock<ICurrencyRepository>>();
            fixture.Freeze<Mock<IExchangeRepository>>();

            var sut = fixture.Create<InstrumentService>();

            var createdInstrument = await sut.CreateInstrumentAsync(instrumentDto);

            Assert.Equal(OperationStatus.Ok, createdInstrument.Status);
            Assert.Equal(createdInstrument.Response.Name, instrumentDto.Name);
            Assert.Equal(createdInstrument.Response.Symbol, instrumentDto.Symbol);
            Assert.Equal(createdInstrument.Response.Note, instrumentDto.Note);
            Assert.Equal(createdInstrument.Response.Exchange, instrumentDto.Exchange);
            Assert.Equal(createdInstrument.Response.CurrencyCode, instrumentDto.CurrencyCode);
        }

        [Fact]
        public async Task CreatingInstrument_CreatesItsExchange_WhenExchangeDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrument = fixture.Create<InstrumentDto>();

            fixture.Freeze<Mock<IInstrumentRepository>>();
            fixture.Freeze<Mock<ICurrencyRepository>>();

            var exchangeRepository = fixture.Freeze<Mock<IExchangeRepository>>();
            exchangeRepository
                .Setup(e => e.ExistsAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(false));

            var sut = fixture.Create<InstrumentService>();
            await sut.CreateInstrumentAsync(instrument);

            exchangeRepository.Verify(
                r => r.Add(It.Is<Exchange>(e => e.Symbol == instrument.Exchange)),
                Times.Once()
            );
        }

        [Fact]
        public async Task CreatingInstrument_DoesNotCreateExchange_WhenExchangeExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrument = fixture.Create<InstrumentDto>();

            fixture.Freeze<Mock<IInstrumentRepository>>();
            fixture.Freeze<Mock<ICurrencyRepository>>();
            var exchangeRepository = fixture.Freeze<Mock<IExchangeRepository>>();

            var sut = fixture.Create<InstrumentService>();
            await sut.CreateInstrumentAsync(instrument);

            exchangeRepository.Verify(
                r => r.Add(It.IsAny<Exchange>()),
                Times.Never()
            );
        }

        [Fact]
        public async Task CreatingInstrument_ThrowsException_WhenCurrencyDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrument = fixture.Create<InstrumentDto>();

            var currencyRepository = fixture.Freeze<Mock<ICurrencyRepository>>();
            currencyRepository
                .Setup(c => c.ExistsAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(false));

            var sut = fixture.Create<InstrumentService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await sut.CreateInstrumentAsync(instrument));
        }

        [Fact]
        public async Task UpdatingInstrument_UpdatesInstrumentInRepository_WhenInstrumentExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrument = fixture.Create<Instrument>();
            var updatedInstrumentDto = fixture.Create<InstrumentDto>();
            updatedInstrumentDto.Id = instrument.Id;

            var instrumentRepository = fixture.Freeze<Mock<IInstrumentRepository>>();
            instrumentRepository
                .Setup(i => i.FindAsync(updatedInstrumentDto.Id))
                .Returns(Task.FromResult(instrument));

            fixture.Freeze<Mock<IExchangeRepository>>();

            var sut = fixture.Create<InstrumentService>();
            await sut.UpdateInstrumentAsync(updatedInstrumentDto);

            instrumentRepository.Verify(r => r.Update(
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
        public async Task UpdatingInstrument_ThrowsException_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var updatedInstrumentDto = fixture.Create<InstrumentDto>();

            var instrumentRepository = fixture.Freeze<Mock<IInstrumentRepository>>();
            instrumentRepository
                .Setup(i => i.FindAsync(updatedInstrumentDto.Id))
                .Returns(Task.FromResult<Instrument>(null));
            instrumentRepository
                .Setup(i => i.ExistsAsync(updatedInstrumentDto.Id))
                .Returns(Task.FromResult(false));

            var sut = fixture.Create<InstrumentService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () =>
                await sut.UpdateInstrumentAsync(updatedInstrumentDto));
        }

        [Fact]
        public async Task UpdatingInstrument_ReturnsUpdatedInstrument_WhenUpdatedSuccessfully()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrument = fixture.Create<Instrument>();
            var updatedInstrumentDto = fixture.Create<InstrumentDto>();
            updatedInstrumentDto.Id = instrument.Id;

            var instrumentRepository = fixture.Freeze<Mock<IInstrumentRepository>>();
            instrumentRepository
                .Setup(i => i.FindAsync(updatedInstrumentDto.Id))
                .Returns(Task.FromResult(instrument));

            fixture.Freeze<Mock<IExchangeRepository>>();

            var sut = fixture.Create<InstrumentService>();
            var updatedInstrument = await sut.UpdateInstrumentAsync(updatedInstrumentDto);

            Assert.Equal(OperationStatus.Ok, updatedInstrument.Status);
            Assert.Equal(updatedInstrumentDto.Id, updatedInstrument.Response.Id);
            Assert.Equal(updatedInstrumentDto.Name, updatedInstrument.Response.Name);
            Assert.Equal(updatedInstrumentDto.Exchange, updatedInstrument.Response.Exchange);
            Assert.Equal(updatedInstrumentDto.Note, updatedInstrument.Response.Note);
        }

        [Fact]
        public async Task UpdatingInstrument_AddsExchangeToRepository_WhenExchangeDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrument = fixture.Create<Instrument>();
            var updatedInstrumentDto = fixture.Create<InstrumentDto>();
            updatedInstrumentDto.Id = instrument.Id;

            var instrumentRepository = fixture.Freeze<Mock<IInstrumentRepository>>();
            instrumentRepository
                .Setup(i => i.FindAsync(updatedInstrumentDto.Id))
                .Returns(Task.FromResult(instrument));

            var exchangeRepository = fixture.Freeze<Mock<IExchangeRepository>>();
            exchangeRepository
                .Setup(e => e.ExistsAsync(updatedInstrumentDto.Exchange))
                .Returns(Task.FromResult(false));

            var sut = fixture.Create<InstrumentService>();
            await sut.UpdateInstrumentAsync(updatedInstrumentDto);

            exchangeRepository.Verify(r => r.Add(It.Is<Exchange>(e => e.Symbol == updatedInstrumentDto.Exchange)),
                Times.Once());
        }

        [Fact]
        public async Task UpdatingInstrument_DoesNotAddExchangeToRepository_WhenExchangeExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrument = fixture.Create<Instrument>();
            var updatedInstrumentDto = fixture.Create<InstrumentDto>();
            updatedInstrumentDto.Id = instrument.Id;

            var instrumentRepository = fixture.Freeze<Mock<IInstrumentRepository>>();
            instrumentRepository
                .Setup(i => i.FindAsync(instrument.Id))
                .Returns(Task.FromResult(instrument));

            var exchangeRepository = fixture.Freeze<Mock<IExchangeRepository>>();
            exchangeRepository
                .Setup(e => e.ExistsAsync(updatedInstrumentDto.Exchange))
                .Returns(Task.FromResult(true));

            var sut = fixture.Create<InstrumentService>();
            await sut.UpdateInstrumentAsync(updatedInstrumentDto);

            exchangeRepository.Verify(r => r.Add(It.IsAny<Exchange>()), Times.Never());
        }

        [Fact]
        public async Task DeletingInstrument_DeletesInstrumentFromRepository_WhenInstrumentExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();

            var instrumentRepository = fixture.Freeze<Mock<IInstrumentRepository>>();
            instrumentRepository
                .Setup(i => i.ExistsAsync(It.Is<int>(id => id == instrumentId)))
                .Returns(Task.FromResult(true));

            fixture.Freeze<Mock<ICurrencyRepository>>();
            fixture.Freeze<Mock<IExchangeRepository>>();

            var sut = fixture.Create<InstrumentService>();

            await sut.DeleteAsync(instrumentId);

            instrumentRepository.Verify(r => r.DeleteAsync(instrumentId), Times.Once());
        }

        [Fact]
        public async Task DeletingInstrument_ThrowsException_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();

            var instrumentRepository = fixture.Freeze<Mock<IInstrumentRepository>>();
            instrumentRepository
                .Setup(i => i.ExistsAsync(instrumentId))
                .Returns(Task.FromResult(false));

            fixture.Freeze<Mock<ICurrencyRepository>>();
            fixture.Freeze<Mock<IExchangeRepository>>();

            var sut = fixture.Create<InstrumentService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await sut.DeleteAsync(instrumentId));
        }
    }
}