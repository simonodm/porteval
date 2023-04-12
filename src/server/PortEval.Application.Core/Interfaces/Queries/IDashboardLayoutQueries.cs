using PortEval.Application.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Core.Interfaces.Queries
{
    public interface IDashboardLayoutQueries
    {
        Task<IEnumerable<DashboardItemDto>> GetDashboardItemsAsync();
    }
}