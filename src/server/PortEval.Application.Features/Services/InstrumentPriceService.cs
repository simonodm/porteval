using PortEval.Application.Features.Extensions;
using PortEval.Application.Features.Interfaces.Repositories;
using PortEval.Application.Features.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using System;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Services
{
    /// <inheritdoc cref="IInstrumentPriceService"/>
    public class InstrumentPriceService : IInstrumentPriceService
    {
        private readonly IInstrumentRepository _instrumentRepository;
        private readonly IInstrumentPriceRepository _instrumentPriceRepository;

        public InstrumentPriceService(IInstrumentRepository instrumentRepository, IInstrumentPriceRepository instrumentPriceRepository)
        {
            _instrumentRepository = instrumentRepository;
            _instrumentPriceRepository = instrumentPriceRepository;
        }

        /// <inheritdoc cref="IInstrumentPriceService.AddPricePointAsync"/>
        public async Task<InstrumentPrice> AddPricePointAsync(InstrumentPriceDto options)
        {
            await ValidateInstrumentExists(options.InstrumentId);

            var pricePoint = InstrumentPrice.Create(options.Time.RoundDown(TimeSpan.FromMinutes(1)), options.Price,
                options.InstrumentId);
            _instrumentPriceRepository.Add(pricePoint);
            await _instrumentPriceRepository.UnitOfWork.CommitAsync();
            return pricePoint;
        }

        /// <inheritdoc cref="IInstrumentPriceService.AddPriceIfNotExistsAsync"/>
        public async Task<InstrumentPrice> AddPriceIfNotExistsAsync(int instrumentId, DateTime time, decimal price)
        {
            await ValidateInstrumentExists(instrumentId);

            var existingPrice = await _instrumentPriceRepository.FindPriceAtAsync(instrumentId, time);
            if (existingPrice != null && existingPrice.Price == price)
            {
                return existingPrice;
            }

            var pricePoint = InstrumentPrice.Create(time.RoundDown(TimeSpan.FromMinutes(1)), price, instrumentId);
            _instrumentPriceRepository.Add(pricePoint);
            await _instrumentPriceRepository.UnitOfWork.CommitAsync();
            return pricePoint;
        }

        /// <inheritdoc cref="IInstrumentPriceService.DeletePricePointByIdAsync"/>
        public async Task DeletePricePointByIdAsync(int instrumentId, int priceId)
        {
            if (!await _instrumentPriceRepository.ExistsAsync(instrumentId, priceId))
            {
                throw new ItemNotFoundException($"Price {priceId} does not exist on instrument {instrumentId}.");
            }
            await _instrumentPriceRepository.DeleteAsync(instrumentId, priceId);
            await _instrumentPriceRepository.UnitOfWork.CommitAsync();
        }

        /// <summary>
        /// Checks whether an instrument with the specified ID exists, throws an exception otherwise.
        /// </summary>
        /// <param name="instrumentId">ID of the instrument to check.</param>
        /// <exception cref="ItemNotFoundException">If no instrument with the supplied ID exists.</exception>
        /// <returns>A task representing the asynchronous search operation.</returns>
        private async Task ValidateInstrumentExists(int instrumentId)
        {
            if (!await _instrumentRepository.ExistsAsync(instrumentId))
            {
                throw new ItemNotFoundException($"Instrument {instrumentId} does not exist.");
            }
        }
    }
}
