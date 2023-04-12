using PortEval.Domain.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Core.Interfaces.Repositories
{
    /// <summary>
    /// Represents a persistently stored collection of charts.
    /// </summary>
    public interface IChartRepository : IRepository
    {
        /// <summary>
        /// Lists all charts.
        /// </summary>
        /// <returns>A task representing the asynchronous retrieval operation. The task result contains an <c>IEnumerable</c> containing all existing charts.</returns>
        public Task<IEnumerable<Chart>> ListAllAsync();

        /// <summary>
        /// Finds a chart by ID.
        /// </summary>
        /// <param name="id">Chart ID.</param>
        /// <returns>A task representing the asynchronous search operation. The task result contains the chart entity with the supplied ID if it exists, null otherwise.</returns>
        public Task<Chart> FindAsync(int id);

        /// <summary>
        /// Adds a chart.
        /// </summary>
        /// <param name="chart">Chart entity to add.</param>
        /// <returns>The added chart entity.</returns>
        public Chart Add(Chart chart);

        /// <summary>
        /// Updates a chart.
        /// </summary>
        /// <param name="chart">Updated chart entity.</param>
        /// <returns>The updated chart.</returns>
        public Chart Update(Chart chart);

        /// <summary>
        /// Deletes a chart by ID.
        /// </summary>
        /// <param name="chartId">Id of chart to delete.</param>
        /// <returns>A task representing the asynchronous lookup and delete operations.</returns>
        public Task DeleteAsync(int chartId);

        /// <summary>
        /// Deletes a chart.
        /// </summary>
        /// <param name="chart">Chart to delete.</param>
        public void Delete(Chart chart);

        /// <summary>
        /// Checks whether a chart with the supplied id exists.
        /// </summary>
        /// <param name="id">Chart id.</param>
        /// <returns>true if a chart with the supplied id exists in the database, false otherwise</returns>
        public Task<bool> ExistsAsync(int id);
    }
}
