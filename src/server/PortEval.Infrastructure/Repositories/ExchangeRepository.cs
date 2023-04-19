using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;

namespace PortEval.Infrastructure.Repositories;

/// <inheritdoc cref="IExchangeRepository" />
public class ExchangeRepository : IExchangeRepository
{
    private readonly PortEvalDbContext _context;

    public ExchangeRepository(PortEvalDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public IUnitOfWork UnitOfWork => _context;

    /// <inheritdoc />
    public async Task<IEnumerable<Exchange>> ListAllAsync()
    {
        return await _context.Exchanges
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Exchange> FindAsync(string exchangeSymbol)
    {
        return await _context.Exchanges
            .OrderBy(e => e.Symbol)
            .FirstOrDefaultAsync(e => e.Symbol == exchangeSymbol);
    }

    /// <inheritdoc />
    public Exchange Add(Exchange exchange)
    {
        var newExchange = _context.Exchanges.Add(exchange).Entity;
        return newExchange;
    }

    /// <inheritdoc />
    public Exchange Update(Exchange exchange)
    {
        var updatedExchange = _context.Exchanges.Update(exchange).Entity;
        return updatedExchange;
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(string exchangeSymbol)
    {
        return await _context.Exchanges.AnyAsync(e => e.Symbol == exchangeSymbol);
    }
}