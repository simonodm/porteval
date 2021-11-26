using PortEval.Domain.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Services.Interfaces.Repositories
{
    /// <summary>
    /// Represents a persistently stored collection of instruments.
    /// </summary>
    public interface IInstrumentRepository : IRepository
    {
        /// <summary>
        /// Lists all instruments.
        /// </summary>
        /// <returns>A task representing the asynchronous retrieval operation. The task result contains an <c>IEnumerable</c> containing all existing instruments.</returns>
        public Task<IEnumerable<Instrument>> ListAllAsync();

        /// <summary>
        /// Finds a instrument by ID.
        /// </summary>
        /// <param name="id">Instrument ID.</param>
        /// <returns>A task representing the asynchronous search operation. The task result contains the instrument entity with the supplied ID if it exists, null otherwise.</returns>
        public Task<Instrument> FindAsync(int id);

        /// <summary>
        /// Adds a instrument.
        /// </summary>
        /// <param name="instrument">Instrument entity to add.</param>
        /// <returns>The added instrument entity with identity set.</returns>
        public Instrument Add(Instrument instrument);

        /// <summary>
        /// Updates a instrument.
        /// </summary>
        /// <param name="instrument">Updated instrument entity.</param>
        /// <returns>The updated instrument.</returns>
        public Instrument Update(Instrument instrument);

        /// <summary>
        /// Deletes a instrument.
        /// </summary>
        /// <param name="instrumentId">Id of instrument to delete.</param>
        /// <returns>A task representing the asynchronous deletion operation.</returns>
        public Task Delete(int instrumentId);

        /// <summary>
        /// Checks whether an instrument with the supplied id exists.
        /// </summary>
        /// <param name="id">Instrument id.</param>
        /// <returns>true if a instrument with the supplied instrument id exists in the database, false otherwise</returns>
        public Task<bool> Exists(int id);
    }
}
