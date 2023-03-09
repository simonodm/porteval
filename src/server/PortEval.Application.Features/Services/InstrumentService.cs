using PortEval.Application.Features.Interfaces.Repositories;
using PortEval.Application.Features.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Services
{
    /// <inheritdoc cref="IInstrumentService"/>
    public class InstrumentService : IInstrumentService
    {
        private readonly IInstrumentRepository _instrumentRepository;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IExchangeRepository _exchangeRepository;

        public InstrumentService(IInstrumentRepository instrumentRepository, ICurrencyRepository currencyRepository, IExchangeRepository exchangeRepository)
        {
            _instrumentRepository = instrumentRepository;
            _currencyRepository = currencyRepository;
            _exchangeRepository = exchangeRepository;
        }

        /// <inheritdoc cref="IInstrumentService.CreateInstrumentAsync"/>
        public async Task<Instrument> CreateInstrumentAsync(InstrumentDto options)
        {
            if (!await _currencyRepository.ExistsAsync(options.CurrencyCode))
            {
                throw new ItemNotFoundException($"Currency {options.CurrencyCode} does not exist.");
            }

            if (await _instrumentRepository.ExistsAsync(options.Symbol))
            {
                throw new OperationNotAllowedException($"An instrument with symbol {options.Symbol} already exists.");
            }

            await CreateExchangeIfDoesNotExist(options.Exchange);

            var instrument = Instrument.Create(options.Name, options.Symbol, string.IsNullOrEmpty(options.Exchange) ? null : options.Exchange,
                options.Type, options.CurrencyCode, options.Note);
            var createdInstrument = _instrumentRepository.Add(instrument);
            await _instrumentRepository.UnitOfWork.CommitAsync();
            return createdInstrument;
        }

        /// <inheritdoc cref="IInstrumentService.UpdateInstrumentAsync"/>
        public async Task<Instrument> UpdateInstrumentAsync(InstrumentDto options)
        {
            var existingInstrument = await _instrumentRepository.FindAsync(options.Id);
            if (existingInstrument == null)
            {
                throw new ItemNotFoundException($"Instrument {options.Id} does not exist.");
            }

            await CreateExchangeIfDoesNotExist(options.Exchange);

            existingInstrument.Rename(options.Name);
            existingInstrument.SetExchange(string.IsNullOrEmpty(options.Exchange) ? null : options.Exchange);
            existingInstrument.SetNote(options.Note);
            existingInstrument.IncreaseVersion();
            _instrumentRepository.Update(existingInstrument);
            await _instrumentRepository.UnitOfWork.CommitAsync();
            return existingInstrument;
        }

        /// <inheritdoc cref="IInstrumentService.DeleteAsync"/>
        public async Task DeleteAsync(int id)
        {
            if (!await _instrumentRepository.ExistsAsync(id))
            {
                throw new ItemNotFoundException($"Instrument {id} does not exist.");
            }

            await _instrumentRepository.DeleteAsync(id);
            await _instrumentRepository.UnitOfWork.CommitAsync();
        }

        private async Task CreateExchangeIfDoesNotExist(string exchange)
        {
            if (!string.IsNullOrEmpty(exchange) && !await _exchangeRepository.ExistsAsync(exchange))
            {
                var newExchange = Exchange.Create(exchange);
                _exchangeRepository.Add(newExchange);
            }
        }
    }
}
