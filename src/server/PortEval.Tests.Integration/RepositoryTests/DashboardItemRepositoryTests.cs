using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.ValueObjects;
using PortEval.Infrastructure.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Repositories;
using Xunit;

namespace PortEval.Tests.Integration.RepositoryTests
{
    public class DashboardItemRepositoryTests : RepositoryTestBase
    {
        private readonly IDashboardItemRepository _dashboardItemRepository;

        public DashboardItemRepositoryTests() : base()
        {
            _dashboardItemRepository = new DashboardItemRepository(DbContext);
        }

        [Fact]
        public async Task GetDashboardItemsAsync_ReturnsAllDashboardItems()
        {
            var chart = await CreateTestChart();

            var first = new DashboardChartItem(chart.Id, new DashboardPosition(0, 0, 1, 1));
            var second = new DashboardChartItem(chart.Id, new DashboardPosition(2, 2, 1, 1));

            DbContext.Add(first);
            DbContext.Add(second);
            await DbContext.SaveChangesAsync();

            var dashboardItems = await _dashboardItemRepository.GetDashboardItemsAsync();

            Assert.Collection(dashboardItems, item => AssertDashboardItemsAreEqual(first, item),
                item => AssertDashboardItemsAreEqual(second, item));
        }

        [Fact]
        public async Task Add_CreatesDashboardItem()
        {
            var chart = await CreateTestChart();
            var dashboardItem = new DashboardChartItem(chart.Id, new DashboardPosition(0, 0, 1, 1));

            _dashboardItemRepository.Add(dashboardItem);
            await _dashboardItemRepository.UnitOfWork.CommitAsync();

            var createdItem = DbContext.DashboardItems.FirstOrDefault();

            AssertDashboardItemsAreEqual(dashboardItem, createdItem);
        }

        [Fact]
        public async Task Update_UpdatesDashboardItem()
        {
            var chart = await CreateTestChart();
            var item = new DashboardChartItem(chart.Id, new DashboardPosition(0, 0, 1, 1));
            DbContext.Add(item);
            await DbContext.SaveChangesAsync();

            item.SetPosition(new DashboardPosition(1, 1, 2, 2));
            _dashboardItemRepository.Update(item);
            await _dashboardItemRepository.UnitOfWork.CommitAsync();

            var updatedItem = DbContext.DashboardItems.FirstOrDefault();

            AssertDashboardItemsAreEqual(item, updatedItem);
        }

        [Fact]
        public async Task Delete_DeletesDashboardItem()
        {
            var chart = await CreateTestChart();
            var item = new DashboardChartItem(chart.Id, new DashboardPosition(0, 0, 1, 1));
            DbContext.DashboardItems.Add(item);
            await DbContext.SaveChangesAsync();

            _dashboardItemRepository.Delete(item);
            await _dashboardItemRepository.UnitOfWork.CommitAsync();

            var isEmpty = !DbContext.DashboardItems.Any();

            Assert.True(isEmpty);
        }

        private async Task<Chart> CreateTestChart()
        {
            var chart = new Chart(Guid.NewGuid().ToString());
            DbContext.Add(chart);
            await DbContext.SaveChangesAsync();

            return chart;
        }

        private void AssertDashboardItemsAreEqual(DashboardChartItem expected, DashboardItem actual)
        {
            Assert.IsType<DashboardChartItem>(actual);
            Assert.Equal(expected?.Id, actual.Id);
            Assert.Equal(expected?.ChartId, ((DashboardChartItem)actual).ChartId);
            Assert.Equal(expected?.Position.X, actual.Position.X);
            Assert.Equal(expected?.Position.Y, actual.Position.Y);
            Assert.Equal(expected?.Position.Width, actual.Position.Width);
            Assert.Equal(expected?.Position.Height, actual.Position.Height);
        }
    }
}
