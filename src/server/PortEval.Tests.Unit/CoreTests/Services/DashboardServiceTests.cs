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
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.ValueObjects;
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.Services;

public class DashboardServiceTests
{
    private readonly Mock<IDashboardItemRepository> _dashboardItemRepository;
    private readonly Mock<IDashboardLayoutQueries> _dashboardQueries;
    private readonly IFixture _fixture;
    private readonly Mock<IChartRepository> _chartRepository;

    public DashboardServiceTests()
    {
        _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
        _dashboardQueries = _fixture.CreateDefaultDashboardLayoutQueriesMock();
        _chartRepository = _fixture.CreateDefaultChartRepositoryMock();
        _dashboardItemRepository = _fixture.CreateDefaultDashboardItemRepositoryMock();
    }

    [Fact]
    public async Task GetDashboardLayoutAsync_ReturnsDashboardLayout()
    {
        var items = GenerateValidDashboardItemDtos(_fixture);

        _dashboardQueries
            .Setup(m => m.GetDashboardItemsAsync())
            .ReturnsAsync(items);

        var sut = _fixture.Create<DashboardService>();

        var result = await sut.GetDashboardLayoutAsync();

        Assert.Equal(OperationStatus.Ok, result.Status);
        Assert.Equal(items, result.Response.Items);
    }

    [Fact]
    public async Task UpdatingDashboardLayout_RemovesExistingDashboardItems_WhenWellFormed()
    {
        var existingDashboardItems = GenerateValidDashboardItemEntities(_fixture);
        var newDashboardItems = GenerateValidDashboardItemDtos(_fixture);

        _dashboardItemRepository
            .Setup(r => r.GetDashboardItemsAsync())
            .ReturnsAsync(existingDashboardItems);

        var sut = _fixture.Create<DashboardService>();

        await sut.UpdateDashboardLayoutAsync(newDashboardItems);

        foreach (var item in existingDashboardItems)
        {
            _dashboardItemRepository.Verify(r => r.Delete(item), Times.Once());
        }
    }

    [Fact]
    public async Task UpdatingDashboardLayout_AddsNewDashboardItemsToRepository_WhenWellFormed()
    {
        var existingDashboardItems = GenerateValidDashboardItemEntities(_fixture);
        var newDashboardItems = GenerateValidDashboardItemDtos(_fixture);

        _dashboardItemRepository
            .Setup(r => r.GetDashboardItemsAsync())
            .ReturnsAsync(existingDashboardItems);

        var sut = _fixture.Create<DashboardService>();

        await sut.UpdateDashboardLayoutAsync(newDashboardItems);

        foreach (var item in newDashboardItems)
        {
            _dashboardItemRepository.Verify(r => r.Add(It.Is<DashboardChartItem>(i =>
                i.Position.X == item.DashboardPositionX &&
                i.Position.Y == item.DashboardPositionY &&
                i.Position.Width == item.DashboardWidth &&
                i.Position.Height == item.DashboardHeight &&
                i.ChartId == item.ChartId
            )), Times.Once());
        }
    }

    [Fact]
    public async Task UpdatingDashboardLayout_ReturnsError_WhenOverlapsAreDetected()
    {
        var existingDashboardItems = GenerateValidDashboardItemEntities(_fixture);
        var newDashboardItems = GenerateOverlappingDashboardItemDtos(_fixture);

        _dashboardItemRepository
            .Setup(r => r.GetDashboardItemsAsync())
            .ReturnsAsync(existingDashboardItems);

        var sut = _fixture.Create<DashboardService>();
        var response = await sut.UpdateDashboardLayoutAsync(newDashboardItems);

        Assert.Equal(OperationStatus.Error, response.Status);
    }

    [Fact]
    public async Task UpdatingDashboardLayout_ReturnsError_WhenItemsHaveInvalidDimensions()
    {
        var existingDashboardItems = GenerateValidDashboardItemEntities(_fixture);
        var newDashboardItems = GenerateInvalidDimensionsDashboardItemDtos(_fixture);

        _dashboardItemRepository
            .Setup(r => r.GetDashboardItemsAsync())
            .ReturnsAsync(existingDashboardItems);

        var sut = _fixture.Create<DashboardService>();
        var response = await sut.UpdateDashboardLayoutAsync(newDashboardItems);

        Assert.Equal(OperationStatus.Error, response.Status);
    }

    [Fact]
    public async Task UpdatingDashboardLayout_ReturnsError_WhenChartDoesNotExist()
    {
        var existingDashboardItems = GenerateValidDashboardItemEntities(_fixture);
        var newDashboardItems = GenerateValidDashboardItemDtos(_fixture);

        _chartRepository
            .Setup(r => r.FindAsync(It.IsAny<int>()))
            .ReturnsAsync((Chart)null);
        _chartRepository
            .Setup(r => r.ExistsAsync(It.IsAny<int>()))
            .ReturnsAsync(false);
        _dashboardItemRepository
            .Setup(r => r.GetDashboardItemsAsync())
            .ReturnsAsync(existingDashboardItems);

        var sut = _fixture.Create<DashboardService>();
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