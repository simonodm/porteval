using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Domain.Models.ValueObjects;
using PortEval.Infrastructure.Repositories;
using Xunit;

namespace PortEval.Tests.Integration.RepositoryTests
{
    public class ChartRepositoryTests : RepositoryTestBase
    {
        private readonly IChartRepository _chartRepository;

        public ChartRepositoryTests() : base()
        {
            _chartRepository = new ChartRepository(DbContext);
        }

        [Fact]
        public async Task ListAllAsync_ReturnsAllCharts()
        {
            DbContext.Add(new Currency("USD", "US Dollar", "$", true));

            var first = new Chart("TEST1", new ChartDateRange(new ToDateRange(DateRangeUnit.DAY, 1)),
                ChartTypeSettings.PriceChart("USD"));
            var second = new Chart("TEST2",
                new ChartDateRange(DateTime.Parse("2022-01-01"), DateTime.Parse("2022-02-01")),
                ChartTypeSettings.PerformanceChart());

            DbContext.Add(first);
            DbContext.Add(second);
            await DbContext.SaveChangesAsync();

            var charts = await _chartRepository.ListAllAsync();

            Assert.Collection(charts, chart => AssertChartsAreEqual(first, chart), chart => AssertChartsAreEqual(second, chart));
        }

        [Fact]
        public async Task FindAsync_ReturnsCorrectChart()
        {
            var chart = new Chart("TEST1", new ChartDateRange(new ToDateRange(DateRangeUnit.DAY, 1)),
                ChartTypeSettings.PerformanceChart());
            DbContext.Add(chart);
            await DbContext.SaveChangesAsync();

            var foundChart = await _chartRepository.FindAsync(chart.Id);

            AssertChartsAreEqual(foundChart, chart);
        }

        [Fact]
        public async Task Add_CreatesNewChart()
        {
            var chart = new Chart("TEST1", new ChartDateRange(new ToDateRange(DateRangeUnit.DAY, 1)),
                ChartTypeSettings.PerformanceChart());

            _chartRepository.Add(chart);
            await _chartRepository.UnitOfWork.CommitAsync();

            var createdChart = DbContext.Charts.FirstOrDefault();

            AssertChartsAreEqual(chart, createdChart);
        }

        [Fact]
        public async Task Update_UpdatesChart()
        {
            var chart = new Chart("TEST1", new ChartDateRange(new ToDateRange(DateRangeUnit.DAY, 1)),
                ChartTypeSettings.AggregatedPerformanceChart(AggregationFrequency.Day));
            DbContext.Add(chart);
            await DbContext.SaveChangesAsync();

            chart.Rename("TEST");
            chart.SetDateRange(new ChartDateRange(DateTime.Parse("2021-06-15"), DateTime.Parse("2021-08-01")));
            chart.SetConfiguration(ChartTypeSettings.PerformanceChart());

            _chartRepository.Update(chart);
            await _chartRepository.UnitOfWork.CommitAsync();

            var updatedChart = DbContext.Charts.FirstOrDefault(c => c.Id == chart.Id);

            AssertChartsAreEqual(updatedChart, chart);
        }

        [Fact]
        public async Task DeleteAsync_DeletesChart()
        {
            var chart = new Chart("TEST1", new ChartDateRange(new ToDateRange(DateRangeUnit.DAY, 1)),
                ChartTypeSettings.AggregatedPerformanceChart(AggregationFrequency.Day));
            DbContext.Add(chart);
            await DbContext.SaveChangesAsync();

            await _chartRepository.DeleteAsync(chart.Id);
            await _chartRepository.UnitOfWork.CommitAsync();

            var chartDeleted = !DbContext.Charts.Any();

            Assert.True(chartDeleted);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenChartExists()
        {
            var chart = new Chart("TEST1", new ChartDateRange(new ToDateRange(DateRangeUnit.DAY, 1)),
                ChartTypeSettings.AggregatedPerformanceChart(AggregationFrequency.Day));
            DbContext.Charts.Add(chart);
            await DbContext.SaveChangesAsync();

            var exists = await _chartRepository.ExistsAsync(chart.Id);

            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsFalse_WhenChartDoesNotExist()
        {
            var exists = await _chartRepository.ExistsAsync(1);

            Assert.False(exists);
        }

        private void AssertChartsAreEqual(Chart expected, Chart actual)
        {
            Assert.Equal(expected?.Name, actual?.Name);
            Assert.Equal(expected?.DateRange.IsToDate, actual?.DateRange.IsToDate);
            Assert.Equal(expected?.DateRange.Start, actual?.DateRange.Start);
            Assert.Equal(expected?.DateRange.End, actual?.DateRange.End);
            Assert.Equal(expected?.TypeConfiguration.Type, actual?.TypeConfiguration.Type);
            Assert.Equal(expected?.TypeConfiguration.CurrencyCode, actual?.TypeConfiguration.CurrencyCode);
            Assert.Equal(expected?.TypeConfiguration.Frequency, actual?.TypeConfiguration.Frequency);
            Assert.Equal(expected?.Lines, actual?.Lines);
        }
    }
}
