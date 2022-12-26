using PortEval.Application.Features.Interfaces.Repositories;
using PortEval.Application.Features.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Services
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
            var existingItems = await _dashboardItemRepository.GetDashboardItemsAsync();

            foreach (var existingItem in existingItems)
            {
                _dashboardItemRepository.Delete(existingItem);
            }

            var newItemEntities = await GenerateItemEntities(newItems);
            if (OverlapsExist(newItemEntities))
            {
                throw new OperationNotAllowedException("Dashboard layout overlaps detected.");
            }

            foreach (var entity in newItemEntities)
            {
                _dashboardItemRepository.Add(entity);
            }

            await _dashboardItemRepository.UnitOfWork.CommitAsync();
        }

        private async Task ValidateChartExists(int chartId)
        {
            if (!await _chartRepository.ExistsAsync(chartId))
            {
                throw new ItemNotFoundException($"Chart {chartId} does not exist.");
            }
        }

        private async Task<List<DashboardItem>> GenerateItemEntities(IEnumerable<DashboardItemDto> items)
        {
            var result = new List<DashboardItem>();
            foreach (var item in items)
            {
                await ValidateChartExists(item.ChartId);
                var position = new DashboardPosition(item.DashboardPositionX, item.DashboardPositionY, item.DashboardWidth, item.DashboardHeight);
                result.Add(DashboardChartItem.Create(item.ChartId, position));
            }

            return result;
        }

        private bool OverlapsExist(IEnumerable<DashboardItem> items)
        {
            var filledLayoutPositions = new HashSet<(int, int)>();

            foreach (var item in items)
            {
                for (int i = item.Position.X; i < item.Position.X + item.Position.Width; i++)
                {
                    for (int j = item.Position.Y; j < item.Position.Y + item.Position.Height; j++)
                    {
                        if (filledLayoutPositions.Contains((i, j)))
                        {
                            return true;
                        }
                        filledLayoutPositions.Add((i, j));
                    }
                }
            }

            return false;
        }
    }
}
