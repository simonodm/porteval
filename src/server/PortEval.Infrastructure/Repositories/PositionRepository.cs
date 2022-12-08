using Microsoft.EntityFrameworkCore;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Infrastructure.Repositories
{
    public class PositionRepository : IPositionRepository
    {
        public IUnitOfWork UnitOfWork => _context;
        private PortEvalDbContext _context;

        public PositionRepository(PortEvalDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc cref="IPositionRepository.ListPortfolioPositionsAsync"/>
        public async Task<IEnumerable<Position>> ListPortfolioPositionsAsync(int portfolioId)
        {
            return await _context.Positions
                .Where(p => p.PortfolioId == portfolioId)
                .OrderBy(p => p.InstrumentId)
                .Include(p => p.Transactions)
                .ToListAsync();
        }

        /// <inheritdoc cref="IPositionRepository.FindAsync"/>
        public async Task<Position> FindAsync(int positionId)
        {
            var position = await _context.Positions
                .Include(p => p.Transactions)
                .FirstOrDefaultAsync(p => p.Id == positionId);

            return position;
        }

        /// <inheritdoc cref="IPositionRepository.FindParentPositionAsync"/>
        public async Task<Position> FindParentPositionAsync(int transactionId)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Position)
                .FirstOrDefaultAsync(t => t.Id == transactionId);

            return transaction?.Position;
        }

        /// <inheritdoc cref="IPositionRepository.Add"/>
        public Position Add(Position position)
        {
            var createdPosition = _context.Positions.Add(position).Entity;
            return createdPosition;
        }

        /// <inheritdoc cref="IPositionRepository.Update"/>
        public Position Update(Position position)
        {
            var updatedPosition = _context.Positions.Update(position).Entity;
            return updatedPosition;
        }

        /// <inheritdoc cref="IPositionRepository.DeleteAsync"/>
        public async Task DeleteAsync(int positionId)
        {
            var foundPosition = await _context.Positions.FirstOrDefaultAsync(p => p.Id == positionId);
            if(foundPosition != null)
            {
                Delete(foundPosition);
            }
        }

        /// <inheritdoc cref="IPositionRepository.Delete"/>
        public void Delete(Position position)
        {
            var foundPositionLines = _context.ChartLines
                .Where(line => EF.Property<string>(line, "Line_Type") == "Position" && (line as ChartLinePosition).PositionId == position.Id);
            _context.ChartLines.RemoveRange(foundPositionLines);
            _context.Positions.Remove(position);
        }

        /// <inheritdoc cref="IPositionRepository.ExistsAsync"/>
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Positions.AnyAsync(i => i.Id == id);
        }
    }
}
