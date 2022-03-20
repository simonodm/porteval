using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;

namespace PortEval.Application.Services.BulkImportExport
{
    public class PositionImportProcessor : ImportProcessor<PositionDto, PositionDtoValidator>
    {
        private readonly IPositionRepository _positionRepository;
        private readonly IInstrumentRepository _instrumentRepository;
        private readonly IPortfolioRepository _portfolioRepository;

        private readonly HashSet<int> _existingPortfoliosCache;
        private readonly HashSet<int> _existingInstrumentsCache;

        public PositionImportProcessor(IPositionRepository positionRepository, IInstrumentRepository instrumentRepository, IPortfolioRepository portfolioRepository) : base(positionRepository.UnitOfWork)
        {
            _positionRepository = positionRepository;
            _instrumentRepository = instrumentRepository;
            _portfolioRepository = portfolioRepository;
            _existingPortfoliosCache = new HashSet<int>();
            _existingInstrumentsCache = new HashSet<int>();
        }

        public override async Task<ErrorLogEntry<PositionDto>> ProcessItem(PositionDto row)
        {
            var logEntry = new ErrorLogEntry<PositionDto>(row);

            var portfolioExists = _existingPortfoliosCache.Contains(row.PortfolioId) || await _portfolioRepository.Exists(row.PortfolioId);
            var instrumentExists = _existingInstrumentsCache.Contains(row.InstrumentId)
                                   || await _instrumentRepository.Exists(row.InstrumentId);

            if (portfolioExists) _existingPortfoliosCache.Add(row.PortfolioId);
            if (instrumentExists) _existingInstrumentsCache.Add(row.InstrumentId);

            if (!portfolioExists)
            {
                logEntry.AddError($"No portfolio with id {row.PortfolioId} found.");
            }

            if (!instrumentExists)
            {
                logEntry.AddError($"No instrument with id {row.InstrumentId} found.");
            }

            if (!portfolioExists || !instrumentExists) return logEntry;

            var existingPosition = row.Id != default
                ? await _positionRepository.FindAsync(row.Id)
                : null;

            if (existingPosition == null)
            {
                if (row.Price == null || row.Amount == null || row.Time == null)
                {
                    logEntry.AddError("Initial transaction data must be provided for a new position.");
                    return logEntry;
                }

                var position = new Position(row.PortfolioId, row.InstrumentId,
                    row.Note);
                position.AddTransaction((decimal)row.Amount, (decimal)row.Price, (DateTime)row.Time);

                _positionRepository.Add(position);
            }
            else
            {
                existingPosition.SetNote(row.Note);
                _positionRepository.Update(existingPosition);
            }

            return logEntry;
        }
    }
}
