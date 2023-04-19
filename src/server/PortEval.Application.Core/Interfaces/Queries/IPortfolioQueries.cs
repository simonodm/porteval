using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Interfaces.Queries;

/// <summary>
///     Implements queries for portfolios stored in the application's persistent storage.
/// </summary>
public interface IPortfolioQueries
{
    /// <summary>
    ///     Retrieves all known portfolios.
    /// </summary>
    /// <returns>
    ///     A task representing the asynchronous query operation.
    ///     Task result contains an <see cref="IEnumerable{T}" /> containing stored portfolios.
    /// </returns>
    Task<IEnumerable<PortfolioDto>> GetAllPortfoliosAsync();

    /// <summary>
    ///     Retrieves a portfolio by ID.
    /// </summary>
    /// <param name="portfolioId">ID of the portfolio to retrieve.</param>
    /// <returns>
    ///     A task representing the asynchronous query operation.
    ///     Task result contains the retrieved portfolio if it exists, <c>null</c> otherwise.
    /// </returns>
    Task<PortfolioDto> GetPortfolioAsync(int portfolioId);
}