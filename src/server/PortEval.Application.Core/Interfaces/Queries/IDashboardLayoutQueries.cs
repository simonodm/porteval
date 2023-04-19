using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Interfaces.Queries;

/// <summary>
///     Implements queries for dashboard data stored in the application's persistent storage.
/// </summary>
public interface IDashboardLayoutQueries
{
    /// <summary>
    ///     Retrieves all dashboard items.
    /// </summary>
    /// <returns>
    ///     A task representing the asynchronous query operation.
    ///     Task result contains an <see cref="IEnumerable{T}" /> containing dashboard items.
    /// </returns>
    Task<IEnumerable<DashboardItemDto>> GetDashboardItemsAsync();
}