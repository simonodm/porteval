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
            var instrument = await FetchInstrument(options.InstrumentId);
            if (await _instrumentPriceRepository.ExistsAsync(options.InstrumentId, options.Time))
            {
                throw new OperationNotAllowedException(
                    $"{instrument.Symbol} already contains a price at {options.Time}.");
            }

            var pricePoint = InstrumentPrice.Create(options.Time.RoundDown(TimeSpan.FromMinutes(1)), options.Price,
                instrument);
            _instrumentPriceRepository.Add(pricePoint);
            await _instrumentPriceRepository.UnitOfWork.CommitAsync();
            return pricePoint;
        }

        /// <inheritdoc cref="IInstrumentPriceService.AddPriceIfNotExistsAsync"/>
        public async Task<InstrumentPrice> AddPriceIfNotExistsAsync(int instrumentId, DateTime time, decimal price)
        {
            var instrument = await FetchInstrument(instrumentId);

            var existingPrice = await _instrumentPriceRepository.FindPriceAtAsync(instrumentId, time);
            if (existingPrice != null && existingPrice.Price == price)
            {
                return existingPrice;
            }

            var pricePoint = InstrumentPrice.Create(time.RoundDown(TimeSpan.FromMinutes(1)), price, instrument);
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

        private async Task<Instrument> FetchInstrument(int instrumentId)
        {
            var instrument = await _instrumentRepository.FindAsync(instrumentId);
            if (instrument == null)
            {
                throw new ItemNotFoundException($"Instrument {instrumentId} does not exist.");
            }

            return instrument;
        }
    }
}
