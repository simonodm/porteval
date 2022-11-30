using Microsoft.EntityFrameworkCore;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Infrastructure.Repositories
{
    /// <inheritdoc cref="IInstrumentRepository"/>
    public class InstrumentRepository : IInstrumentRepository
    {
        public IUnitOfWork UnitOfWork => _context;
        private readonly PortEvalDbContext _context;

        public InstrumentRepository(PortEvalDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc cref="IInstrumentRepository.ListAllAsync"/>
        public async Task<IEnumerable<Instrument>> ListAllAsync()
        {
            return await _context.Instruments
                .AsNoTracking()
                .OrderBy(i => i.Symbol)
                .ToListAsync();
        }

        /// <inheritdoc cref="IInstrumentRepository.FindAsync"/>
        public async Task<Instrument> FindAsync(int id)
        {
            var instrumentEntity = await _context.Instruments
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);
            return instrumentEntity;
        }

        /// <inheritdoc cref="IInstrumentRepository.Add("/>
        public Instrument Add(Instrument instrument)
        {
            var newInstrument = _context.Instruments.Add(instrument).Entity;
            return newInstrument;
        }

        /// <inheritdoc cref="IInstrumentRepository.Update"/>
        public Instrument Update(Instrument instrument)
        {
            var updatedInstrument = _context.Instruments.Update(instrument).Entity;
            return updatedInstrument;
        }

        /// <inheritdoc cref="IInstrumentRepository.DeleteAsync"/>
        public async Task DeleteAsync(int instrumentId)
        {
            var foundInstrumentEntity = await _context.Instruments.FirstOrDefaultAsync(i => i.Id == instrumentId);
            if(foundInstrumentEntity != null)
            {
                var foundInstrumentLines = _context.ChartLines
                    .Where(line => EF.Property<string>(line, "Line_Type") == "Instrument" && (line as ChartLineInstrument).InstrumentId == instrumentId);
                var foundPositionLines = _context.ChartLines
                    .Where(line => EF.Property<string>(line, "Line_Type") == "Position")
                    .Join(_context.Positions.Where(pos => pos.InstrumentId == instrumentId), line => (line as ChartLinePosition).PositionId, position => position.Id, (line, position) => line);

                _context.ChartLines.RemoveRange(foundInstrumentLines);
                _context.ChartLines.RemoveRange(foundPositionLines);
                _context.Instruments.Remove(foundInstrumentEntity);
            }
        }

        /// <inheritdoc cref="IInstrumentRepository.ExistsAsync"/>
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Instruments.AnyAsync(i => i.Id == id);
        }
    }
}
