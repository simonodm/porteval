using PortEval.Application.Features.Queries;
using PortEval.Application.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Interfaces.Queries
{
    /// <summary>
    /// High performance read-only chart queries.
    /// </summary>
    public interface IChartQueries
    {
        /// <summary>
        /// Retrieves all charts.
        /// </summary>
        /// <returns>A task representing the asynchronous database query. Task result contains an <c>IEnumerable</c> of chart DTOs.</returns>
        public Task<QueryResponse<IEnumerable<ChartDto>>> GetCharts();

        /// <summary>
        /// Retrieves a single chart based on its ID.
        /// </summary>
        /// <param name="chartId">Parent chart ID.</param>
        /// <returns>
        /// A task representing the asynchronous database query. Task result contains a chart DTO with the specified ID if it exists, <c>null</c> otherwise.
        /// </returns>
        public Task<QueryResponse<ChartDto>> GetChart(int chartId);
    }
}
