using Microsoft.EntityFrameworkCore;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Infrastructure.Repositories
{
    /// <inheritdoc cref="IInstrumentSplitRepository"/>
    public class InstrumentSplitRepository : IInstrumentSplitRepository
    {
        public IUnitOfWork UnitOfWork => _context;
        private readonly PortEvalDbContext _context;

        public InstrumentSplitRepository(PortEvalDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<InstrumentSplit>> ListInstrumentSplitsAsync(int instrumentId)
        {
            return await _context.InstrumentSplits
                .Where(s => s.InstrumentId == instrumentId)
                .OrderBy(s => s.Time)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<InstrumentSplit>> ListNonProcessedSplitsAsync()
        {
            return await _context.InstrumentSplits
                .Where(s => s.ProcessingStatus == InstrumentSplitProcessingStatus.NotProcessed)
                .OrderBy(s => s.Time)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<InstrumentSplit>> ListRollbackRequestedSplitsAsync()
        {
            return await _context.InstrumentSplits
                .Where(s => s.ProcessingStatus == InstrumentSplitProcessingStatus.RollbackRequested)
                .OrderBy(s => s.Time)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<InstrumentSplit> FindAsync(int splitId)
        {
            return await _context.InstrumentSplits.FirstOrDefaultAsync(s => s.Id == splitId);
        }

        /// <inheritdoc />
        public InstrumentSplit Add(InstrumentSplit instrumentSplit)
        {
            var entity = _context.InstrumentSplits.Add(instrumentSplit).Entity;
            return entity;
        }

        /// <inheritdoc />
        public InstrumentSplit Update(InstrumentSplit instrumentSplit)
        {
            var updatedSplit = _context.InstrumentSplits.Update(instrumentSplit).Entity;
            return updatedSplit;
        }

        /// <inheritdoc />
        public async Task DeleteAsync(int splitId)
        {
            var foundSplit = await _context.InstrumentSplits.FirstOrDefaultAsync(s => s.Id == splitId);
            if (foundSplit != default)
            {
                Delete(foundSplit);
            }
        }

        /// <inheritdoc />
        public void Delete(InstrumentSplit split)
        {
            _context.InstrumentSplits.Remove(split);
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync(int splitId)
        {
            return await _context.InstrumentSplits.Where(s => s.Id == splitId).AnyAsync();
        }
    }
}
