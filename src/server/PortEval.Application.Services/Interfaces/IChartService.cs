using PortEval.Application.Models.DTOs;
using PortEval.Domain.Models.Entities;
using System.Threading.Tasks;

namespace PortEval.Application.Services.Interfaces
{
    /// <summary>
    /// Handles creation, modification and removal of charts.
    /// </summary>
    public interface IChartService
    {
        /// <summary>
        /// Creates a chart according to the supplied DTO.
        /// </summary>
        /// <param name="options">A DTO containing client's request body.</param>
        /// <returns>A task representing the asynchronous creation operation. Task result contains newly created chart entity.</returns>
        public Task<Chart> CreateChartAsync(ChartDto options);

        /// <summary>
        /// Modifies a chart according to the supplied DTO.
        /// </summary>
        /// <param name="options">A DTO containing client's request body.</param>
        /// <returns>A task representing the asynchronous update operation. Task result contains the updated chart entity.</returns>
        public Task<Chart> UpdateChartAsync(ChartDto options);

        /// <summary>
        /// Deletes a chart.
        /// </summary>
        /// <param name="id">Chart ID.</param>
        /// <returns>A task representing the asynchronous deletion operation.</returns>
        public Task DeleteChartAsync(int id);
    }
}
