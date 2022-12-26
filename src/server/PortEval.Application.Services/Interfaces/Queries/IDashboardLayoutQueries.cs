using PortEval.Application.Features.Queries;
using PortEval.Application.Models.DTOs;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Interfaces.Queries
{
    public interface IDashboardLayoutQueries
    {
        public Task<QueryResponse<DashboardLayoutDto>> GetDashboardLayout();
    }
}
