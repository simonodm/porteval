using PortEval.Application.Features.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Services.BulkImportExport
{
    public class PriceImportProcessor : ImportProcessor<InstrumentPriceDto, InstrumentPriceDtoValidator>
    {
        private readonly IInstrumentPriceService _priceService;

        public PriceImportProcessor(IInstrumentPriceService priceService) : base()
        {
            _priceService = priceService;
        }

        protected override async Task<ProcessedRowErrorLogEntry<InstrumentPriceDto>> ProcessItem(InstrumentPriceDto row)
        {
            var logEntry = new ProcessedRowErrorLogEntry<InstrumentPriceDto>(row);
            if (row.Id == default)
            {
                var price = await _priceService.AddPricePointAsync(row);
                logEntry.Data.Id = price.Id;
            }

            return logEntry;
        }
    }
}
