using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Domain.Models.Entities;

namespace PortEval.Application.Core.Interfaces.Repositories
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
        /// Finds an instrument by ID.
        /// </summary>
        /// <param name="id">Instrument ID.</param>
        /// <returns>A task representing the asynchronous search operation. The task result contains the instrument entity with the supplied ID if it exists, null otherwise.</returns>
        public Task<Instrument> FindAsync(int id);

        /// <summary>
        /// Adds an instrument.
        /// </summary>
        /// <param name="instrument">Instrument entity to add.</param>
        /// <returns>The added instrument entity with identity set.</returns>
        public Instrument Add(Instrument instrument);

        /// <summary>
        /// Updates an instrument.
        /// </summary>
        /// <param name="instrument">Updated instrument entity.</param>
        /// <returns>The updated instrument.</returns>
        public Instrument Update(Instrument instrument);

        /// <summary>
        /// Deletes an instrument by ID.
        /// </summary>
        /// <param name="instrumentId">Id of instrument to delete.</param>
        /// <returns>A task representing the asynchronous lookup and deletion operations.</returns>
        public Task DeleteAsync(int instrumentId);

        /// <summary>
        /// Deletes an instrument.
        /// </summary>
        /// <param name="instrument">Instrument to delete.</param>
        public void Delete(Instrument instrument);

        /// <summary>
        /// Checks whether an instrument with the supplied id exists.
        /// </summary>
        /// <param name="id">Instrument id.</param>
        /// <returns>true if a instrument with the supplied instrument id exists in the database, false otherwise</returns>
        public Task<bool> ExistsAsync(int id);

        /// <summary>
        /// Checks whether an instrument with the supplied symbol exists.
        /// </summary>
        /// <param name="symbol">Instrument symbol.</param>
        /// <returns>true if a instrument with the supplied symbol exists in the database, false otherwise</returns>
        public Task<bool> ExistsAsync(string symbol);
    }
}
