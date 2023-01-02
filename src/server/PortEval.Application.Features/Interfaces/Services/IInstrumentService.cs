﻿using PortEval.Application.Models.DTOs;
using PortEval.Domain.Models.Entities;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Interfaces.Services
{
    /// <summary>
    /// Handles creation and removal of instruments.
    /// </summary>
    public interface IInstrumentService
    {
        /// <summary>
        /// Creates an instrument according to the supplied DTO.
        /// </summary>
        /// <param name="options">A DTO containing instrument data.</param>
        /// <returns>A task representing the asynchronous creation operation. Task result contains newly created instrument entity.</returns>
        public Task<Instrument> CreateInstrumentAsync(InstrumentDto options);

        /// <summary>
        /// Updates an instrument according to the supplied DTO.
        /// </summary>
        /// <param name="options">A DTO containing instrument data.</param>
        /// <returns>A task representing the asynchronous update operation. Task result contains updated instrument entity.</returns>
        public Task<Instrument> UpdateInstrumentAsync(InstrumentDto options);

        /// <summary>
        /// Deletes an instrument.
        /// </summary>
        /// <param name="id">Instrument ID.</param>
        /// <returns>A task representing the asynchronous deletion operation.</returns>
        public Task DeleteAsync(int id);
    }
}