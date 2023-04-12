using PortEval.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Core.Interfaces.Repositories
{
    /// <summary>
    /// Represents a persistently stored collection of instrument prices.
    /// </summary>
    public interface IInstrumentPriceRepository : IRepository
    {
        /// <summary>
        /// Lists all persisted prices of an instrument.
        /// </summary>
        /// <param name="instrumentId">ID of instrument to list prices of.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains an <see cref="IEnumerable{T}"/> containing all available prices of the provided instrument.
        /// </returns>
        public Task<IEnumerable<InstrumentPrice>> ListInstrumentPricesAsync(int instrumentId);

        /// <summary>
        /// Retrieves a specific price by ID.
        /// </summary>
        /// <param name="instrumentId">ID of instrument to find price of.</param>
        /// <param name="priceId">ID of the price to retrieve.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains the requested price if it exists, <c>null</c> otherwise.
        /// </returns>
        public Task<InstrumentPrice> FindPriceByIdAsync(int instrumentId, int priceId);

        /// <summary>
        /// Retrieves the latest available price of an instrument at the specified time.
        /// </summary>
        /// <param name="instrumentId">ID of instrument to find price of.</param>
        /// <param name="time">Time at which to retrieve the price.</param>
        /// <returns>
        /// A task representing the asynchronous retrieval operation.
        /// Task result contains the requested price if it exists, <c>null</c> otherwise.
        /// </returns>
        public Task<InstrumentPrice> FindPriceAtAsync(int instrumentId, DateTime time);

        /// <summary>
        /// Adds an instrument price to the repository.
        /// </summary>
        /// <param name="price">Instrument price to add.</param>
        /// <returns>The entity which was added.</returns>
        public InstrumentPrice Add(InstrumentPrice price);

        /// <summary>
        /// Updates an instrument price.
        /// </summary>
        /// <param name="price">Instrument price to update.</param>
        /// <returns>The updated entity.</returns>
        public InstrumentPrice Update(InstrumentPrice price);

        /// <summary>
        /// Upserts the provided prices into the repository, performing matching based on parent instrument ID and price time.
        /// </summary>
        /// <remarks>
        /// For performance reasons, this operation runs in its own unit of work, so it is committed automatically.
        /// </remarks>
        /// <param name="prices">Prices to upsert into the repository.</param>
        /// <returns>A task representing the asynchronous bulk upsert operation.</returns>
        public Task BulkUpsertAsync(IList<InstrumentPrice> prices);

        /// <summary>
        /// Deletes a price by ID.
        /// </summary>
        /// <param name="instrumentId">ID of the instrument to which the price belongs.</param>
        /// <param name="priceId">ID of the instrument price to delete.</param>
        /// <returns>A task representing the asynchronous deletion operation.</returns>
        public Task DeleteAsync(int instrumentId, int priceId);

        /// <summary>
        /// Deletes a price.
        /// </summary>
        /// <param name="price">Instrument price to delete.</param>
        public void Delete(InstrumentPrice price);

        /// <summary>
        /// Determines whether a price with the specified IDs exists.
        /// </summary>
        /// <param name="instrumentId">ID of the instrument to which the price belongs.</param>
        /// <param name="priceId">ID of the instrument price.</param>
        /// <returns>
        /// A task representing the asynchronous query operation.
        /// Task result contains <c>true</c> if the price exists, <c>false</c> otherwise.
        /// </returns>
        public Task<bool> ExistsAsync(int instrumentId, int priceId);

        /// <summary>
        /// Determines whether an instrument price exists at the specified time.
        /// </summary>
        /// <param name="instrumentId">ID of the instrument to which the price belongs.</param>
        /// <param name="time">Time of the price.</param>
        /// <returns>
        /// A task representing the asynchronous query operation.
        /// Task result contains <c>true</c> if the price exists, <c>false</c> otherwise.
        /// </returns>
        public Task<bool> ExistsAsync(int instrumentId, DateTime time);
    }
}
