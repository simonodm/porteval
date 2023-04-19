using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Tests.Unit.Helpers;
using Xunit;

namespace PortEval.Tests.Unit.ControllerTests;

public class DashboardControllerTests
{
    private readonly Mock<IDashboardService> _dashboardLayoutService;
    private readonly IFixture _fixture;

    public DashboardControllerTests()
    {
        _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
        _dashboardLayoutService = _fixture.Freeze<Mock<IDashboardService>>();
    }

    [Fact]
    public async Task GetDashboardLayout_ReturnsDashboardLayout()
    {
        var dashboardLayout = _fixture.Create<DashboardLayoutDto>();

        _dashboardLayoutService
            .Setup(m => m.GetDashboardLayoutAsync())
            .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(dashboardLayout));

        var sut = _fixture.Build<DashboardController>().OmitAutoProperties().Create();

        var result = await sut.GetDashboardLayout();

        _dashboardLayoutService.Verify(m => m.GetDashboardLayoutAsync(), Times.Once());
        Assert.Equal(dashboardLayout, result.Value);
    }

    [Fact]
    public async Task UpdateDashboardLayout_UpdatesLayout()
    {
        var dashboardLayout = _fixture.Create<DashboardLayoutDto>();

        _dashboardLayoutService
            .Setup(m => m.UpdateDashboardLayoutAsync(It.IsAny<IEnumerable<DashboardItemDto>>()))
            .Returns<IEnumerable<DashboardItemDto>>(items =>
                Task.FromResult(OperationResponseHelper.GenerateSuccessfulOperationResponse(
                    new DashboardLayoutDto
                    {
                        Items = items.ToList()
                    })));

        var sut = _fixture.Build<DashboardController>().OmitAutoProperties().Create();

        await sut.UpdateDashboardLayout(dashboardLayout);

        _dashboardLayoutService.Verify(m => m.UpdateDashboardLayoutAsync(dashboardLayout.Items), Times.Once());
    }
}