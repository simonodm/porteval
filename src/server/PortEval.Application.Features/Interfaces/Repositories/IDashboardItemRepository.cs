using PortEval.Domain.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Interfaces.Repositories
{
    public interface IDashboardItemRepository : IRepository
    {
        public Task<IEnumerable<DashboardItem>> GetDashboardItemsAsync();
        public DashboardItem Add(DashboardItem item);
        public DashboardItem Update(DashboardItem item);
        public void Delete(DashboardItem item);
    }
}
