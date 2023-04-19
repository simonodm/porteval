using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Interfaces.Queries;

/// <summary>
///     Implements queries for stock exchanges stored in the application's persistent storage.
/// </summary>
public interface IExchangeQueries
{
    /// <summary>
    ///     Retrieves all known stock exchanges.
    /// </summary>
    /// <returns>
    ///     A task representing the asynchronous query operation.
    ///     Task result contains an <see cref="IEnumerable{T}" /> containing stored stock exchanges.
    /// </returns>
    Task<IEnumerable<ExchangeDto>> GetKnownExchangesAsync();
}