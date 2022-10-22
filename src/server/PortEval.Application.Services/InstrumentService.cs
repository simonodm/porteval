﻿using PortEval.Application.Models.DTOs;
using PortEval.Application.Services.Interfaces;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using System.Threading.Tasks;
using Hangfire;
using PortEval.Application.Services.Interfaces.BackgroundJobs;

namespace PortEval.Application.Services
{
    /// <inheritdoc cref="IInstrumentService"/>
    public class InstrumentService : IInstrumentService
    {
        private readonly IInstrumentRepository _instrumentRepository;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IExchangeRepository _exchangeRepository;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public InstrumentService(IInstrumentRepository instrumentRepository, ICurrencyRepository currencyRepository, IExchangeRepository exchangeRepository, IBackgroundJobClient backgroundJobClient)
        {
            _instrumentRepository = instrumentRepository;
            _currencyRepository = currencyRepository;
            _exchangeRepository = exchangeRepository;
            _backgroundJobClient = backgroundJobClient;
        }

        /// <inheritdoc cref="IInstrumentService.CreateInstrumentAsync"/>
        public async Task<Instrument> CreateInstrumentAsync(InstrumentDto options)
        {
            if (!(await _currencyRepository.Exists(options.CurrencyCode)))
            {
                throw new ItemNotFoundException($"Currency {options.CurrencyCode} does not exist.");
            }

            await CreateExchangeIfDoesNotExist(options.Exchange);

            var instrument = new Instrument(options.Name, options.Symbol, string.IsNullOrEmpty(options.Exchange) ? null : options.Exchange,
                options.Type, options.CurrencyCode, options.Note);
            var createdInstrument = _instrumentRepository.Add(instrument);
            await _instrumentRepository.UnitOfWork.CommitAsync();
            _backgroundJobClient.Enqueue<IInitialPriceFetchJob>(job => job.Run(createdInstrument.Id));
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
            if (!(await _instrumentRepository.Exists(id)))
            {
                throw new ItemNotFoundException($"Instrument {id} does not exist.");
            }

            await _instrumentRepository.Delete(id);
            await _instrumentRepository.UnitOfWork.CommitAsync();
        }

        private async Task CreateExchangeIfDoesNotExist(string exchange)
        {
            if(!string.IsNullOrEmpty(exchange) && !(await _exchangeRepository.Exists(exchange)))
            {
                var newExchange = new Exchange(exchange);
                _exchangeRepository.Add(newExchange);
            }
        }
    }
}
