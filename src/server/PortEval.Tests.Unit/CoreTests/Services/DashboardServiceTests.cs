using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Core;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Core.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.ValueObjects;
using PortEval.Infrastructure.Queries;
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.Services
{
    public class DashboardServiceTests
    {
        [Fact]
        public async Task GetDashboardLayoutAsync_ReturnsDashboardLayout()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var items = GenerateValidDashboardItemDtos(fixture);

            var dashboardQueries = fixture.Freeze<Mock<IDashboardLayoutQueries>>();
            dashboardQueries
                .Setup(m => m.GetDashboardItemsAsync())
                .ReturnsAsync(items);

            var sut = fixture.Create<DashboardService>();

            var result = await sut.GetDashboardLayoutAsync();

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(items, result.Response.Items);
        }

        [Fact]
        public async Task UpdatingDashboardLayout_RemovesExistingDashboardItems_WhenWellFormed()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var existingDashboardItems = GenerateValidDashboardItemEntities(fixture);
            var newDashboardItems = GenerateValidDashboardItemDtos(fixture);

            fixture.Freeze<Mock<IChartRepository>>();
            var dashboardItemRepository = fixture.Freeze<Mock<IDashboardItemRepository>>();
            dashboardItemRepository
                .Setup(r => r.GetDashboardItemsAsync())
                .ReturnsAsync(existingDashboardItems);

            var sut = fixture.Create<DashboardService>();

            await sut.UpdateDashboardLayoutAsync(newDashboardItems);

            foreach (var item in existingDashboardItems)
                dashboardItemRepository.Verify(r => r.Delete(item), Times.Once());
        }

        [Fact]
        public async Task UpdatingDashboardLayout_AddsNewDashboardItemsToRepository_WhenWellFormed()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var existingDashboardItems = GenerateValidDashboardItemEntities(fixture);
            var newDashboardItems = GenerateValidDashboardItemDtos(fixture);
            
            var dashboardItemRepository = fixture.Freeze<Mock<IDashboardItemRepository>>();
            dashboardItemRepository
                .Setup(r => r.GetDashboardItemsAsync())
                .ReturnsAsync(existingDashboardItems);
            var chartRepository = fixture.Freeze<Mock<IChartRepository>>();
            chartRepository
                .Setup(r => r.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new Chart(id, fixture.Create<string>()));

            var sut = fixture.Create<DashboardService>();

            await sut.UpdateDashboardLayoutAsync(newDashboardItems);

            foreach (var item in newDashboardItems)
                dashboardItemRepository.Verify(r => r.Add(It.Is<DashboardChartItem>(i =>
                    i.Position.X == item.DashboardPositionX &&
                    i.Position.Y == item.DashboardPositionY &&
                    i.Position.Width == item.DashboardWidth &&
                    i.Position.Height == item.DashboardHeight &&
                    i.ChartId == item.ChartId
                )), Times.Once());
        }

        [Fact]
        public async Task UpdatingDashboardLayout_ReturnsError_WhenOverlapsAreDetected()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var existingDashboardItems = GenerateValidDashboardItemEntities(fixture);
            var newDashboardItems = GenerateOverlappingDashboardItemDtos(fixture);

            fixture.Freeze<Mock<IChartRepository>>();
            var dashboardItemRepository = fixture.Freeze<Mock<IDashboardItemRepository>>();
            dashboardItemRepository
                .Setup(r => r.GetDashboardItemsAsync())
                .ReturnsAsync(existingDashboardItems);

            var sut = fixture.Create<DashboardService>();
            var response = await sut.UpdateDashboardLayoutAsync(newDashboardItems);

            Assert.Equal(OperationStatus.Error, response.Status);
        }

        [Fact]
        public async Task UpdatingDashboardLayout_ReturnsError_WhenItemsHaveInvalidDimensions()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var existingDashboardItems = GenerateValidDashboardItemEntities(fixture);
            var newDashboardItems = GenerateInvalidDimensionsDashboardItemDtos(fixture);

            fixture.Freeze<Mock<IChartRepository>>();
            var dashboardItemRepository = fixture.Freeze<Mock<IDashboardItemRepository>>();
            dashboardItemRepository
                .Setup(r => r.GetDashboardItemsAsync())
                .ReturnsAsync(existingDashboardItems);

            var sut = fixture.Create<DashboardService>();
            var response = await sut.UpdateDashboardLayoutAsync(newDashboardItems);

            Assert.Equal(OperationStatus.Error, response.Status);
        }

        [Fact]
        public async Task UpdatingDashboardLayout_ReturnsError_WhenChartDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var existingDashboardItems = GenerateValidDashboardItemEntities(fixture);
            var newDashboardItems = GenerateValidDashboardItemDtos(fixture);

            var chartRepository = fixture.Freeze<Mock<IChartRepository>>();
            chartRepository
                .Setup(r => r.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((Chart)null);
            chartRepository
                .Setup(r => r.ExistsAsync(It.IsAny<int>()))
                .ReturnsAsync(false);
            var dashboardItemRepository = fixture.Freeze<Mock<IDashboardItemRepository>>();
            dashboardItemRepository
                .Setup(r => r.GetDashboardItemsAsync())
                .ReturnsAsync(existingDashboardItems);

            var sut = fixture.Create<DashboardService>();
            var response = await sut.UpdateDashboardLayoutAsync(newDashboardItems);
            
            Assert.Equal(OperationStatus.Error, response.Status);
        }

        private List<DashboardItem> GenerateValidDashboardItemEntities(IFixture fixture)
        {
            var existingDashboardItems = new List<DashboardItem>();
            for (var i = 0; i < 3; i++)
            {
                var dashboardItem = new DashboardChartItem(fixture.Create<int>(), new DashboardPosition(i, i, 1, 1));
                existingDashboardItems.Add(dashboardItem);
            }

            return existingDashboardItems;
        }

        private List<DashboardItemDto> GenerateValidDashboardItemDtos(IFixture fixture)
        {
            var newDashboardItems = new List<DashboardItemDto>();
            for (var i = 0; i < 4; i++)
            {
                var dashboardItemDto = fixture
                    .Build<DashboardItemDto>()
                    .With(item => item.DashboardPositionX, i)
                    .With(item => item.DashboardPositionY, i)
                    .With(item => item.DashboardWidth, 2)
                    .With(item => item.DashboardHeight, 1)
                    .Create();
                newDashboardItems.Add(dashboardItemDto);
            }

            return newDashboardItems;
        }

        private List<DashboardItemDto> GenerateOverlappingDashboardItemDtos(IFixture fixture)
        {
            var newDashboardItems = new List<DashboardItemDto>();
            for (var i = 0; i < 3; i++)
            {
                var dashboardItemDto = fixture
                    .Build<DashboardItemDto>()
                    .With(item => item.DashboardPositionX, 0)
                    .With(item => item.DashboardPositionY, 0)
                    .With(item => item.DashboardWidth, i + 1)
                    .With(item => item.DashboardHeight, i + 1)
                    .Create();
                newDashboardItems.Add(dashboardItemDto);
            }

            return newDashboardItems;
        }

        private List<DashboardItemDto> GenerateInvalidDimensionsDashboardItemDtos(IFixture fixture)
        {
            var newDashboardItems = new List<DashboardItemDto>();
            for (var i = 0; i < 3; i++)
            {
                var dashboardItemDto = fixture
                    .Build<DashboardItemDto>()
                    .With(item => item.DashboardPositionX, i)
                    .With(item => item.DashboardPositionY, i)
                    .With(item => item.DashboardWidth, 0)
                    .With(item => item.DashboardHeight, 0)
                    .Create();
                newDashboardItems.Add(dashboardItemDto);
            }

            return newDashboardItems;
        }
    }
}