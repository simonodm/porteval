using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Interfaces.Services
{
    /// <summary>
    /// Handles application dashboard operations.
    /// </summary>
    public interface IDashboardService
    {
        /// <summary>
        /// Retrieves the current dashboard layout.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing the current dashboard layout.
        /// </returns>
        public Task<OperationResponse<DashboardLayoutDto>> GetDashboardLayoutAsync();

        /// <summary>
        /// Replaces the existing dashboard layout with a layout consisting of provided dashboard items.
        /// </summary>
        /// <param name="newItems">Items contained in the new dashboard layout.</param>
        /// <returns>
        /// A task representing the asynchronous update operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing the updated dashboard layout.
        /// </returns>
        public Task<OperationResponse<DashboardLayoutDto>> UpdateDashboardLayoutAsync(IEnumerable<DashboardItemDto> newItems);
    }
}
