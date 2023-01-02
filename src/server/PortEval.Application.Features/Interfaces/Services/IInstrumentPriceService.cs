using PortEval.Application.Models.DTOs;
using PortEval.Domain.Models.Entities;
using System;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Interfaces.Services
{
    /// <summary>
    /// Handles creation and deletion of instrument prices.
    /// </summary>
    public interface IInstrumentPriceService
    {
        /// <summary>
        /// Adds an instrument price according to the supplied DTO.
        /// </summary>
        /// <param name="options">A DTO containing client's request body.</param>
        /// <returns>A task representing the asynchronous creation operation. Task result contains newly created instrument price entity.</returns>
        public Task<InstrumentPrice> AddPricePointAsync(InstrumentPriceDto options);

        /// <summary>
        /// Adds an instrument price if no instrument price is found at the specified time, or if the price found differs from the provided value.
        /// </summary>
        /// <param name="instrumentId">ID of an instrument to find or create price for.</param>
        /// <param name="time">Time at which the price is valid.</param>
        /// <param name="price">Price represented in the currency of the instrument.</param>
        /// <returns>
        /// A task representing the asynchronous search and creation operation. Task result contains the existing price at the specified time,
        /// or a newly created instrument price entity if it is a new price.
        /// </returns>
        public Task<InstrumentPrice> AddPriceIfNotExistsAsync(int instrumentId, DateTime time, decimal price);

        /// <summary>
        /// Deletes an instrument price.
        /// </summary>
        /// <param name="instrumentId">Parent instrument ID.</param>
        /// <param name="priceId">Instrument price ID.</param>
        /// <returns>A task representing the asynchronous deletion operation.</returns>
        public Task DeletePricePointByIdAsync(int instrumentId, int priceId);
    }
}
