using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;

namespace PortEval.Infrastructure.Repositories
{
    public class InstrumentPriceRepository : IInstrumentPriceRepository
    {
        public IUnitOfWork UnitOfWork => _context;
        private readonly PortEvalDbContext _context;

        public InstrumentPriceRepository(PortEvalDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InstrumentPrice>> ListInstrumentPricesAsync(int instrumentId)
        {
            return await _context.InstrumentPrices
                .AsNoTracking()
                .Where(price => price.InstrumentId == instrumentId)
                .ToListAsync();
        }

        public async Task<InstrumentPrice> FindPriceByIdAsync(int instrumentId, int priceId)
        {
            return await _context.InstrumentPrices
                .AsNoTracking()
                .Where(price => price.InstrumentId == instrumentId && price.Id == priceId)
                .FirstOrDefaultAsync();
        }

        public async Task<InstrumentPrice> FindPriceAtAsync(int instrumentId, DateTime time)
        {
            return await _context.InstrumentPrices
                .AsNoTracking()
                .OrderByDescending(price => price.Time)
                .FirstOrDefaultAsync(price => price.InstrumentId == instrumentId && price.Time <= time);
        }

        public InstrumentPrice Add(InstrumentPrice price)
        {
            return _context.InstrumentPrices.Add(price).Entity;
        }

        public async Task BulkInsertAsync(IList<InstrumentPrice> prices)
        {
            await _context.BulkInsertAsync(prices);
        }

        public async Task DeleteAsync(int instrumentId, int priceId)
        {
            var foundPrice = await _context.InstrumentPrices
                .FirstOrDefaultAsync(price => price.InstrumentId == instrumentId && price.Id == priceId);

            if (foundPrice != null)
            {
                _context.InstrumentPrices.Remove(foundPrice);
            }
        }

        public async Task<bool> ExistsAsync(int instrumentId, int priceId)
        {
            return await _context.InstrumentPrices
                .AnyAsync(price => price.Id == priceId && price.InstrumentId == instrumentId);
        }

        public async Task<bool> ExistsAsync(int instrumentId, DateTime time)
        {
            return await _context.InstrumentPrices
                .AnyAsync(price => price.InstrumentId == instrumentId && price.Time == time);
        }
    }
}
