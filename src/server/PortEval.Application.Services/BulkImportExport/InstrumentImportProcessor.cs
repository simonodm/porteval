using Hangfire;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators;
using PortEval.Application.Services.Interfaces;
using PortEval.Application.Services.Interfaces.BackgroundJobs;
using PortEval.Domain.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Services.BulkImportExport
{
    public class InstrumentImportProcessor : ImportProcessor<InstrumentDto, InstrumentDtoValidator>
    {
        private readonly IInstrumentService _instrumentService;
        private readonly List<Instrument> _newInstruments;

        public InstrumentImportProcessor(IInstrumentService instrumentService) : base()
        {
            _instrumentService = instrumentService;
            _newInstruments = new List<Instrument>();

            OnImportFinish = () =>
            {
                foreach (var instrument in _newInstruments)
                {
                    BackgroundJob.Enqueue<IInitialPriceFetchJob>(job => job.Run(instrument.Id));
                }
            };
        }

        public override async Task<ProcessedRowErrorLogEntry<InstrumentDto>> ProcessItem(InstrumentDto row)
        {
            var logEntry = new ProcessedRowErrorLogEntry<InstrumentDto>(row);

            if (row.Id != default)
            {
                await _instrumentService.UpdateInstrumentAsync(row);
            }
            else
            {
                var instrument = await _instrumentService.CreateInstrumentAsync(row);
                _newInstruments.Add(instrument);
                logEntry.Data.Id = instrument.Id;
            }

            return logEntry;
        }
    }
}
