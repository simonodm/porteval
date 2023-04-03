﻿using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Core.Queries;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Interfaces.Queries
{
    /// <summary>
    /// Implements high performance read-only chart queries.
    /// </summary>
    public interface IChartQueries
    {
        /// <summary>
        /// Retrieves all charts.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains a <see cref="QueryResponse{T}"/> wrapper over an <c>IEnumerable</c> of chart DTOs.
        /// </returns>
        public Task<QueryResponse<IEnumerable<ChartDto>>> GetCharts();

        /// <summary>
        /// Retrieves a single chart based on its ID.
        /// </summary>
        /// <param name="chartId">Parent chart ID.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains a <see cref="QueryResponse{T}"/> wrapper over a chart DTO with the specified ID if it exists, <c>null</c> otherwise.
        /// </returns>
        public Task<QueryResponse<ChartDto>> GetChart(int chartId);
    }
}
