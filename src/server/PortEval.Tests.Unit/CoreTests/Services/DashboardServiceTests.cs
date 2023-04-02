using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.ValueObjects;
using PortEval.Tests.Unit.Helpers.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Core.Services;
using Xunit;

namespace PortEval.Tests.Unit.FeatureTests.Services
{
    public class DashboardServiceTests
    {
        [Fact]
        public async Task UpdatingDashboardLayout_RemovesExistingDashboardItems_WhenWellFormed()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var existingDashboardItems = GenerateValidDashboardItemEntities(fixture);
            var newDashboardItems = GenerateValidDashboardItemDtos(fixture);

            fixture.CreateDefaultChartRepositoryMock();
            var dashboardItemRepository = fixture.CreateDefaultDashboardItemRepositoryMock();
            dashboardItemRepository
                .Setup(r => r.GetDashboardItemsAsync())
                .ReturnsAsync(existingDashboardItems);

            var sut = fixture.Create<DashboardService>();

            await sut.UpdateDashboardLayout(newDashboardItems);

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

            fixture.CreateDefaultChartRepositoryMock();
            var dashboardItemRepository = fixture.CreateDefaultDashboardItemRepositoryMock();
            dashboardItemRepository
                .Setup(r => r.GetDashboardItemsAsync())
                .ReturnsAsync(existingDashboardItems);

            var sut = fixture.Create<DashboardService>();

            await sut.UpdateDashboardLayout(newDashboardItems);

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
        public async Task UpdatingDashboardLayout_ThrowsException_WhenOverlapsAreDetected()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var existingDashboardItems = GenerateValidDashboardItemEntities(fixture);
            var newDashboardItems = GenerateOverlappingDashboardItemDtos(fixture);

            fixture.CreateDefaultChartRepositoryMock();
            var dashboardItemRepository = fixture.CreateDefaultDashboardItemRepositoryMock();
            dashboardItemRepository
                .Setup(r => r.GetDashboardItemsAsync())
                .ReturnsAsync(existingDashboardItems);

            var sut = fixture.Create<DashboardService>();

            await Assert.ThrowsAsync<OperationNotAllowedException>(async () =>
                await sut.UpdateDashboardLayout(newDashboardItems));
        }

        [Fact]
        public async Task UpdatingDashboardLayout_ThrowsException_WhenItemsHaveInvalidDimensions()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var existingDashboardItems = GenerateValidDashboardItemEntities(fixture);
            var newDashboardItems = GenerateInvalidDimensionsDashboardItemDtos(fixture);

            fixture.CreateDefaultChartRepositoryMock();
            var dashboardItemRepository = fixture.CreateDefaultDashboardItemRepositoryMock();
            dashboardItemRepository
                .Setup(r => r.GetDashboardItemsAsync())
                .ReturnsAsync(existingDashboardItems);

            var sut = fixture.Create<DashboardService>();

            await Assert.ThrowsAsync<OperationNotAllowedException>(async () =>
                await sut.UpdateDashboardLayout(newDashboardItems));
        }

        [Fact]
        public async Task UpdatingDashboardLayout_ThrowsException_WhenChartDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var existingDashboardItems = GenerateValidDashboardItemEntities(fixture);
            var newDashboardItems = GenerateValidDashboardItemDtos(fixture);

            var chartRepository = fixture.CreateDefaultChartRepositoryMock();
            chartRepository
                .Setup(r => r.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((Chart)null);
            chartRepository
                .Setup(r => r.ExistsAsync(It.IsAny<int>()))
                .ReturnsAsync(false);
            var dashboardItemRepository = fixture.CreateDefaultDashboardItemRepositoryMock();
            dashboardItemRepository
                .Setup(r => r.GetDashboardItemsAsync())
                .ReturnsAsync(existingDashboardItems);

            var sut = fixture.Create<DashboardService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () =>
                await sut.UpdateDashboardLayout(newDashboardItems));
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