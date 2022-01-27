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

        /// <inheritdoc cref="IPositionRepository.ListAsync"/>
        public async Task<IEnumerable<Position>> ListAsync(int portfolioId)
        {
            return await _context.Positions
                .Where(p => p.PortfolioId == portfolioId)
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

        /// <inheritdoc cref="IPositionRepository.FindParentPosition"/>
        public async Task<Position> FindParentPosition(int transactionId)
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

        /// <inheritdoc cref="IPositionRepository.Delete"/>
        public async Task Delete(int positionId)
        {
            var foundPosition = await _context.Positions.FirstOrDefaultAsync(p => p.Id == positionId);
            if(foundPosition != null)
            {
                var foundPositionLines = _context.ChartLines
                    .Where(line => EF.Property<string>(line, "Line_Type") == "Position" && (line as ChartLinePosition).PositionId == foundPosition.Id);
                _context.ChartLines.RemoveRange(foundPositionLines);
                _context.Positions.Remove(foundPosition);
            }
        }

        /// <inheritdoc cref="IPositionRepository.Exists"/>
        public async Task<bool> Exists(int id)
        {
            return await _context.Positions.AnyAsync(i => i.Id == id);
        }
    }
}
