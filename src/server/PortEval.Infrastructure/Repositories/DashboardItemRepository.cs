using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;

namespace PortEval.Infrastructure.Repositories;

/// <inheritdoc cref="IDashboardItemRepository" />
public class DashboardItemRepository : IDashboardItemRepository
{
    private readonly PortEvalDbContext _context;

    public DashboardItemRepository(PortEvalDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public IUnitOfWork UnitOfWork => _context;

    /// <inheritdoc />
    public async Task<IEnumerable<DashboardItem>> GetDashboardItemsAsync()
    {
        return await _context.DashboardItems
            .OrderBy(i => i.Position.X)
            .ThenBy(i => i.Position.Y)
            .ToListAsync();
    }

    /// <inheritdoc />
    public DashboardItem Add(DashboardItem item)
    {
        return _context.DashboardItems.Add(item).Entity;
    }

    /// <inheritdoc />
    public DashboardItem Update(DashboardItem item)
    {
        return _context.DashboardItems.Update(item).Entity;
    }

    /// <inheritdoc />
    public void Delete(DashboardItem item)
    {
        _context.DashboardItems.Remove(item);
    }
}