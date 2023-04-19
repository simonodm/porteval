using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Interfaces.Queries;

/// <summary>
///     Implements queries for positions stored in the application's persistent storage.
/// </summary>
public interface IPositionQueries
{
    /// <summary>
    ///     Retrieves all created positions.
    /// </summary>
    /// <returns>
    ///     A task representing the asynchronous query operation.
    ///     Task result contains an <see cref="IEnumerable{T}" /> containing stored positions.
    /// </returns>
    Task<IEnumerable<PositionDto>> GetAllPositionsAsync();

    /// <summary>
    ///     Retrieves all positions of a portfolio.
    /// </summary>
    /// <param name="portfolioId">Portfolio to retrieve positions of.</param>
    /// <returns>
    ///     A task representing the asynchronous query operation.
    ///     Task result contains an <see cref="IEnumerable{T}" /> containing retrieved positions.
    /// </returns>
    Task<IEnumerable<PositionDto>> GetPortfolioPositionsAsync(int portfolioId);

    /// <summary>
    ///     Retrieves a position by ID.
    /// </summary>
    /// <param name="positionId">ID of the position to retrieve.</param>
    /// <returns>
    ///     A task representing the asynchronous query operation.
    ///     Task result contains the retrieved position if it exists, <c>null</c> otherwise.
    /// </returns>
    Task<PositionDto> GetPositionAsync(int positionId);
}