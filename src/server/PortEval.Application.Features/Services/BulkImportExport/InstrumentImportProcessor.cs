using PortEval.Application.Features.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Services.BulkImportExport
{
    public class InstrumentImportProcessor : ImportProcessor<InstrumentDto, InstrumentDtoValidator>
    {
        private readonly IInstrumentService _instrumentService;

        public InstrumentImportProcessor(IInstrumentService instrumentService) : base()
        {
            _instrumentService = instrumentService;
        }

        protected override async Task<ProcessedRowErrorLogEntry<InstrumentDto>> ProcessItem(InstrumentDto row)
        {
            var logEntry = new ProcessedRowErrorLogEntry<InstrumentDto>(row);

            if (row.Id != default)
            {
                await _instrumentService.UpdateInstrumentAsync(row);
            }
            else
            {
                var instrument = await _instrumentService.CreateInstrumentAsync(row);
                logEntry.Data.Id = instrument.Id;
            }

            return logEntry;
        }
    }
}
