using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Tests.Unit.Helpers;
using System.Threading.Tasks;
using PortEval.Domain.Models.Entities;
using Xunit;

namespace PortEval.Tests.Unit.ControllerTests
{
    public class DashboardControllerTests
    {
        [Fact]
        public async Task GetDashboardLayout_ReturnsDashboardLayout()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var dashboardLayout = fixture.Create<DashboardLayoutDto>();

            var dashboardLayoutService = fixture.Freeze<Mock<IDashboardService>>();
            dashboardLayoutService
                .Setup(m => m.GetDashboardLayoutAsync())
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(dashboardLayout));

            var sut = fixture.Build<DashboardController>().OmitAutoProperties().Create();

            var result = await sut.GetDashboardLayout();

            dashboardLayoutService.Verify(m => m.GetDashboardLayoutAsync(), Times.Once());
            Assert.Equal(dashboardLayout, result.Value);
        }

        [Fact]
        public async Task UpdateDashboardLayout_UpdatesLayout()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var dashboardLayout = fixture.Create<DashboardLayoutDto>();

            var dashboardService = fixture.Freeze<Mock<IDashboardService>>();
            dashboardService
                .Setup(m => m.UpdateDashboardLayoutAsync(It.IsAny<IEnumerable<DashboardItemDto>>()))
                .Returns<IEnumerable<DashboardItemDto>>(items =>
                    Task.FromResult(OperationResponseHelper.GenerateSuccessfulOperationResponse(
                        new DashboardLayoutDto
                        {
                            Items = items.ToList()
                        })));

            var sut = fixture.Build<DashboardController>().OmitAutoProperties().Create();

            await sut.UpdateDashboardLayout(dashboardLayout);

            dashboardService.Verify(m => m.UpdateDashboardLayoutAsync(dashboardLayout.Items), Times.Once());
        }
    }
}
