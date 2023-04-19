using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Domain.Models.Entities;

namespace PortEval.Application.Core.Interfaces.Repositories;

/// <summary>
///     Represents a persistently stored collection of dashboard elements.
/// </summary>
public interface IDashboardItemRepository : IRepository
{
    /// <summary>
    ///     Retrieves all dashboard items.
    /// </summary>
    /// <returns>
    ///     A task representing the asynchronous retrieval operation.
    ///     Task result contains an <see cref="IEnumerable{T}" /> containing all dashboard items.
    /// </returns>
    public Task<IEnumerable<DashboardItem>> GetDashboardItemsAsync();

    /// <summary>
    ///     Adds a dashboard item to the collection.
    /// </summary>
    /// <param name="item">Dashboard item to add.</param>
    /// <returns>The added dashboard item.</returns>
    public DashboardItem Add(DashboardItem item);

    /// <summary>
    ///     Updates a dashboard item in the collection.
    /// </summary>
    /// <param name="item">Dashboard item to update.</param>
    /// <returns>The updated dashboard item.</returns>
    public DashboardItem Update(DashboardItem item);

    /// <summary>
    ///     Deletes a dashboard item.
    /// </summary>
    /// <param name="item">Dashboard item to delete.</param>
    public void Delete(DashboardItem item);
}