using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;

namespace PortEval.Application.Services.BulkImportExport
{
    public class TransactionImportProcessor : ImportProcessor<TransactionDto, TransactionDtoValidator>
    {
        private readonly IPositionRepository _positionRepository;
        private readonly Dictionary<int, Position> _positionCache;

        public TransactionImportProcessor(IPositionRepository positionRepository) : base(positionRepository.UnitOfWork)
        {
            _positionRepository = positionRepository;
            _positionCache = new Dictionary<int, Position>();
        }

        public override async Task<ErrorLogEntry<TransactionDto>> ProcessItem(TransactionDto row)
        {
            var logEntry = new ErrorLogEntry<TransactionDto>(row);

            Position position;
            if (_positionCache.ContainsKey(row.PositionId))
            {
                position = _positionCache[row.PositionId];
            }
            else
            {
                position = await _positionRepository.FindAsync(row.PositionId);
                if (position != null) _positionCache[row.PositionId] = position;
            }

            if (position == null)
            {
                logEntry.AddError($"No position with id {row.PositionId} found.");
                return logEntry;
            }

            var existingTransaction = row.Id != default
                ? position.FindTransaction(row.Id)
                : null;

            if (existingTransaction != null)
            {
                existingTransaction.SetNote(row.Note);
            }
            else
            {
                position.AddTransaction(row.Amount, row.Price, row.Time,
                    row.Note);
            }

            _positionRepository.Update(position);
            return logEntry;
        }
    }
}
