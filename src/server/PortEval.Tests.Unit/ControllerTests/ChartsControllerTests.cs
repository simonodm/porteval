﻿using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Services.Interfaces;
using PortEval.Application.Services.Queries.Interfaces;
using PortEval.Domain.Models.Entities;
using PortEval.Tests.Unit.Helpers;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.ControllerTests
{
    public class ChartsControllerTests
    {
        [Fact]
        public async Task GetAllCharts_ReturnsChartsFromQueries()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var charts = fixture.CreateMany<ChartDto>();

            var chartQueries = fixture.Freeze<Mock<IChartQueries>>();
            chartQueries
                .Setup(m => m.GetCharts())
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(charts));
            var sut = fixture.Build<ChartsController>().OmitAutoProperties().Create();

            var result = await sut.GetAllCharts();

            chartQueries.Verify(m => m.GetCharts(), Times.Once());
            Assert.Equal(charts, result.Value);
        }

        [Fact]
        public async Task GetChart_ReturnsCorrectChartFromQueries_WhenChartExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Create<ChartDto>();

            var chartQueries = fixture.Freeze<Mock<IChartQueries>>();
            chartQueries
                .Setup(m => m.GetChart(chart.Id))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(chart));

            var sut = fixture.Build<ChartsController>().OmitAutoProperties().Create();

            var result = await sut.GetChart(chart.Id);

            chartQueries.Verify(m => m.GetChart(chart.Id), Times.Once());
            Assert.Equal(chart, result.Value);
        }

        [Fact]
        public async Task GetChart_ReturnsNotFound_WhenChartDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chartId = fixture.Create<int>();

            var chartQueries = fixture.Freeze<Mock<IChartQueries>>();
            chartQueries
                .Setup(m => m.GetChart(It.IsAny<int>()))
                .ReturnsAsync(ControllerTestHelper.GenerateNotFoundQueryResponse<ChartDto>());

            var sut = fixture.Build<ChartsController>().OmitAutoProperties().Create();

            var result = await sut.GetChart(chartId);

            chartQueries.Verify(m => m.GetChart(chartId), Times.Once());
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task PostChart_CreatesChartUsingService()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Create<ChartDto>();

            var chartService = fixture.Freeze<Mock<IChartService>>();
            chartService
                .Setup(m => m.CreateChartAsync(chart))
                .ReturnsAsync(fixture.Create<Chart>());
            var sut = fixture.Build<ChartsController>().OmitAutoProperties().Create();

            await sut.PostChart(chart);

            chartService.Verify(m => m.CreateChartAsync(chart), Times.Once());
        }

        [Fact]
        public async Task PutChart_UpdatesChartUsingService()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Create<ChartDto>();

            var chartService = fixture.Freeze<Mock<IChartService>>();
            var sut = fixture.Build<ChartsController>().OmitAutoProperties().Create();

            await sut.PutChart(chart.Id, chart);

            chartService.Verify(m => m.UpdateChartAsync(chart), Times.Once());
        }

        [Fact]
        public async Task PutChart_ReturnsBadRequest_WhenQueryParameterIdAndBodyIdDontMatch()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chart = fixture.Create<ChartDto>();

            var chartService = fixture.Freeze<Mock<IChartService>>();
            var sut = fixture.Build<ChartsController>().OmitAutoProperties().Create();

            var result = await sut.PutChart(chart.Id + 1, chart);

            chartService.Verify(m => m.UpdateChartAsync(chart), Times.Never());
            Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeleteChart_DeletesChartUsingService()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var chartId = fixture.Create<int>();

            var chartService = fixture.Freeze<Mock<IChartService>>();
            var sut = fixture.Build<ChartsController>().OmitAutoProperties().Create();

            await sut.DeleteChart(chartId);

            chartService.Verify(m => m.DeleteChartAsync(chartId), Times.Once());
        }
    }
}
