using PortEval.Application.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Services.Interfaces
{
    public interface IDashboardService
    {
        public Task UpdateDashboardLayout(IEnumerable<DashboardItemDto> newItems);
    }
}
