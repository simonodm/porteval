using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Interfaces.Queries;

/// <summary>
///     Implements queries for chart data stored in the application's persistent storage.
/// </summary>
public interface IChartQueries
{
    /// <summary>
    ///     Retrieves all created charts.
    /// </summary>
    /// <returns>
    ///     A task representing the asynchronous query.
    ///     Task result contains an <see cref="IEnumerable{T}" /> containing created charts.
    /// </returns>
    Task<IEnumerable<ChartDto>> GetChartsAsync();

    /// <summary>
    ///     Retrieves a chart by ID.
    /// </summary>
    /// <param name="chartId">ID of the chart to retrieve.</param>
    /// <returns>
    ///     A task representing the asynchronous query.
    ///     Task result contains the chart with the specified if it exists, <c>null</c> otherwise.
    /// </returns>
    Task<ChartDto> GetChartAsync(int chartId);
}