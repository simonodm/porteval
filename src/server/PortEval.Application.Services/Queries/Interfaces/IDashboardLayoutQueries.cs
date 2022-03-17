using PortEval.Application.Models.DTOs;
using System.Threading.Tasks;

namespace PortEval.Application.Services.Queries.Interfaces
{
    public interface IDashboardLayoutQueries
    {
        public Task<QueryResponse<DashboardLayoutDto>> GetDashboardLayout();
    }
}
