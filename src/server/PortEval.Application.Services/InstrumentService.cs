using PortEval.Application.Models.DTOs;
using PortEval.Application.Services.Interfaces;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using System.Collections.Generic;
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

        public InstrumentService(IInstrumentRepository instrumentRepository, ICurrencyRepository currencyRepository)
        {
            _instrumentRepository = instrumentRepository;
            _currencyRepository = currencyRepository;
        }

        /// <inheritdoc cref="IInstrumentService.CreateInstrumentAsync"/>
        public async Task<Instrument> CreateInstrumentAsync(InstrumentDto options)
        {
            if (!(await _currencyRepository.Exists(options.CurrencyCode)))
            {
                throw new ItemNotFoundException($"Currency {options.CurrencyCode} does not exist.");
            }

            var instrument = new Instrument(options.Name, options.Symbol, options.Exchange, options.Type, options.CurrencyCode);
            var createdInstrument = _instrumentRepository.Add(instrument);
            await _instrumentRepository.UnitOfWork.CommitAsync();
            BackgroundJob.Enqueue<IInitialPriceFetchJob>(job => job.Run(createdInstrument.Id));
            return createdInstrument;
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
    }
}
