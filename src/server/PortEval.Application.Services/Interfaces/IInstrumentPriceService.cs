using PortEval.Application.Models.DTOs;
using PortEval.Domain.Models.Entities;
using System.Threading.Tasks;

namespace PortEval.Application.Services.Interfaces
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
        /// Deletes an insturment price.
        /// </summary>
        /// <param name="instrumentId">Parent instrument ID.</param>
        /// <param name="priceId">Instrument price ID.</param>
        /// <returns>A task representing the asynchronous deletion operation.</returns>
        public Task DeletePricePointByIdAsync(int instrumentId, int priceId);
    }
}
