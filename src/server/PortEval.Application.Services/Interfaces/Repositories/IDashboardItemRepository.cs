using PortEval.Domain.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Services.Interfaces.Repositories
{
    public interface IDashboardItemRepository : IRepository
    {
        public Task<IEnumerable<DashboardItem>> GetDashboardItems();
        public DashboardItem Add(DashboardItem item);
        public DashboardItem Update(DashboardItem item);
        public Task Remove(DashboardItem item);
    }
}
