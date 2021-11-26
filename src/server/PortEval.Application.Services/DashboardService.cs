using PortEval.Application.Models.DTOs;
using PortEval.Application.Services.Interfaces;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IChartRepository _chartRepository;
        private readonly IDashboardItemRepository _dashboardItemRepository;

        public DashboardService(IDashboardItemRepository dashboardItemRepository, IChartRepository chartRepository)
        {
            _dashboardItemRepository = dashboardItemRepository;
            _chartRepository = chartRepository;
        }

        public async Task UpdateDashboardLayout(IEnumerable<DashboardItemDto> newItems)
        {
            var existingItems = await _dashboardItemRepository.GetDashboardItems();

            foreach (var existingItem in existingItems)
            {
                var matchingItem = newItems.FirstOrDefault(item => ItemsAreSame(item, existingItem));
                if (matchingItem == null)
                {
                    await _dashboardItemRepository.Remove(existingItem);
                }
                else
                {
                    var position = new DashboardPosition(matchingItem.DashboardPositionX, matchingItem.DashboardPositionY, matchingItem.DashboardWidth, matchingItem.DashboardHeight);
                    existingItem.SetPosition(position);
                    await _dashboardItemRepository.Update(existingItem);
                }
            }

            foreach (var newItem in newItems)
            {
                var existingItem = existingItems.FirstOrDefault(item => ItemsAreSame(newItem, item));
                if (existingItem == null)
                {
                    await ValidateChartExists(newItem.ChartId);
                    var position = new DashboardPosition(newItem.DashboardPositionX, newItem.DashboardPositionY, newItem.DashboardWidth, newItem.DashboardHeight);
                    var item = new DashboardChartItem(newItem.ChartId, position);
                    _dashboardItemRepository.Add(item);
                }
            }

            await _dashboardItemRepository.UnitOfWork.CommitAsync();
        }

        private async Task ValidateChartExists(int chartId)
        {
            if (!(await _chartRepository.Exists(chartId)))
            {
                throw new ItemNotFoundException($"Chart {chartId} does not exist.");
            }
        }

        private bool ItemsAreSame(DashboardItemDto itemDto, DashboardItem itemEntity)
        {
            return itemEntity is DashboardChartItem dashboardChartItem && itemDto.ChartId == dashboardChartItem.ChartId;
        }
    }
}
