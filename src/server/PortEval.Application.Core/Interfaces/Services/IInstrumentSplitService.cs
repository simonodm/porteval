using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Models.Entities;

namespace PortEval.Application.Core.Interfaces.Services
{
    /// <summary>
    /// Handles creation and removal of instrument price splits.
    /// </summary>
    public interface IInstrumentSplitService
    {
        /// <summary>
        /// Creates a price split according to the provided DTO.
        /// </summary>
        /// <param name="options">A DTO containing split data.</param>
        /// <returns>A task representing the asynchronous creation operation. Task result contains the newly created split entity.</returns>
        public Task<InstrumentSplit> CreateSplitAsync(InstrumentSplitDto options);

        /// <summary>
        /// Updates an instrument split.
        /// </summary>
        /// <param name="instrumentId">ID of the expected parent instrument</param>
        /// <param name="options">A DTO containing updated split data.</param>
        /// <returns>A task representing the asynchronous update operation.</returns>
        public Task<InstrumentSplit> UpdateSplitAsync(int instrumentId, InstrumentSplitDto options);
    }
}
