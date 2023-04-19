using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Domain.Models.Entities;

namespace PortEval.Application.Core.Interfaces.Repositories;

/// <summary>
///     Represents a persistently stored collection of stock exchanges.
/// </summary>
public interface IExchangeRepository : IRepository
{
    /// <summary>
    ///     Lists all exchanges.
    /// </summary>
    /// <returns>
    ///     A task representing the asynchronous retrieval operation. The task result contains an <c>IEnumerable</c> containing
    ///     all existing exchanges.
    /// </returns>
    public Task<IEnumerable<Exchange>> ListAllAsync();

    /// <summary>
    ///     Finds an exchange by its symbol.
    /// </summary>
    /// <param name="exchangeSymbol">Exchange symbol.</param>
    /// <returns>
    ///     A task representing the asynchronous retrieval operation. The task result contains the exchange with the specified
    ///     symbol if it exists, <c>null</c> otherwise.
    /// </returns>
    public Task<Exchange> FindAsync(string exchangeSymbol);

    /// <summary>
    ///     Creates a new exchange.
    /// </summary>
    /// <param name="exchange">Exchange to create.</param>
    /// <returns>
    ///     The created exchange.
    /// </returns>
    public Exchange Add(Exchange exchange);

    /// <summary>
    ///     Updates an exchange.
    /// </summary>
    /// <param name="exchange">Exchange to update.</param>
    /// <returns>
    ///     The updated exchange.
    /// </returns>
    public Exchange Update(Exchange exchange);

    /// <summary>
    ///     Checks whether an exchange with the specified symbol exists.
    /// </summary>
    /// <param name="exchangeSymbol">Exchange symbol.</param>
    /// <returns>
    ///     A task representing the asynchronous retrieval operation. The task result contains <c>true</c> if an exchange with
    ///     the specified symbol exists, <c>false</c> otherwise.
    /// </returns>
    public Task<bool> ExistsAsync(string exchangeSymbol);
}