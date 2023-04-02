using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Tests.Unit.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Interfaces.Services;
using Xunit;

namespace PortEval.Tests.Unit.ControllerTests
{
    public class PositionControllerTests
    {
        [Fact]
        public async Task GetPosition_ReturnsCorrectPosition_WhenPositionExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Create<PositionDto>();

            var positionQueries = fixture.Freeze<Mock<IPositionQueries>>();
            positionQueries
                .Setup(m => m.GetPosition(position.Id))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(position));

            var sut = fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPosition(position.Id);

            positionQueries.Verify(m => m.GetPosition(position.Id), Times.Once());
            Assert.Equal(position, result.Value);
        }

        [Fact]
        public async Task GetPosition_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionId = fixture.Create<int>();

            var positionQueries = fixture.Freeze<Mock<IPositionQueries>>();
            positionQueries
                .Setup(m => m.GetPosition(It.IsAny<int>()))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<PositionDto>());

            var sut = fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPosition(positionId);

            positionQueries.Verify(m => m.GetPosition(positionId), Times.Once());
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPositionValue_ReturnsPositionValue_WhenPositionExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionId = fixture.Create<int>();
            var value = fixture.Create<EntityValueDto>();

            var positionQueries = fixture.Freeze<Mock<IPositionQueries>>();
            positionQueries
                .Setup(m => m.GetPositionValue(positionId, value.Time))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(value));

            var sut = fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionValue(positionId, value.Time);

            positionQueries.Verify(m => m.GetPositionValue(positionId, value.Time));
            Assert.Equal(value, result.Value);
        }

        [Fact]
        public async Task GetPositionValue_ReturnsCurrentPositionValue_WhenTimeParameterIsNotProvided()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var now = DateTime.UtcNow;
            var positionId = fixture.Create<int>();
            var value = fixture.Build<EntityValueDto>().With(v => v.Time, now).Create();

            var positionQueries = fixture.Freeze<Mock<IPositionQueries>>();
            positionQueries
                .Setup(m => m.GetPositionValue(positionId, It.Is<DateTime>(d => d >= now)))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(value));

            var sut = fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionValue(positionId, null);

            positionQueries.Verify(m => m.GetPositionValue(positionId, It.Is<DateTime>(d => d >= now)));
            Assert.Equal(value, result.Value);
        }

        [Fact]
        public async Task GetPositionValue_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionId = fixture.Create<int>();

            var positionQueries = fixture.Freeze<Mock<IPositionQueries>>();
            positionQueries
                .Setup(m => m.GetPositionValue(positionId, It.IsAny<DateTime>()))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<EntityValueDto>());

            var sut = fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionValue(positionId, DateTime.UtcNow);

            positionQueries.Verify(m => m.GetPositionValue(positionId, It.IsAny<DateTime>()));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPositionProfit_ReturnsProfit_WhenPositionExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionId = fixture.Create<int>();
            var profit = fixture.Create<EntityProfitDto>();
            var dateRange = fixture.Create<DateRangeParams>();

            var positionQueries = fixture.Freeze<Mock<IPositionQueries>>();
            positionQueries
                .Setup(m => m.GetPositionProfit(positionId, dateRange))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(profit));

            var sut = fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionProfit(positionId, dateRange);

            positionQueries.Verify(m => m.GetPositionProfit(positionId, dateRange));
            Assert.Equal(profit, result.Value);
        }

        [Fact]
        public async Task GetPositionProfit_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionId = fixture.Create<int>();
            var dateRange = fixture.Create<DateRangeParams>();

            var positionQueries = fixture.Freeze<Mock<IPositionQueries>>();
            positionQueries
                .Setup(m => m.GetPositionProfit(positionId, dateRange))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<EntityProfitDto>());

            var sut = fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionProfit(positionId, dateRange);

            positionQueries.Verify(m => m.GetPositionProfit(positionId, dateRange));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPositionPerformance_ReturnsPositionPerformance_WhenPositionExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionId = fixture.Create<int>();
            var performance = fixture.Create<EntityPerformanceDto>();
            var dateRange = fixture.Create<DateRangeParams>();

            var positionQueries = fixture.Freeze<Mock<IPositionQueries>>();
            positionQueries
                .Setup(m => m.GetPositionPerformance(positionId, dateRange))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(performance));

            var sut = fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionPerformance(positionId, dateRange);

            positionQueries.Verify(m => m.GetPositionPerformance(positionId, dateRange));
            Assert.Equal(performance, result.Value);
        }

        [Fact]
        public async Task GetPositionPerformance_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionId = fixture.Create<int>();
            var dateRange = fixture.Create<DateRangeParams>();

            var positionQueries = fixture.Freeze<Mock<IPositionQueries>>();
            positionQueries
                .Setup(m => m.GetPositionPerformance(positionId, dateRange))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<EntityPerformanceDto>());

            var sut = fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionPerformance(positionId, dateRange);

            positionQueries.Verify(m => m.GetPositionPerformance(positionId, dateRange));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPositionBreakEvenPoint_ReturnsBreakEvenPoint_WhenPositionExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionId = fixture.Create<int>();
            var bep = fixture.Create<PositionBreakEvenPointDto>();
            var time = fixture.Create<DateTime>();

            var positionQueries = fixture.Freeze<Mock<IPositionQueries>>();
            positionQueries
                .Setup(m => m.GetPositionBreakEvenPoint(positionId, time))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(bep));

            var sut = fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionBreakEvenPoint(positionId, time);

            positionQueries.Verify(m => m.GetPositionBreakEvenPoint(positionId, time));
            Assert.Equal(bep, result.Value);
        }

        [Fact]
        public async Task GetPositionBreakEvenPoint_ReturnsCurrentBreakEvenPoint_WhenTimeQueryParameterIsNotProvided()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionId = fixture.Create<int>();
            var bep = fixture.Create<PositionBreakEvenPointDto>();
            var now = DateTime.UtcNow;

            var positionQueries = fixture.Freeze<Mock<IPositionQueries>>();
            positionQueries
                .Setup(m => m.GetPositionBreakEvenPoint(positionId, It.Is<DateTime>(dt => dt >= now)))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(bep));

            var sut = fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionBreakEvenPoint(positionId, null);

            positionQueries.Verify(m => m.GetPositionBreakEvenPoint(positionId, It.Is<DateTime>(dt => dt >= now)));
            Assert.Equal(bep, result.Value);
        }

        [Fact]
        public async Task GetPositionBreakEvenPoint_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionId = fixture.Create<int>();
            var time = fixture.Create<DateTime>();

            var positionQueries = fixture.Freeze<Mock<IPositionQueries>>();
            positionQueries
                .Setup(m => m.GetPositionBreakEvenPoint(positionId, time))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<PositionBreakEvenPointDto>());

            var sut = fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionBreakEvenPoint(positionId, time);

            positionQueries.Verify(m => m.GetPositionBreakEvenPoint(positionId, time));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPositionChartedValue_ReturnsChartedValue_WhenPositionExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionId = fixture.Create<int>();
            var chartedValue = fixture.CreateMany<EntityChartPointDto>();
            var dateRange = fixture.Create<DateRangeParams>();
            var aggregationFrequency = fixture.Create<AggregationFrequency>();
            var currencyCode = fixture.Create<string>();

            var positionQueries = fixture.Freeze<Mock<IPositionQueries>>();
            positionQueries
                .Setup(m => m.ChartPositionValue(positionId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(chartedValue));

            var sut = fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionChartedValue(positionId, dateRange, aggregationFrequency, currencyCode);

            positionQueries.Verify(m => m.ChartPositionValue(positionId, dateRange, aggregationFrequency, currencyCode));
            Assert.Equal(chartedValue, result.Value);
        }

        [Fact]
        public async Task GetPositionChartedValue_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionId = fixture.Create<int>();
            var dateRange = fixture.Create<DateRangeParams>();
            var aggregationFrequency = fixture.Create<AggregationFrequency>();
            var currencyCode = fixture.Create<string>();

            var positionQueries = fixture.Freeze<Mock<IPositionQueries>>();
            positionQueries
                .Setup(m => m.ChartPositionValue(positionId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<IEnumerable<EntityChartPointDto>>());

            var sut = fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionChartedValue(positionId, dateRange, aggregationFrequency, currencyCode);

            positionQueries.Verify(m => m.ChartPositionValue(positionId, dateRange, aggregationFrequency, currencyCode));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPositionChartedProfit_ReturnsChartedProfit_WhenPositionExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionId = fixture.Create<int>();
            var chartedProfit = fixture.CreateMany<EntityChartPointDto>();
            var dateRange = fixture.Create<DateRangeParams>();
            var aggregationFrequency = fixture.Create<AggregationFrequency>();
            var currencyCode = fixture.Create<string>();

            var positionQueries = fixture.Freeze<Mock<IPositionQueries>>();
            positionQueries
                .Setup(m => m.ChartPositionProfit(positionId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(chartedProfit));

            var sut = fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionChartedProfit(positionId, dateRange, aggregationFrequency, currencyCode);

            positionQueries.Verify(m => m.ChartPositionProfit(positionId, dateRange, aggregationFrequency, currencyCode));
            Assert.Equal(chartedProfit, result.Value);
        }

        [Fact]
        public async Task GetPositionChartedProfit_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionId = fixture.Create<int>();
            var dateRange = fixture.Create<DateRangeParams>();
            var aggregationFrequency = fixture.Create<AggregationFrequency>();
            var currencyCode = fixture.Create<string>();

            var positionQueries = fixture.Freeze<Mock<IPositionQueries>>();
            positionQueries
                .Setup(m => m.ChartPositionProfit(positionId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<IEnumerable<EntityChartPointDto>>());

            var sut = fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionChartedProfit(positionId, dateRange, aggregationFrequency, currencyCode);

            positionQueries.Verify(m => m.ChartPositionProfit(positionId, dateRange, aggregationFrequency, currencyCode));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPositionChartedPerformance_ReturnsChartedPerformance_WhenPositionExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionId = fixture.Create<int>();
            var chartedPerformance = fixture.CreateMany<EntityChartPointDto>();
            var dateRange = fixture.Create<DateRangeParams>();
            var aggregationFrequency = fixture.Create<AggregationFrequency>();

            var positionQueries = fixture.Freeze<Mock<IPositionQueries>>();
            positionQueries
                .Setup(m => m.ChartPositionPerformance(positionId, dateRange, aggregationFrequency))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(chartedPerformance));

            var sut = fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionChartedPerformance(positionId, dateRange, aggregationFrequency);

            positionQueries.Verify(m => m.ChartPositionPerformance(positionId, dateRange, aggregationFrequency));
            Assert.Equal(chartedPerformance, result.Value);
        }

        [Fact]
        public async Task GetPositionChartedPerformance_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionId = fixture.Create<int>();
            var dateRange = fixture.Create<DateRangeParams>();
            var aggregationFrequency = fixture.Create<AggregationFrequency>();

            var positionQueries = fixture.Freeze<Mock<IPositionQueries>>();
            positionQueries
                .Setup(m => m.ChartPositionPerformance(positionId, dateRange, aggregationFrequency))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<IEnumerable<EntityChartPointDto>>());

            var sut = fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionChartedPerformance(positionId, dateRange, aggregationFrequency);

            positionQueries.Verify(m => m.ChartPositionPerformance(positionId, dateRange, aggregationFrequency));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPositionAggregatedPerformance_ReturnsAggregatedPerformance_WhenPositionExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionId = fixture.Create<int>();
            var aggregatedPerformance = fixture.CreateMany<EntityChartPointDto>();
            var dateRange = fixture.Create<DateRangeParams>();
            var aggregationFrequency = fixture.Create<AggregationFrequency>();

            var positionQueries = fixture.Freeze<Mock<IPositionQueries>>();
            positionQueries
                .Setup(m => m.ChartPositionPerformanceAggregated(positionId, dateRange, aggregationFrequency))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(aggregatedPerformance));

            var sut = fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionAggregatedPerformance(positionId, dateRange, aggregationFrequency);

            positionQueries.Verify(m => m.ChartPositionPerformanceAggregated(positionId, dateRange, aggregationFrequency));
            Assert.Equal(aggregatedPerformance, result.Value);
        }

        [Fact]
        public async Task GetPositionAggregatedPerformance_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionId = fixture.Create<int>();
            var dateRange = fixture.Create<DateRangeParams>();
            var aggregationFrequency = fixture.Create<AggregationFrequency>();

            var positionQueries = fixture.Freeze<Mock<IPositionQueries>>();
            positionQueries
                .Setup(m => m.ChartPositionPerformanceAggregated(positionId, dateRange, aggregationFrequency))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<IEnumerable<EntityChartPointDto>>());

            var sut = fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionAggregatedPerformance(positionId, dateRange, aggregationFrequency);

            positionQueries.Verify(m => m.ChartPositionPerformanceAggregated(positionId, dateRange, aggregationFrequency));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPositionAggregatedProfit_ReturnsAggregatedProfit_WhenPositionExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionId = fixture.Create<int>();
            var aggregatedProfit = fixture.CreateMany<EntityChartPointDto>();
            var dateRange = fixture.Create<DateRangeParams>();
            var aggregationFrequency = fixture.Create<AggregationFrequency>();
            var currencyCode = fixture.Create<string>();

            var positionQueries = fixture.Freeze<Mock<IPositionQueries>>();
            positionQueries
                .Setup(m => m.ChartPositionProfitAggregated(positionId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(aggregatedProfit));

            var sut = fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionAggregatedProfit(positionId, dateRange, aggregationFrequency, currencyCode);

            positionQueries.Verify(m => m.ChartPositionProfitAggregated(positionId, dateRange, aggregationFrequency, currencyCode));
            Assert.Equal(aggregatedProfit, result.Value);
        }

        [Fact]
        public async Task GetPositionAggregatedProfit_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionId = fixture.Create<int>();
            var dateRange = fixture.Create<DateRangeParams>();
            var aggregationFrequency = fixture.Create<AggregationFrequency>();
            var currencyCode = fixture.Create<string>();

            var positionQueries = fixture.Freeze<Mock<IPositionQueries>>();
            positionQueries
                .Setup(m => m.ChartPositionProfitAggregated(positionId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<IEnumerable<EntityChartPointDto>>());

            var sut = fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionAggregatedProfit(positionId, dateRange, aggregationFrequency, currencyCode);

            positionQueries.Verify(m => m.ChartPositionProfitAggregated(positionId, dateRange, aggregationFrequency, currencyCode));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPositionStatistics_ReturnsPositionStatistics_WhenPositionExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionId = fixture.Create<int>();
            var statistics = fixture.Create<PositionStatisticsDto>();

            var positionQueries = fixture.Freeze<Mock<IPositionQueries>>();
            positionQueries
                .Setup(m => m.GetPositionStatistics(positionId))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(statistics));

            var sut = fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionStatistics(positionId);

            positionQueries.Verify(m => m.GetPositionStatistics(positionId), Times.Once());
            Assert.Equal(statistics, result.Value);
        }

        [Fact]
        public async Task GetPositionStatistics_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionId = fixture.Create<int>();

            var positionQueries = fixture.Freeze<Mock<IPositionQueries>>();
            positionQueries
                .Setup(m => m.GetPositionStatistics(positionId))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<PositionStatisticsDto>());

            var sut = fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionStatistics(positionId);

            positionQueries.Verify(m => m.GetPositionStatistics(positionId), Times.Once());
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task PostPosition_CreatesPosition()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Create<PositionDto>();

            var positionService = fixture.Freeze<Mock<IPositionService>>();
            positionService
                .Setup(m => m.OpenPositionAsync(position))
                .ReturnsAsync(fixture.Create<Position>());
            var sut = fixture.Build<PositionsController>().OmitAutoProperties().Create();

            await sut.PostPosition(position);

            positionService.Verify(m => m.OpenPositionAsync(position), Times.Once());
        }

        [Fact]
        public async Task PutPosition_UpdatesPosition()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Create<PositionDto>();

            var positionService = fixture.Freeze<Mock<IPositionService>>();
            var sut = fixture.Build<PositionsController>().OmitAutoProperties().Create();

            await sut.PutPosition(position.Id, position);

            positionService.Verify(m => m.UpdatePositionAsync(position), Times.Once());
        }

        [Fact]
        public async Task PutPosition_ReturnsBadRequest_WhenQueryParameterIdAndBodyIdDontMatch()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Create<PositionDto>();

            var positionService = fixture.Freeze<Mock<IPositionService>>();
            var sut = fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.PutPosition(position.Id + 1, position);

            positionService.Verify(m => m.UpdatePositionAsync(position), Times.Never());
            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeletePosition_DeletesPosition()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionId = fixture.Create<int>();

            var positionService = fixture.Freeze<Mock<IPositionService>>();
            var sut = fixture.Build<PositionsController>().OmitAutoProperties().Create();

            await sut.DeletePosition(positionId);

            positionService.Verify(m => m.RemovePositionAsync(positionId), Times.Once());
        }
    }
}
