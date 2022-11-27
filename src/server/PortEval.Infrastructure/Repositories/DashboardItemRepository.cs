using Microsoft.EntityFrameworkCore;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Infrastructure.Repositories
{
    public class DashboardItemRepository : IDashboardItemRepository
    {
        public IUnitOfWork UnitOfWork => _context;
        private readonly PortEvalDbContext _context;

        public DashboardItemRepository(PortEvalDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DashboardItem>> GetDashboardItemsAsync()
        {
            return await _context.DashboardItems
                .AsNoTracking()
                .OrderBy(i => i.Position.X)
                .ThenBy(i => i.Position.Y)
                .ToListAsync();
        }

        public DashboardItem Add(DashboardItem item)
        {
            return _context.DashboardItems.Add(item).Entity;
        }

        public DashboardItem Update(DashboardItem item)
        {
            return _context.DashboardItems.Update(item).Entity;
        }

        public async Task DeleteAsync(DashboardItem item)
        {
            var existingItem = await _context.DashboardItems.FirstOrDefaultAsync(i => i.Id == item.Id);
            if (existingItem != null)
            {
                _context.DashboardItems.Remove(existingItem);
            }
        }
    }
}
