using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Domain.Models.Entities;

namespace PortEval.Application.Core.Interfaces.Repositories
{
    /// <summary>
    /// Represents a persistently stored collection of positions.
    /// </summary>
    public interface IPositionRepository : IRepository
    {
        /// <summary>
        /// Retrieves positions of a specific portfolio.
        /// </summary>
        /// <param name="portfolioId">ID of the portfolio to retrieve positions of.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains an <see cref="IEnumerable{T}"/> containing positions of the specified portfolio.
        /// </returns>
        public Task<IEnumerable<Position>> ListPortfolioPositionsAsync(int portfolioId);

        /// <summary>
        /// Retrieves positions of a specific instrument.
        /// </summary>
        /// <param name="instrumentId">ID of the instrument to retrieve positions of.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains an <see cref="IEnumerable{T}"/> containing positions of the specified instrument.
        /// </returns>
        public Task<IEnumerable<Position>> ListAllInstrumentPositionsAsync(int instrumentId);

        /// <summary>
        /// Finds a position by ID.
        /// </summary>
        /// <param name="positionId">ID of the position to find.</param>
        /// <returns>
        /// A task representing the asynchronous search operation.
        /// Task result contains the position with a matching ID if it exists, <c>null</c> otherwise.
        /// </returns>
        public Task<Position> FindAsync(int positionId);

        /// <summary>
        /// Finds position to which the specified transaction belongs.
        /// </summary>
        /// <param name="transactionId">ID of the transaction to find parent position of.</param>
        /// <returns>
        /// A task representing the asynchronous search operation.
        /// Task result contains the position containing the specified transaction if it exists, <c>null</c> otherwise.
        /// </returns>
        public Task<Position> FindParentPositionAsync(int transactionId);

        /// <summary>
        /// Adds a position to the collection
        /// </summary>
        /// <param name="position">Position to add.</param>
        /// <returns>The added position.</returns>
        public Position Add(Position position);

        /// <summary>
        /// Updates a position.
        /// </summary>
        /// <param name="position">Position to update.</param>
        /// <returns>The updated position.</returns>
        public Position Update(Position position);

        /// <summary>
        /// Deletes a position by ID.
        /// </summary>
        /// <param name="positionId">ID of the position to delete.</param>
        /// <returns>A task representing the asynchronous deletion operation.</returns>
        public Task DeleteAsync(int positionId);

        /// <summary>
        /// Deletes a position.
        /// </summary>
        /// <param name="position">Position to delete.</param>
        public void Delete(Position position);

        /// <summary>
        /// Checks whether a position with the specified ID exists.
        /// </summary>
        /// <param name="id">ID of the position to check existence of.</param>
        /// <returns>
        /// A task representing the asynchronous check operation.
        /// Task result contains <c>true</c> if a position with the specified ID exists, <c>false</c> otherwise.
        /// </returns>
        public Task<bool> ExistsAsync(int id);
    }
}