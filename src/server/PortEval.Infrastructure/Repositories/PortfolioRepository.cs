﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;

namespace PortEval.Infrastructure.Repositories;

/// <inheritdoc cref="IPortfolioRepository" />
public class PortfolioRepository : IPortfolioRepository
{
    private readonly PortEvalDbContext _context;

    public PortfolioRepository(PortEvalDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public IUnitOfWork UnitOfWork => _context;

    /// <inheritdoc />
    public async Task<IEnumerable<Portfolio>> ListAllAsync()
    {
        return await _context.Portfolios
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Portfolio> FindAsync(int id)
    {
        var portfolio = await _context.Portfolios
            .FirstOrDefaultAsync(p => p.Id == id);

        return portfolio;
    }

    /// <inheritdoc />
    public Portfolio Add(Portfolio portfolio)
    {
        var createdPortfolio = _context.Portfolios.Add(portfolio).Entity;
        return createdPortfolio;
    }

    /// <inheritdoc />
    public Portfolio Update(Portfolio portfolio)
    {
        var updatedPortfolio = _context.Portfolios.Update(portfolio).Entity;
        return updatedPortfolio;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int portfolioId)
    {
        var foundPortfolioEntity = await _context.Portfolios.FirstOrDefaultAsync(p => p.Id == portfolioId);
        if (foundPortfolioEntity != null)
        {
            Delete(foundPortfolioEntity);
        }
    }

    /// <inheritdoc />
    public void Delete(Portfolio portfolio)
    {
        var foundPortfolioLines = _context.ChartLines
            .Where(line => EF.Property<string>(line, "Line_Type") == "Portfolio" &&
                           (line as ChartLinePortfolio).PortfolioId == portfolio.Id);
        var foundPositionLines = _context.ChartLines
            .Where(line => EF.Property<string>(line, "Line_Type") == "Position")
            .Join(_context.Positions, line => (line as ChartLinePosition).PositionId, position => position.Id,
                (line, position) => line);

        _context.ChartLines.RemoveRange(foundPortfolioLines);
        _context.ChartLines.RemoveRange(foundPositionLines);
        _context.Portfolios.Remove(portfolio);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Portfolios.AnyAsync(i => i.Id == id);
    }
}