using System.Threading.Tasks;
using PortEval.Application.Core.Queries;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Interfaces.Queries
{
    /// <summary>
    /// Implements high performance read-only dashboard queries.
    /// </summary>
    public interface IDashboardLayoutQueries
    {
        /// <summary>
        /// Retrieves the dashboard layout of the application.
        /// </summary>
        /// <returns>A task representing the asynchronous database query operation.</returns>
        public Task<QueryResponse<DashboardLayoutDto>> GetDashboardLayout();
    }
}
