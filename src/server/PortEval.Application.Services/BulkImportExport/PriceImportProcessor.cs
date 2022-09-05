using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators;
using PortEval.Application.Services.Interfaces;
using System.Threading.Tasks;


namespace PortEval.Application.Services.BulkImportExport
{
    public class PriceImportProcessor : ImportProcessor<InstrumentPriceDto, InstrumentPriceDtoValidator>
    {
        private readonly IInstrumentPriceService _priceService;

        public PriceImportProcessor(IInstrumentPriceService priceService) : base()
        {
            _priceService = priceService;
        }

        public override async Task<ProcessedRowErrorLogEntry<InstrumentPriceDto>> ProcessItem(InstrumentPriceDto row)
        {
            var logEntry = new ProcessedRowErrorLogEntry<InstrumentPriceDto>(row);
            var price = await _priceService.AddPricePointAsync(row);
            logEntry.Data.Id = price.Id;

            return logEntry;
        }
    }
}
