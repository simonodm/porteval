using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Core.Queries;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Interfaces.Queries
{
    /// <summary>
    /// Implements high performance read-only exchange queries.
    /// </summary>
    public interface IExchangeQueries
    {
        /// <summary>
        /// Retrieves all known stock exchanges.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous database query operation.
        /// Task result contains a <see cref="QueryResponse{T}"/> wrapper over an <see cref="IEnumerable{T}"/> containing retrieved stock exchanges.
        /// </returns>
        public Task<QueryResponse<IEnumerable<ExchangeDto>>> GetKnownExchanges();
    }
}
