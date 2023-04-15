using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators;
using System.Threading.Tasks;

namespace PortEval.Application.Core.Common.BulkImportExport
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
                var response = await _instrumentService.UpdateInstrumentAsync(row);
                if (response.Status != OperationStatus.Ok)
                {
                    logEntry.AddError(response.Message);
                }
            }
            else
            {
                var response = await _instrumentService.CreateInstrumentAsync(row);
                if (response.Status != OperationStatus.Ok)
                {
                    logEntry.AddError(response.Message);
                }

                logEntry.Data.Id = response.Response?.Id ?? default;
            }

            return logEntry;
        }
    }
}
