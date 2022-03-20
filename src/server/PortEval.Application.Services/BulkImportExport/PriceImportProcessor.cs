using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;

namespace PortEval.Application.Services.BulkImportExport
{
    public class PriceImportProcessor : ImportProcessor<InstrumentPriceDto, InstrumentPriceDtoValidator>
    {
        private readonly IInstrumentRepository _instrumentRepository;
        private readonly IInstrumentPriceRepository _priceRepository;

        private readonly HashSet<int> _existingInstrumentsCache;

        public PriceImportProcessor(IInstrumentPriceRepository priceRepository, IInstrumentRepository instrumentRepository) : base(priceRepository.UnitOfWork)
        {
            _priceRepository = priceRepository;
            _instrumentRepository = instrumentRepository;
            _existingInstrumentsCache = new HashSet<int>();
        }

        public override async Task<ErrorLogEntry<InstrumentPriceDto>> ProcessItem(InstrumentPriceDto row)
        {
            var logEntry = new ErrorLogEntry<InstrumentPriceDto>(row);
            var instrumentExists = false;

            if (_existingInstrumentsCache.Contains(row.InstrumentId))
            {
                instrumentExists = true;
            }
            else if (await _instrumentRepository.Exists(row.InstrumentId))
            {
                instrumentExists = true;
                _existingInstrumentsCache.Add(row.InstrumentId);
            }

            if (!instrumentExists)
            {
                logEntry.AddError($"No instrument with id {row.InstrumentId} found.");
                return logEntry;
            }

            var priceExists = (row.Id != default &&
                               await _priceRepository.Exists(row.InstrumentId, row.Id))
                              || await _priceRepository.Exists(row.InstrumentId, row.Time);
            if (!priceExists)
            {
                _priceRepository.AddInstrumentPrice(new InstrumentPrice(row.Time, row.Price,
                    row.InstrumentId));
            }

            return logEntry;
        }
    }
}
