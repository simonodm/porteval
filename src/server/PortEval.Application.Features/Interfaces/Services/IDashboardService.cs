using PortEval.Application.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Interfaces.Services
{
    public interface IDashboardService
    {
        public Task UpdateDashboardLayout(IEnumerable<DashboardItemDto> newItems);
    }
}
