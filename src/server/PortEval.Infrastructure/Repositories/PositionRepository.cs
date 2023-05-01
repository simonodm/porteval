using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;

namespace PortEval.Infrastructure.Repositories;

/// <inheritdoc cref="IPositionRepository" />
public class PositionRepository : IPositionRepository
{
    private readonly PortEvalDbContext _context;

    public PositionRepository(PortEvalDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public IUnitOfWork UnitOfWork => _context;

    /// <inheritdoc />
    public async Task<IEnumerable<Position>> ListPortfolioPositionsAsync(int portfolioId)
    {
        return await _context.Positions
            .Where(p => p.PortfolioId == portfolioId)
            .OrderBy(p => p.InstrumentId)
            .Include(p => p.Transactions)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Position>> ListAllInstrumentPositionsAsync(int instrumentId)
    {
        return await _context.Positions
            .Where(p => p.InstrumentId == instrumentId)
            .Include(p => p.Transactions)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Position> FindAsync(int positionId)
    {
        var position = await _context.Positions
            .Include(p => p.Transactions)
            .FirstOrDefaultAsync(p => p.Id == positionId);

        return position;
    }

    /// <inheritdoc />
    public async Task<Position> FindParentPositionAsync(int transactionId)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Position)
            .FirstOrDefaultAsync(t => t.Id == transactionId);

        return transaction?.Position;
    }

    /// <inheritdoc />
    public Position Add(Position position)
    {
        var createdPosition = _context.Positions.Add(position).Entity;
        return createdPosition;
    }

    /// <inheritdoc />
    public Position Update(Position position)
    {
        var updatedPosition = _context.Positions.Update(position).Entity;
        return updatedPosition;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int positionId)
    {
        var foundPosition = await _context.Positions.FirstOrDefaultAsync(p => p.Id == positionId);
        if (foundPosition != null)
        {
            Delete(foundPosition);
        }
    }

    /// <inheritdoc />
    public void Delete(Position position)
    {
        var foundPositionLines = _context.ChartLines
            .Where(line => EF.Property<string>(line, "Line_Type") == "Position" &&
                           (line as ChartLinePosition).PositionId == position.Id);
        _context.ChartLines.RemoveRange(foundPositionLines);
        _context.Positions.Remove(position);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Positions.AnyAsync(i => i.Id == id);
    }
}