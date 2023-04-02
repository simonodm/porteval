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
        /// Replaces the existing dashboard layout with a layout consisting of provided dashboard items.
        /// </summary>
        /// <param name="newItems">Items contained in the new dashboard layout.</param>
        /// <returns>A task representing the asynchronous update operation.</returns>
        public Task UpdateDashboardLayout(IEnumerable<DashboardItemDto> newItems);
    }
}
