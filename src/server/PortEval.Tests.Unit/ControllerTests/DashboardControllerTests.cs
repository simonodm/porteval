using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Services.Interfaces;
using PortEval.Application.Services.Queries.Interfaces;
using PortEval.Tests.Unit.Helpers;
using System.Threading.Tasks;
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

            var dashboardLayoutQueries = fixture.Freeze<Mock<IDashboardLayoutQueries>>();
            dashboardLayoutQueries
                .Setup(m => m.GetDashboardLayout())
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(dashboardLayout));

            var sut = fixture.Build<DashboardController>().OmitAutoProperties().Create();

            var result = await sut.GetDashboardLayout();

            dashboardLayoutQueries.Verify(m => m.GetDashboardLayout(), Times.Once());
            Assert.Equal(dashboardLayout, result.Value);
        }

        [Fact]
        public async Task UpdateDashboardLayout_UpdatesLayout()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var dashboardLayout = fixture.Create<DashboardLayoutDto>();

            var dashboardService = fixture.Freeze<Mock<IDashboardService>>();

            var sut = fixture.Build<DashboardController>().OmitAutoProperties().Create();

            await sut.UpdateDashboardLayout(dashboardLayout);

            dashboardService.Verify(m => m.UpdateDashboardLayout(dashboardLayout.Items), Times.Once());
        }
    }
}
