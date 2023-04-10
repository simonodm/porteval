using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;
using PortEval.Tests.Unit.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.ControllerTests
{
    public class InstrumentControllerTests
    {
        [Fact]
        public async Task GetAllInstruments_ReturnsInstrumentsPage()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentsPage = fixture.Create<PaginatedResponse<InstrumentDto>>();
            var pagination = fixture.Create<PaginationParams>();

            var instrumentService = fixture.Freeze<Mock<IInstrumentService>>();
            instrumentService
                .Setup(m => m.GetInstrumentsPageAsync(pagination))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(instrumentsPage));
            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetAllInstruments(pagination);

            instrumentService.Verify(m => m.GetInstrumentsPageAsync(pagination), Times.Once());
            Assert.Equal(instrumentsPage, result.Value);
        }

        [Fact]
        public async Task GetInstrument_ReturnsCorrectInstrument_WhenInstrumentExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrument = fixture.Create<InstrumentDto>();

            var instrumentService = fixture.Freeze<Mock<IInstrumentService>>();
            instrumentService
                .Setup(m => m.GetInstrumentAsync(instrument.Id))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(instrument));

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrument(instrument.Id);

            instrumentService.Verify(m => m.GetInstrumentAsync(instrument.Id), Times.Once());
            Assert.Equal(instrument, result.Value);
        }

        [Fact]
        public async Task GetInstrument_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();

            var instrumentService = fixture.Freeze<Mock<IInstrumentService>>();
            instrumentService
                .Setup(m => m.GetInstrumentAsync(It.IsAny<int>()))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<InstrumentDto>());

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrument(instrumentId);

            instrumentService.Verify(m => m.GetInstrumentAsync(instrumentId), Times.Once());
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetInstrumentProfit_ReturnsProfit_WhenInstrumentExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();
            var profit = fixture.Create<EntityProfitDto>();
            var dateRange = fixture.Create<DateRangeParams>();

            var instrumentService = fixture.Freeze<Mock<IInstrumentService>>();
            instrumentService
                .Setup(m => m.GetInstrumentProfitAsync(instrumentId, dateRange))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(profit));

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentProfit(instrumentId, dateRange);

            instrumentService.Verify(m => m.GetInstrumentProfitAsync(instrumentId, dateRange));
            Assert.Equal(profit, result.Value);
        }

        [Fact]
        public async Task GetInstrumentProfit_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();
            var dateRange = fixture.Create<DateRangeParams>();

            var instrumentService = fixture.Freeze<Mock<IInstrumentService>>();
            instrumentService
                .Setup(m => m.GetInstrumentProfitAsync(instrumentId, dateRange))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<EntityProfitDto>());

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentProfit(instrumentId, dateRange);

            instrumentService.Verify(m => m.GetInstrumentProfitAsync(instrumentId, dateRange));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetInstrumentPerformance_ReturnsInstrumentPerformance_WhenInstrumentExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();
            var performance = fixture.Create<EntityPerformanceDto>();
            var dateRange = fixture.Create<DateRangeParams>();

            var instrumentService = fixture.Freeze<Mock<IInstrumentService>>();
            instrumentService
                .Setup(m => m.GetInstrumentPerformanceAsync(instrumentId, dateRange))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(performance));

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentPerformance(instrumentId, dateRange);

            instrumentService.Verify(m => m.GetInstrumentPerformanceAsync(instrumentId, dateRange));
            Assert.Equal(performance, result.Value);
        }

        [Fact]
        public async Task GetInstrumentPerformance_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();
            var dateRange = fixture.Create<DateRangeParams>();

            var instrumentService = fixture.Freeze<Mock<IInstrumentService>>();
            instrumentService
                .Setup(m => m.GetInstrumentPerformanceAsync(instrumentId, dateRange))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<EntityPerformanceDto>());

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentPerformance(instrumentId, dateRange);

            instrumentService.Verify(m => m.GetInstrumentPerformanceAsync(instrumentId, dateRange));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetInstrumentChartedPrices_ReturnsChartedPrices_WhenInstrumentExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();
            var chartedPrices = fixture.CreateMany<EntityChartPointDto>();
            var dateRange = fixture.Create<DateRangeParams>();
            var aggregationFrequency = fixture.Create<AggregationFrequency>();
            var currencyCode = fixture.Create<string>();

            var instrumentService = fixture.Freeze<Mock<IInstrumentService>>();
            instrumentService
                .Setup(m => m.ChartInstrumentPricesAsync(instrumentId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(chartedPrices));

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentChartedPrices(instrumentId, dateRange, aggregationFrequency, currencyCode);

            instrumentService.Verify(m => m.ChartInstrumentPricesAsync(instrumentId, dateRange, aggregationFrequency, currencyCode));
            Assert.Equal(chartedPrices, result.Value);
        }

        [Fact]
        public async Task GetInstrumentChartedPrices_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();
            var dateRange = fixture.Create<DateRangeParams>();
            var aggregationFrequency = fixture.Create<AggregationFrequency>();
            var currencyCode = fixture.Create<string>();

            var instrumentService = fixture.Freeze<Mock<IInstrumentService>>();
            instrumentService
                .Setup(m => m.ChartInstrumentPricesAsync(instrumentId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<IEnumerable<EntityChartPointDto>>());

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentChartedPrices(instrumentId, dateRange, aggregationFrequency, currencyCode);

            instrumentService.Verify(m => m.ChartInstrumentPricesAsync(instrumentId, dateRange, aggregationFrequency, currencyCode));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetInstrumentChartedProfit_ReturnsChartedProfit_WhenInstrumentExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();
            var chartedProfit = fixture.CreateMany<EntityChartPointDto>();
            var dateRange = fixture.Create<DateRangeParams>();
            var aggregationFrequency = fixture.Create<AggregationFrequency>();
            var currencyCode = fixture.Create<string>();

            var instrumentService = fixture.Freeze<Mock<IInstrumentService>>();
            instrumentService
                .Setup(m => m.ChartInstrumentProfitAsync(instrumentId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(chartedProfit));

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentChartedProfit(instrumentId, dateRange, aggregationFrequency, currencyCode);

            instrumentService.Verify(m => m.ChartInstrumentProfitAsync(instrumentId, dateRange, aggregationFrequency, currencyCode));
            Assert.Equal(chartedProfit, result.Value);
        }

        [Fact]
        public async Task GetInstrumentChartedProfit_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();
            var dateRange = fixture.Create<DateRangeParams>();
            var aggregationFrequency = fixture.Create<AggregationFrequency>();
            var currencyCode = fixture.Create<string>();

            var instrumentService = fixture.Freeze<Mock<IInstrumentService>>();
            instrumentService
                .Setup(m => m.ChartInstrumentProfitAsync(instrumentId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<IEnumerable<EntityChartPointDto>>());

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentChartedProfit(instrumentId, dateRange, aggregationFrequency, currencyCode);

            instrumentService.Verify(m => m.ChartInstrumentProfitAsync(instrumentId, dateRange, aggregationFrequency, currencyCode));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetInstrumentChartedPerformance_ReturnsChartedPerformance_WhenInstrumentExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();
            var chartedPerformance = fixture.CreateMany<EntityChartPointDto>();
            var dateRange = fixture.Create<DateRangeParams>();
            var aggregationFrequency = fixture.Create<AggregationFrequency>();

            var instrumentService = fixture.Freeze<Mock<IInstrumentService>>();
            instrumentService
                .Setup(m => m.ChartInstrumentPerformanceAsync(instrumentId, dateRange, aggregationFrequency))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(chartedPerformance));

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentChartedPerformance(instrumentId, dateRange, aggregationFrequency);

            instrumentService.Verify(m => m.ChartInstrumentPerformanceAsync(instrumentId, dateRange, aggregationFrequency));
            Assert.Equal(chartedPerformance, result.Value);
        }

        [Fact]
        public async Task GetInstrumentChartedPerformance_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();
            var dateRange = fixture.Create<DateRangeParams>();
            var aggregationFrequency = fixture.Create<AggregationFrequency>();

            var instrumentService = fixture.Freeze<Mock<IInstrumentService>>();
            instrumentService
                .Setup(m => m.ChartInstrumentPerformanceAsync(instrumentId, dateRange, aggregationFrequency))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<IEnumerable<EntityChartPointDto>>());

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentChartedPerformance(instrumentId, dateRange, aggregationFrequency);

            instrumentService.Verify(m => m.ChartInstrumentPerformanceAsync(instrumentId, dateRange, aggregationFrequency));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetInstrumentAggregatedPerformance_ReturnsAggregatedPerformance_WhenInstrumentExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();
            var aggregatedPerformance = fixture.CreateMany<EntityChartPointDto>();
            var dateRange = fixture.Create<DateRangeParams>();
            var aggregationFrequency = fixture.Create<AggregationFrequency>();

            var instrumentService = fixture.Freeze<Mock<IInstrumentService>>();
            instrumentService
                .Setup(m => m.ChartInstrumentAggregatedPerformanceAsync(instrumentId, dateRange, aggregationFrequency))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(aggregatedPerformance));

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentAggregatedPerformance(instrumentId, dateRange, aggregationFrequency);

            instrumentService.Verify(m => m.ChartInstrumentAggregatedPerformanceAsync(instrumentId, dateRange, aggregationFrequency));
            Assert.Equal(aggregatedPerformance, result.Value);
        }

        [Fact]
        public async Task GetInstrumentAggregatedPerformance_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();
            var dateRange = fixture.Create<DateRangeParams>();
            var aggregationFrequency = fixture.Create<AggregationFrequency>();

            var instrumentService = fixture.Freeze<Mock<IInstrumentService>>();
            instrumentService
                .Setup(m => m.ChartInstrumentAggregatedPerformanceAsync(instrumentId, dateRange, aggregationFrequency))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<IEnumerable<EntityChartPointDto>>());

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentAggregatedPerformance(instrumentId, dateRange, aggregationFrequency);

            instrumentService.Verify(m => m.ChartInstrumentAggregatedPerformanceAsync(instrumentId, dateRange, aggregationFrequency));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetInstrumentAggregatedProfit_ReturnsAggregatedProfit_WhenInstrumentExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();
            var aggregatedProfit = fixture.CreateMany<EntityChartPointDto>();
            var dateRange = fixture.Create<DateRangeParams>();
            var aggregationFrequency = fixture.Create<AggregationFrequency>();
            var currencyCode = fixture.Create<string>();

            var instrumentService = fixture.Freeze<Mock<IInstrumentService>>();
            instrumentService
                .Setup(m => m.ChartInstrumentAggregatedProfitAsync(instrumentId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(aggregatedProfit));

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentAggregatedProfit(instrumentId, dateRange, aggregationFrequency, currencyCode);

            instrumentService.Verify(m => m.ChartInstrumentAggregatedProfitAsync(instrumentId, dateRange, aggregationFrequency, currencyCode));
            Assert.Equal(aggregatedProfit, result.Value);
        }

        [Fact]
        public async Task GetInstrumentAggregatedProfit_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();
            var dateRange = fixture.Create<DateRangeParams>();
            var aggregationFrequency = fixture.Create<AggregationFrequency>();
            var currencyCode = fixture.Create<string>();

            var instrumentService = fixture.Freeze<Mock<IInstrumentService>>();
            instrumentService
                .Setup(m => m.ChartInstrumentAggregatedProfitAsync(instrumentId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<IEnumerable<EntityChartPointDto>>());

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentAggregatedProfit(instrumentId, dateRange, aggregationFrequency, currencyCode);

            instrumentService.Verify(m => m.ChartInstrumentAggregatedProfitAsync(instrumentId, dateRange, aggregationFrequency, currencyCode));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task PostInstrument_CreatesInstrument()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrument = fixture.Create<InstrumentDto>();

            var instrumentService = fixture.Freeze<Mock<IInstrumentService>>();
            instrumentService
                .Setup(m => m.CreateInstrumentAsync(instrument))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(instrument));
            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            await sut.PostInstrument(instrument);

            instrumentService.Verify(m => m.CreateInstrumentAsync(instrument), Times.Once());
        }

        [Fact]
        public async Task PutInstrument_UpdatesInstrument()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrument = fixture.Create<InstrumentDto>();

            var instrumentService = fixture.Freeze<Mock<IInstrumentService>>();
            instrumentService
                .Setup(m => m.UpdateInstrumentAsync(instrument))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(instrument));
            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            await sut.PutInstrument(instrument.Id, instrument);

            instrumentService.Verify(m => m.UpdateInstrumentAsync(instrument), Times.Once());
        }

        [Fact]
        public async Task PutInstrument_ReturnsBadRequest_WhenQueryParameterIdAndBodyIdDontMatch()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrument = fixture.Create<InstrumentDto>();

            var instrumentService = fixture.Freeze<Mock<IInstrumentService>>();
            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.PutInstrument(instrument.Id + 1, instrument);

            instrumentService.Verify(m => m.UpdateInstrumentAsync(instrument), Times.Never());
            Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeleteInstrument_DeletesInstrument()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();

            var instrumentService = fixture.Freeze<Mock<IInstrumentService>>();
            instrumentService
                .Setup(m => m.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse());
            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            await sut.DeleteInstrument(instrumentId);

            instrumentService.Verify(m => m.DeleteAsync(instrumentId), Times.Once());
        }
    }
}
