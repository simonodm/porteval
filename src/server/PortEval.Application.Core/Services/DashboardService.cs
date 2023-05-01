using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.ValueObjects;

namespace PortEval.Application.Core.Services;

/// <inheritdoc cref="IDashboardService" />
public class DashboardService : IDashboardService
{
    private readonly IDashboardItemRepository _dashboardItemRepository;
    private readonly IDashboardLayoutQueries _dashboardLayoutDataQueries;
    private readonly IChartRepository _chartRepository;

    /// <summary>
    ///     Initializes the service.
    /// </summary>
    public DashboardService(IDashboardItemRepository dashboardItemRepository, IChartRepository chartRepository,
        IDashboardLayoutQueries dashboardLayoutDataQueries)
    {
        _dashboardItemRepository = dashboardItemRepository;
        _chartRepository = chartRepository;
        _dashboardLayoutDataQueries = dashboardLayoutDataQueries;
    }

    /// <inheritdoc />
    public async Task<OperationResponse<DashboardLayoutDto>> GetDashboardLayoutAsync()
    {
        var items = await _dashboardLayoutDataQueries.GetDashboardItemsAsync();

        return new OperationResponse<DashboardLayoutDto>
        {
            Response = new DashboardLayoutDto
            {
                Items = items.ToList()
            }
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<DashboardLayoutDto>> UpdateDashboardLayoutAsync(
        IEnumerable<DashboardItemDto> newItems)
    {
        var existingItems = await _dashboardItemRepository.GetDashboardItemsAsync();

        foreach (var existingItem in existingItems) _dashboardItemRepository.Delete(existingItem);

        var newItemEntities = await GenerateItemEntities(newItems);
        if (newItemEntities.Status != OperationStatus.Ok)
        {
            return new OperationResponse<DashboardLayoutDto>
            {
                Status = newItemEntities.Status,
                Message = newItemEntities.Message
            };
        }

        if (OverlapsExist(newItemEntities.Response))
        {
            return new OperationResponse<DashboardLayoutDto>
            {
                Status = OperationStatus.Error,
                Message = "Dashboard layout overlaps detected."
            };
        }

        foreach (var entity in newItemEntities.Response) _dashboardItemRepository.Add(entity);

        await _dashboardItemRepository.UnitOfWork.CommitAsync();
        return await GetDashboardLayoutAsync();
    }

    private async Task<OperationResponse<List<DashboardItem>>> GenerateItemEntities(IEnumerable<DashboardItemDto> items)
    {
        var result = new List<DashboardItem>();
        try
        {
            foreach (var item in items)
            {
                var chart = await _chartRepository.FindAsync(item.ChartId);
                if (chart == null)
                {
                    return new OperationResponse<List<DashboardItem>>
                    {
                        Status = OperationStatus.Error,
                        Message = $"Chart {item.ChartId} does not exist."
                    };
                }

                var position = new DashboardPosition(item.DashboardPositionX, item.DashboardPositionY,
                    item.DashboardWidth, item.DashboardHeight);
                result.Add(DashboardChartItem.Create(chart, position));
            }
        }
        catch (PortEvalException ex)
        {
            return new OperationResponse<List<DashboardItem>>
            {
                Status = OperationStatus.Error,
                Message = ex.Message
            };
        }

        return new OperationResponse<List<DashboardItem>>
        {
            Response = result
        };
    }

    private bool OverlapsExist(IEnumerable<DashboardItem> items)
    {
        var filledLayoutPositions = new HashSet<(int, int)>();

        foreach (var item in items)
            for (var i = item.Position.X; i < item.Position.X + item.Position.Width; i++)
            for (var j = item.Position.Y; j < item.Position.Y + item.Position.Height; j++)
            {
                if (filledLayoutPositions.Contains((i, j)))
                {
                    return true;
                }

                filledLayoutPositions.Add((i, j));
            }

        return false;
    }
}