using Microsoft.EntityFrameworkCore;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Infrastructure.Repositories
{
    /// <inheritdoc cref="ICurrencyRepository"/>
    public class CurrencyRepository : ICurrencyRepository
    {
        public IUnitOfWork UnitOfWork => _context;
        private readonly PortEvalDbContext _context;

        public CurrencyRepository(PortEvalDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc cref="ICurrencyRepository.ListAllAsync"/>
        public async Task<IEnumerable<Currency>> ListAllAsync()
        {
            return await _context.Currencies
                .OrderBy(c => c.Code)
                .ToListAsync();
        }

        /// <inheritdoc cref="ICurrencyRepository.FindAsync"/>
        public async Task<Currency> FindAsync(string currencyCode)
        {
            var currencyEntity = await _context.Currencies
                .FirstOrDefaultAsync(c => c.Code == currencyCode);

            return currencyEntity;
        }

        /// <inheritdoc cref="ICurrencyRepository.GetDefaultCurrencyAsync"/>
        public async Task<Currency> GetDefaultCurrencyAsync()
        {
            var defaultCurrency = await _context.Currencies
                .Where(c => c.IsDefault)
                .FirstOrDefaultAsync();

            return defaultCurrency;
        }

        /// <inheritdoc cref="ICurrencyRepository.Update"/>
        public Currency Update(Currency currency)
        {
            return _context.Currencies.Update(currency).Entity;
        }

        /// <inheritdoc cref="ICurrencyRepository.ExistsAsync"/>
        public async Task<bool> ExistsAsync(string currencyCode)
        {
            return await _context.Currencies.AnyAsync(c => c.Code == currencyCode);
        }
    }
}
