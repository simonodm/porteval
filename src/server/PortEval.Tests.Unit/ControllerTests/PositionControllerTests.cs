using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;
using PortEval.Tests.Unit.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.ControllerTests
{
    public class PositionControllerTests
    {
        private IFixture _fixture;
        private Mock<IPositionService> _positionService;

        public PositionControllerTests()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            _positionService = _fixture.Freeze<Mock<IPositionService>>();
        }

        [Fact]
        public async Task GetPosition_ReturnsCorrectPosition_WhenPositionExists()
        {
            var position = _fixture.Create<PositionDto>();

            _positionService
                .Setup(m => m.GetPositionAsync(position.Id))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(position));

            var sut = _fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPosition(position.Id);

            _positionService.Verify(m => m.GetPositionAsync(position.Id), Times.Once());
            Assert.Equal(position, result.Value);
        }

        [Fact]
        public async Task GetPosition_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var positionId = _fixture.Create<int>();

            _positionService
                .Setup(m => m.GetPositionAsync(It.IsAny<int>()))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<PositionDto>());

            var sut = _fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPosition(positionId);

            _positionService.Verify(m => m.GetPositionAsync(positionId), Times.Once());
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPositionValue_ReturnsPositionValue_WhenPositionExists()
        {
            var positionId = _fixture.Create<int>();
            var value = _fixture.Create<EntityValueDto>();

            _positionService
                .Setup(m => m.GetPositionValueAsync(positionId, value.Time))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(value));

            var sut = _fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionValue(positionId, value.Time);

            _positionService.Verify(m => m.GetPositionValueAsync(positionId, value.Time));
            Assert.Equal(value, result.Value);
        }

        [Fact]
        public async Task GetPositionValue_ReturnsCurrentPositionValue_WhenTimeParameterIsNotProvided()
        {
            var now = DateTime.UtcNow;
            var positionId = _fixture.Create<int>();
            var value = _fixture.Build<EntityValueDto>().With(v => v.Time, now).Create();

            _positionService
                .Setup(m => m.GetPositionValueAsync(positionId, It.Is<DateTime>(d => d >= now)))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(value));

            var sut = _fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionValue(positionId, null);

            _positionService.Verify(m => m.GetPositionValueAsync(positionId, It.Is<DateTime>(d => d >= now)));
            Assert.Equal(value, result.Value);
        }

        [Fact]
        public async Task GetPositionValue_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var positionId = _fixture.Create<int>();

            _positionService
                .Setup(m => m.GetPositionValueAsync(positionId, It.IsAny<DateTime>()))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<EntityValueDto>());

            var sut = _fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionValue(positionId, DateTime.UtcNow);

            _positionService.Verify(m => m.GetPositionValueAsync(positionId, It.IsAny<DateTime>()));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPositionProfit_ReturnsProfit_WhenPositionExists()
        {
            var positionId = _fixture.Create<int>();
            var profit = _fixture.Create<EntityProfitDto>();
            var dateRange = _fixture.Create<DateRangeParams>();

            _positionService
                .Setup(m => m.GetPositionProfitAsync(positionId, dateRange))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(profit));

            var sut = _fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionProfit(positionId, dateRange);

            _positionService.Verify(m => m.GetPositionProfitAsync(positionId, dateRange));
            Assert.Equal(profit, result.Value);
        }

        [Fact]
        public async Task GetPositionProfit_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var positionId = _fixture.Create<int>();
            var dateRange = _fixture.Create<DateRangeParams>();

            _positionService
                .Setup(m => m.GetPositionProfitAsync(positionId, dateRange))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<EntityProfitDto>());

            var sut = _fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionProfit(positionId, dateRange);

            _positionService.Verify(m => m.GetPositionProfitAsync(positionId, dateRange));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPositionPerformance_ReturnsPositionPerformance_WhenPositionExists()
        {
            var positionId = _fixture.Create<int>();
            var performance = _fixture.Create<EntityPerformanceDto>();
            var dateRange = _fixture.Create<DateRangeParams>();

            _positionService
                .Setup(m => m.GetPositionPerformanceAsync(positionId, dateRange))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(performance));

            var sut = _fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionPerformance(positionId, dateRange);

            _positionService.Verify(m => m.GetPositionPerformanceAsync(positionId, dateRange));
            Assert.Equal(performance, result.Value);
        }

        [Fact]
        public async Task GetPositionPerformance_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var positionId = _fixture.Create<int>();
            var dateRange = _fixture.Create<DateRangeParams>();

            _positionService
                .Setup(m => m.GetPositionPerformanceAsync(positionId, dateRange))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<EntityPerformanceDto>());

            var sut = _fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionPerformance(positionId, dateRange);

            _positionService.Verify(m => m.GetPositionPerformanceAsync(positionId, dateRange));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPositionBreakEvenPoint_ReturnsBreakEvenPoint_WhenPositionExists()
        {
            var positionId = _fixture.Create<int>();
            var bep = _fixture.Create<PositionBreakEvenPointDto>();
            var time = _fixture.Create<DateTime>();

            _positionService
                .Setup(m => m.GetPositionBreakEvenPointAsync(positionId, time))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(bep));

            var sut = _fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionBreakEvenPoint(positionId, time);

            _positionService.Verify(m => m.GetPositionBreakEvenPointAsync(positionId, time));
            Assert.Equal(bep, result.Value);
        }

        [Fact]
        public async Task GetPositionBreakEvenPoint_ReturnsCurrentBreakEvenPoint_WhenTimeQueryParameterIsNotProvided()
        {
            var positionId = _fixture.Create<int>();
            var bep = _fixture.Create<PositionBreakEvenPointDto>();
            var now = DateTime.UtcNow;

            _positionService
                .Setup(m => m.GetPositionBreakEvenPointAsync(positionId, It.Is<DateTime>(dt => dt >= now)))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(bep));

            var sut = _fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionBreakEvenPoint(positionId, null);

            _positionService.Verify(m => m.GetPositionBreakEvenPointAsync(positionId, It.Is<DateTime>(dt => dt >= now)));
            Assert.Equal(bep, result.Value);
        }

        [Fact]
        public async Task GetPositionBreakEvenPoint_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var positionId = _fixture.Create<int>();
            var time = _fixture.Create<DateTime>();

            _positionService
                .Setup(m => m.GetPositionBreakEvenPointAsync(positionId, time))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<PositionBreakEvenPointDto>());

            var sut = _fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionBreakEvenPoint(positionId, time);

            _positionService.Verify(m => m.GetPositionBreakEvenPointAsync(positionId, time));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPositionChartedValue_ReturnsChartedValue_WhenPositionExists()
        {
            var positionId = _fixture.Create<int>();
            var chartedValue = _fixture.CreateMany<EntityChartPointDto>();
            var dateRange = _fixture.Create<DateRangeParams>();
            var aggregationFrequency = _fixture.Create<AggregationFrequency>();
            var currencyCode = _fixture.Create<string>();

            _positionService
                .Setup(m => m.ChartPositionValueAsync(positionId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(chartedValue));

            var sut = _fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionChartedValue(positionId, dateRange, aggregationFrequency, currencyCode);

            _positionService.Verify(m => m.ChartPositionValueAsync(positionId, dateRange, aggregationFrequency, currencyCode));
            Assert.Equal(chartedValue, result.Value);
        }

        [Fact]
        public async Task GetPositionChartedValue_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var positionId = _fixture.Create<int>();
            var dateRange = _fixture.Create<DateRangeParams>();
            var aggregationFrequency = _fixture.Create<AggregationFrequency>();
            var currencyCode = _fixture.Create<string>();

            _positionService
                .Setup(m => m.ChartPositionValueAsync(positionId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<IEnumerable<EntityChartPointDto>>());

            var sut = _fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionChartedValue(positionId, dateRange, aggregationFrequency, currencyCode);

            _positionService.Verify(m => m.ChartPositionValueAsync(positionId, dateRange, aggregationFrequency, currencyCode));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPositionChartedProfit_ReturnsChartedProfit_WhenPositionExists()
        {
            var positionId = _fixture.Create<int>();
            var chartedProfit = _fixture.CreateMany<EntityChartPointDto>();
            var dateRange = _fixture.Create<DateRangeParams>();
            var aggregationFrequency = _fixture.Create<AggregationFrequency>();
            var currencyCode = _fixture.Create<string>();

            _positionService
                .Setup(m => m.ChartPositionProfitAsync(positionId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(chartedProfit));

            var sut = _fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionChartedProfit(positionId, dateRange, aggregationFrequency, currencyCode);

            _positionService.Verify(m => m.ChartPositionProfitAsync(positionId, dateRange, aggregationFrequency, currencyCode));
            Assert.Equal(chartedProfit, result.Value);
        }

        [Fact]
        public async Task GetPositionChartedProfit_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var positionId = _fixture.Create<int>();
            var dateRange = _fixture.Create<DateRangeParams>();
            var aggregationFrequency = _fixture.Create<AggregationFrequency>();
            var currencyCode = _fixture.Create<string>();

            _positionService
                .Setup(m => m.ChartPositionProfitAsync(positionId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<IEnumerable<EntityChartPointDto>>());

            var sut = _fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionChartedProfit(positionId, dateRange, aggregationFrequency, currencyCode);

            _positionService.Verify(m => m.ChartPositionProfitAsync(positionId, dateRange, aggregationFrequency, currencyCode));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPositionChartedPerformance_ReturnsChartedPerformance_WhenPositionExists()
        {
            var positionId = _fixture.Create<int>();
            var chartedPerformance = _fixture.CreateMany<EntityChartPointDto>();
            var dateRange = _fixture.Create<DateRangeParams>();
            var aggregationFrequency = _fixture.Create<AggregationFrequency>();

            _positionService
                .Setup(m => m.ChartPositionPerformanceAsync(positionId, dateRange, aggregationFrequency))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(chartedPerformance));

            var sut = _fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionChartedPerformance(positionId, dateRange, aggregationFrequency);

            _positionService.Verify(m => m.ChartPositionPerformanceAsync(positionId, dateRange, aggregationFrequency));
            Assert.Equal(chartedPerformance, result.Value);
        }

        [Fact]
        public async Task GetPositionChartedPerformance_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var positionId = _fixture.Create<int>();
            var dateRange = _fixture.Create<DateRangeParams>();
            var aggregationFrequency = _fixture.Create<AggregationFrequency>();

            _positionService
                .Setup(m => m.ChartPositionPerformanceAsync(positionId, dateRange, aggregationFrequency))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<IEnumerable<EntityChartPointDto>>());

            var sut = _fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionChartedPerformance(positionId, dateRange, aggregationFrequency);

            _positionService.Verify(m => m.ChartPositionPerformanceAsync(positionId, dateRange, aggregationFrequency));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPositionAggregatedPerformance_ReturnsAggregatedPerformance_WhenPositionExists()
        {
            var positionId = _fixture.Create<int>();
            var aggregatedPerformance = _fixture.CreateMany<EntityChartPointDto>();
            var dateRange = _fixture.Create<DateRangeParams>();
            var aggregationFrequency = _fixture.Create<AggregationFrequency>();

            _positionService
                .Setup(m => m.ChartPositionAggregatedPerformanceAsync(positionId, dateRange, aggregationFrequency))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(aggregatedPerformance));

            var sut = _fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionAggregatedPerformance(positionId, dateRange, aggregationFrequency);

            _positionService.Verify(m => m.ChartPositionAggregatedPerformanceAsync(positionId, dateRange, aggregationFrequency));
            Assert.Equal(aggregatedPerformance, result.Value);
        }

        [Fact]
        public async Task GetPositionAggregatedPerformance_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var positionId = _fixture.Create<int>();
            var dateRange = _fixture.Create<DateRangeParams>();
            var aggregationFrequency = _fixture.Create<AggregationFrequency>();

            _positionService
                .Setup(m => m.ChartPositionAggregatedPerformanceAsync(positionId, dateRange, aggregationFrequency))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<IEnumerable<EntityChartPointDto>>());

            var sut = _fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionAggregatedPerformance(positionId, dateRange, aggregationFrequency);

            _positionService.Verify(m => m.ChartPositionAggregatedPerformanceAsync(positionId, dateRange, aggregationFrequency));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPositionAggregatedProfit_ReturnsAggregatedProfit_WhenPositionExists()
        {
            var positionId = _fixture.Create<int>();
            var aggregatedProfit = _fixture.CreateMany<EntityChartPointDto>();
            var dateRange = _fixture.Create<DateRangeParams>();
            var aggregationFrequency = _fixture.Create<AggregationFrequency>();
            var currencyCode = _fixture.Create<string>();

            _positionService
                .Setup(m => m.ChartPositionAggregatedProfitAsync(positionId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(aggregatedProfit));

            var sut = _fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionAggregatedProfit(positionId, dateRange, aggregationFrequency, currencyCode);

            _positionService.Verify(m => m.ChartPositionAggregatedProfitAsync(positionId, dateRange, aggregationFrequency, currencyCode));
            Assert.Equal(aggregatedProfit, result.Value);
        }

        [Fact]
        public async Task GetPositionAggregatedProfit_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var positionId = _fixture.Create<int>();
            var dateRange = _fixture.Create<DateRangeParams>();
            var aggregationFrequency = _fixture.Create<AggregationFrequency>();
            var currencyCode = _fixture.Create<string>();

            _positionService
                .Setup(m => m.ChartPositionAggregatedProfitAsync(positionId, dateRange, aggregationFrequency, currencyCode))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<IEnumerable<EntityChartPointDto>>());

            var sut = _fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionAggregatedProfit(positionId, dateRange, aggregationFrequency, currencyCode);

            _positionService.Verify(m => m.ChartPositionAggregatedProfitAsync(positionId, dateRange, aggregationFrequency, currencyCode));
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetPositionStatistics_ReturnsPositionStatistics_WhenPositionExists()
        {
            var positionId = _fixture.Create<int>();
            var statistics = _fixture.Create<PositionStatisticsDto>();

            _positionService
                .Setup(m => m.GetPositionStatisticsAsync(positionId))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(statistics));

            var sut = _fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionStatistics(positionId);

            _positionService.Verify(m => m.GetPositionStatisticsAsync(positionId), Times.Once());
            Assert.Equal(statistics, result.Value);
        }

        [Fact]
        public async Task GetPositionStatistics_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var positionId = _fixture.Create<int>();

            _positionService
                .Setup(m => m.GetPositionStatisticsAsync(positionId))
                .ReturnsAsync(OperationResponseHelper.GenerateNotFoundOperationResponse<PositionStatisticsDto>());

            var sut = _fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionStatistics(positionId);

            _positionService.Verify(m => m.GetPositionStatisticsAsync(positionId), Times.Once());
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task PostPosition_CreatesPosition()
        {
            var position = _fixture.Create<PositionDto>();

            _positionService
                .Setup(m => m.OpenPositionAsync(position))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(position));
            var sut = _fixture.Build<PositionsController>().OmitAutoProperties().Create();

            await sut.PostPosition(position);

            _positionService.Verify(m => m.OpenPositionAsync(position), Times.Once());
        }

        [Fact]
        public async Task PutPosition_UpdatesPosition()
        {
            var position = _fixture.Create<PositionDto>();

            _positionService
                .Setup(m => m.UpdatePositionAsync(position))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(position));

            var sut = _fixture.Build<PositionsController>().OmitAutoProperties().Create();

            await sut.PutPosition(position.Id, position);

            _positionService.Verify(m => m.UpdatePositionAsync(position), Times.Once());
        }

        [Fact]
        public async Task PutPosition_ReturnsBadRequest_WhenQueryParameterIdAndBodyIdDontMatch()
        {
            var position = _fixture.Create<PositionDto>();

            var sut = _fixture.Build<PositionsController>().OmitAutoProperties().Create();

            var result = await sut.PutPosition(position.Id + 1, position);

            _positionService.Verify(m => m.UpdatePositionAsync(position), Times.Never());
            Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeletePosition_DeletesPosition()
        {
            var positionId = _fixture.Create<int>();

            _positionService
                .Setup(m => m.RemovePositionAsync(positionId))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse());

            var sut = _fixture.Build<PositionsController>().OmitAutoProperties().Create();

            await sut.DeletePosition(positionId);

            _positionService.Verify(m => m.RemovePositionAsync(positionId), Times.Once());
        }
    }
}
