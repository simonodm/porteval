using PortEval.Application.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Core.Interfaces.Services
{
    /// <summary>
    /// Handles creation, modification and removal of charts.
    /// </summary>
    public interface IChartService
    {
        /// <summary>
        /// Retrieves all created charts.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing all created charts.
        /// </returns>
        public Task<OperationResponse<IEnumerable<ChartDto>>> GetAllChartsAsync();

        /// <summary>
        /// Retrieves a chart by ID.
        /// </summary>
        /// <param name="chartId">ID of the chart to retrieve.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing the retrieved chart, if it exists.
        /// </returns>
        public Task<OperationResponse<ChartDto>> GetChartAsync(int chartId);

        /// <summary>
        /// Creates a chart according to the supplied DTO.
        /// </summary>
        /// <param name="options">A DTO containing client's request body.</param>
        /// <returns>
        /// A task representing the asynchronous creation operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing the newly created chart.
        /// </returns>
        public Task<OperationResponse<ChartDto>> CreateChartAsync(ChartDto options);

        /// <summary>
        /// Modifies a chart according to the supplied DTO.
        /// </summary>
        /// <param name="options">A DTO containing client's request body.</param>
        /// <returns>
        /// A task representing the asynchronous update operation.
        /// Task result contains an <see cref="OperationResponse{T}"/> containing the updated chart, if it exists.
        /// </returns>
        public Task<OperationResponse<ChartDto>> UpdateChartAsync(ChartDto options);

        /// <summary>
        /// Deletes a chart.
        /// </summary>
        /// <param name="id">Chart ID.</param>
        /// <returns>
        /// A task representing the asynchronous deletion operation.
        /// Task result contains an <see cref="OperationResponse"/> representing the status of the operation.
        /// </returns>
        public Task<OperationResponse> DeleteChartAsync(int id);
    }
}
