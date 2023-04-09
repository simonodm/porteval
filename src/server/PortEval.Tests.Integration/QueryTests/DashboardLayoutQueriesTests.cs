﻿using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Queries;
using Xunit;
using PortEval.Application.Core;

namespace PortEval.Tests.Integration.QueryTests
{
    [Collection("Query test collection")]
    public class DashboardLayoutQueriesTests
    {
        private readonly IDashboardLayoutQueries _dashboardLayoutQueries;

        public DashboardLayoutQueriesTests(QueryTestFixture fixture)
        {
            var scope = fixture.Factory.Services.CreateScope();
            _dashboardLayoutQueries = scope.ServiceProvider.GetRequiredService<IDashboardLayoutQueries>();
        }

        [Fact]
        public async Task GetDashboardLayout_ReturnsDashboardLayoutFromDb()
        {
            var queryResult = await _dashboardLayoutQueries.GetDashboardLayout();

            Assert.Equal(OperationStatus.Ok, queryResult.Status);
            Assert.Equal(2, queryResult.Response.Items.Count);
            Assert.Contains(queryResult.Response.Items, i =>
                i.DashboardPositionX == 0 &&
                i.DashboardPositionY == 0 &&
                i.DashboardHeight == 1 &&
                i.DashboardWidth == 1 &&
                i.ChartId != default
            );
            Assert.Contains(queryResult.Response.Items, i =>
                i.DashboardPositionX == 1 &&
                i.DashboardPositionY == 1 &&
                i.DashboardHeight == 2 &&
                i.DashboardWidth == 2 &&
                i.ChartId != default
            );
        }
    }
}
