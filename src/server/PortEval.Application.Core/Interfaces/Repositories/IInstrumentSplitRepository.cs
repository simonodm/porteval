using PortEval.Domain.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Core.Interfaces.Repositories
{
    /// <summary>
    /// Represents a persistently stored collection of instrument splits.
    /// </summary>
    public interface IInstrumentSplitRepository : IRepository
    {
        /// <summary>
        /// Lists all splits of a particular instrument.
        /// </summary>
        /// <param name="instrumentId">ID of the instrument to find splits of.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains an <c>IEnumerable</c> containing instrument splits.
        /// </returns>
        public Task<IEnumerable<InstrumentSplit>> ListInstrumentSplitsAsync(int instrumentId);

        /// <summary>
        /// Lists all splits which have not been processed yet.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains an <c>IEnumerable</c> containing non-processed splits.
        /// </returns>
        public Task<IEnumerable<InstrumentSplit>> ListNonProcessedSplitsAsync();

        /// <summary>
        /// Lists all splits, for which a rollback has been requested but not yet processed.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains an <c>IEnumerable</c> containing splits for which rollback has been requested.
        /// </returns>
        public Task<IEnumerable<InstrumentSplit>> ListRollbackRequestedSplitsAsync();

        /// <summary>
        /// Finds an instrument split by ID.
        /// </summary>
        /// <param name="splitId">ID of the instrument split to find.</param>
        /// <returns>
        /// A task representing the asynchronous database query.
        /// Task result contains the found split if such exists, <c>null</c> otherwise.
        /// </returns>
        public Task<InstrumentSplit> FindAsync(int splitId);

        /// <summary>
        /// Adds a split to the repository.
        /// </summary>
        /// <param name="instrumentSplit">Instrument split to add.</param>
        /// <returns>The added instrument split.</returns>
        public InstrumentSplit Add(InstrumentSplit instrumentSplit);

        /// <summary>
        /// Updates a split.
        /// </summary>
        /// <param name="instrumentSplit">The updated split.</param>
        /// <returns>The updated instrument split.</returns>
        public InstrumentSplit Update(InstrumentSplit instrumentSplit);

        /// <summary>
        /// Deletes a split by ID.
        /// </summary>
        /// <param name="splitId">ID of the split to delete.</param>
        /// <returns>A task representing the asynchronous lookup and deletion operations.</returns>
        public Task DeleteAsync(int splitId);

        /// <summary>
        /// Deletes a split.
        /// </summary>
        /// <param name="split">Instrument split to delete.</param>
        public void Delete(InstrumentSplit split);

        /// <summary>
        /// Checks whether a split with the specified ID exists.
        /// </summary>
        /// <param name="splitId">ID of the split to check for existence.</param>
        /// <returns>
        /// A task representing the asynchronous lookup operation.
        /// Task result contains <c>true</c> if a split with the provided ID exists, <c>false</c> otherwise.
        /// </returns>
        public Task<bool> ExistsAsync(int splitId);
    }
}
