using System.Collections.Generic;
using System.Threading.Tasks;
using Hangfire;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators;
using PortEval.Application.Services.Interfaces.BackgroundJobs;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;

namespace PortEval.Application.Services.BulkImportExport
{
    public class InstrumentImportProcessor : ImportProcessor<InstrumentDto, InstrumentDtoValidator>
    {
        private readonly IInstrumentRepository _instrumentRepository;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly List<Instrument> _newInstruments;

        public InstrumentImportProcessor(IInstrumentRepository instrumentRepository, ICurrencyRepository currencyRepository) : base(instrumentRepository.UnitOfWork)
        {
            _instrumentRepository = instrumentRepository;
            _currencyRepository = currencyRepository;
            _newInstruments = new List<Instrument>();

            OnImportFinish = () =>
            {
                foreach (var instrument in _newInstruments)
                {
                    BackgroundJob.Enqueue<IInitialPriceFetchJob>(job => job.Run(instrument.Id));
                }
            };
        }

        public override async Task<ErrorLogEntry<InstrumentDto>> ProcessItem(InstrumentDto row)
        {
            var logEntry = new ErrorLogEntry<InstrumentDto>(row);
            if (!await _currencyRepository.Exists(row.CurrencyCode))
            {
                logEntry.AddError($"Unknown currency: {row.CurrencyCode}.");
            }
            else
            {
                var existingInstrument = row.Id != default
                    ? await _instrumentRepository.FindAsync(row.Id)
                    : null;

                if (existingInstrument == null)
                {
                    var instrument = new Instrument(row.Name, row.Symbol,
                        row.Exchange, row.Type, row.CurrencyCode, row.Note);
                    _instrumentRepository.Add(instrument);
                    _newInstruments.Add(instrument);
                }
                else
                {
                    existingInstrument.Rename(row.Name);
                    existingInstrument.SetNote(row.Note);
                    _instrumentRepository.Update(existingInstrument);
                }
            }

            return logEntry;
        }
    }
}
