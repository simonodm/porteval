using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Features.Interfaces.Queries;
using PortEval.Application.Features.Interfaces.Services;
using PortEval.Application.Models;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Entities;
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

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.GetInstrumentsPage(pagination))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(instrumentsPage));
            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetAllInstruments(pagination);

            instrumentQueries.Verify(m => m.GetInstrumentsPage(pagination), Times.Once());
            Assert.Equal(instrumentsPage, result.Value);
        }

        [Fact]
        public async Task GetInstrument_ReturnsCorrectInstrument_WhenInstrumentExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrument = fixture.Create<InstrumentDto>();

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.GetInstrument(instrument.Id))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(instrument));

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrument(instrument.Id);

            instrumentQueries.Verify(m => m.GetInstrument(instrument.Id), Times.Once());
            Assert.Equal(instrument, result.Value);
        }

        [Fact]
        public async Task GetInstrument_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.GetInstrument(It.IsAny<int>()))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<InstrumentDto>());

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrument(instrumentId);

            instrumentQueries.Verify(m => m.GetInstrument(instrumentId), Times.Once());
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

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.GetInstrumentProfit(instrumentId, dateRange))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(profit));

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentProfit(instrumentId, dateRange);

            instrumentQueries.Verify(m => m.GetInstrumentProfit(instrumentId, dateRange));
            Assert.Equal(profit, result.Value);
        }

        [Fact]
        public async Task GetInstrumentProfit_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();
            var dateRange = fixture.Create<DateRangeParams>();

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.GetInstrumentProfit(instrumentId, dateRange))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<EntityProfitDto>());

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentProfit(instrumentId, dateRange);

            instrumentQueries.Verify(m => m.GetInstrumentProfit(instrumentId, dateRange));
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

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.GetInstrumentPerformance(instrumentId, dateRange))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(performance));

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentPerformance(instrumentId, dateRange);

            instrumentQueries.Verify(m => m.GetInstrumentPerformance(instrumentId, dateRange));
            Assert.Equal(performance, result.Value);
        }

        [Fact]
        public async Task GetInstrumentPerformance_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();
            var dateRange = fixture.Create<DateRangeParams>();

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.GetInstrumentPerformance(instrumentId, dateRange))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<EntityPerformanceDto>());

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentPerformance(instrumentId, dateRange);

            instrumentQueries.Verify(m => m.GetInstrumentPerformance(instrumentId, dateRange));
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

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.ChartInstrumentPrices(instrumentId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(chartedPrices));

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentChartedPrices(instrumentId, dateRange, aggregationFrequency, currencyCode);

            instrumentQueries.Verify(m => m.ChartInstrumentPrices(instrumentId, dateRange, aggregationFrequency, currencyCode));
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

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.ChartInstrumentPrices(instrumentId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<IEnumerable<EntityChartPointDto>>());

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentChartedPrices(instrumentId, dateRange, aggregationFrequency, currencyCode);

            instrumentQueries.Verify(m => m.ChartInstrumentPrices(instrumentId, dateRange, aggregationFrequency, currencyCode));
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

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.ChartInstrumentProfit(instrumentId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(chartedProfit));

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentChartedProfit(instrumentId, dateRange, aggregationFrequency, currencyCode);

            instrumentQueries.Verify(m => m.ChartInstrumentProfit(instrumentId, dateRange, aggregationFrequency, currencyCode));
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

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.ChartInstrumentProfit(instrumentId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<IEnumerable<EntityChartPointDto>>());

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentChartedProfit(instrumentId, dateRange, aggregationFrequency, currencyCode);

            instrumentQueries.Verify(m => m.ChartInstrumentProfit(instrumentId, dateRange, aggregationFrequency, currencyCode));
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

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.ChartInstrumentPerformance(instrumentId, dateRange, aggregationFrequency))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(chartedPerformance));

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentChartedPerformance(instrumentId, dateRange, aggregationFrequency);

            instrumentQueries.Verify(m => m.ChartInstrumentPerformance(instrumentId, dateRange, aggregationFrequency));
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

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.ChartInstrumentPerformance(instrumentId, dateRange, aggregationFrequency))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<IEnumerable<EntityChartPointDto>>());

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentChartedPerformance(instrumentId, dateRange, aggregationFrequency);

            instrumentQueries.Verify(m => m.ChartInstrumentPerformance(instrumentId, dateRange, aggregationFrequency));
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

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.ChartInstrumentPerformanceAggregated(instrumentId, dateRange, aggregationFrequency))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(aggregatedPerformance));

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentAggregatedPerformance(instrumentId, dateRange, aggregationFrequency);

            instrumentQueries.Verify(m => m.ChartInstrumentPerformanceAggregated(instrumentId, dateRange, aggregationFrequency));
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

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.ChartInstrumentPerformanceAggregated(instrumentId, dateRange, aggregationFrequency))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<IEnumerable<EntityChartPointDto>>());

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentAggregatedPerformance(instrumentId, dateRange, aggregationFrequency);

            instrumentQueries.Verify(m => m.ChartInstrumentPerformanceAggregated(instrumentId, dateRange, aggregationFrequency));
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

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.ChartInstrumentProfitAggregated(instrumentId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(aggregatedProfit));

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentAggregatedProfit(instrumentId, dateRange, aggregationFrequency, currencyCode);

            instrumentQueries.Verify(m => m.ChartInstrumentProfitAggregated(instrumentId, dateRange, aggregationFrequency, currencyCode));
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

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.ChartInstrumentProfitAggregated(instrumentId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<IEnumerable<EntityChartPointDto>>());

            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentAggregatedProfit(instrumentId, dateRange, aggregationFrequency, currencyCode);

            instrumentQueries.Verify(m => m.ChartInstrumentProfitAggregated(instrumentId, dateRange, aggregationFrequency, currencyCode));
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
                .ReturnsAsync(fixture.Create<Instrument>());
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
            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteInstrument_DeletesInstrument()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();

            var instrumentService = fixture.Freeze<Mock<IInstrumentService>>();
            var sut = fixture.Build<InstrumentsController>().OmitAutoProperties().Create();

            await sut.DeleteInstrument(instrumentId);

            instrumentService.Verify(m => m.DeleteAsync(instrumentId), Times.Once());
        }
    }
}
