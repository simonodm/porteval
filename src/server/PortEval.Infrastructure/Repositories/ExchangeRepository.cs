using Microsoft.EntityFrameworkCore;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Infrastructure.Repositories
{
    /// <summary>
    /// Represents a persistently stored collection of exchanges.
    /// </summary>
    public class ExchangeRepository : IExchangeRepository
    {
        public IUnitOfWork UnitOfWork => _context;
        private readonly PortEvalDbContext _context;

        public ExchangeRepository(PortEvalDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Exchange>> ListAllAsync()
        {
            return await _context.Exchanges
                .AsNoTracking()
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Exchange> FindAsync(string exchangeSymbol)
        {
            return await _context.Exchanges
                .AsNoTracking()
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
}
