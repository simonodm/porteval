using PortEval.Application.Models.DTOs;
using PortEval.Application.Services.Interfaces;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using System.Threading.Tasks;

namespace PortEval.Application.Services
{
    /// <inheritdoc cref="IInstrumentPriceService"/>
    public class InstrumentPriceService : IInstrumentPriceService
    {
        private readonly IInstrumentRepository _instrumentRepository;

        public InstrumentPriceService(IInstrumentRepository instrumentRepository)
        {
            _instrumentRepository = instrumentRepository;
        }

        /// <inheritdoc cref="IInstrumentPriceService.AddPricePointAsync"/>
        public async Task<InstrumentPrice> AddPricePointAsync(InstrumentPriceDto options)
        {
            var instrument = await FetchInstrument(options.InstrumentId);
            var pricePoint = instrument.AddPricePoint(options.Time, options.Price);
            _instrumentRepository.Update(instrument);
            await _instrumentRepository.UnitOfWork.CommitAsync();
            return pricePoint;
        }

        /// <inheritdoc cref="IInstrumentPriceService.AddPricePointAsync"/>
        public async Task DeletePricePointByIdAsync(int instrumentId, int priceId)
        {
            var instrument = await FetchInstrument(instrumentId);
            instrument.RemovePricePointById(priceId);
            _instrumentRepository.Update(instrument);
            await _instrumentRepository.UnitOfWork.CommitAsync();
        }

        /// <summary>
        /// Retrieves an instrument by ID.
        /// </summary>
        /// <param name="instrumentId">ID of the instrument to retrieve.</param>
        /// <exception cref="ItemNotFoundException">If no instrument with the supplied ID exists.</exception>
        /// <returns>A task representing the asynchronous search operation. Task result contains the found instrument entity.</returns>
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
