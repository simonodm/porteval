using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Interfaces.DataQueries
{
    public interface IDashboardLayoutDataQueries
    {
        Task<IEnumerable<DashboardItemDto>> GetDashboardItemsAsync();
    }
}