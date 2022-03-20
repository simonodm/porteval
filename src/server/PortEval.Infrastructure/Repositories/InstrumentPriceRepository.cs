using System;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<InstrumentPrice> FindPriceAsync(int instrumentId, int priceId)
        {
            return await _context.InstrumentPrices
                .Where(price => price.InstrumentId == instrumentId && price.Id == priceId)
                .FirstOrDefaultAsync();
        }

        public async Task<InstrumentPrice> FindPriceAt(int instrumentId, DateTime time)
        {
            return await _context.InstrumentPrices
                .OrderByDescending(price => price.Time)
                .FirstOrDefaultAsync(price => price.InstrumentId == instrumentId && price.Time <= time);
        }

        public InstrumentPrice AddInstrumentPrice(InstrumentPrice price)
        {
            return _context.InstrumentPrices.Add(price).Entity;
        }

        public async Task DeleteInstrumentPriceAsync(int instrumentId, int priceId)
        {
            var foundPrice = await _context.InstrumentPrices
                .FirstOrDefaultAsync(price => price.InstrumentId == instrumentId && price.Id == priceId);

            if (foundPrice != null)
            {
                _context.InstrumentPrices.Remove(foundPrice);
            }
        }

        public async Task<bool> Exists(int instrumentId, int priceId)
        {
            return await _context.InstrumentPrices
                .AnyAsync(price => price.Id == priceId && price.InstrumentId == instrumentId);
        }

        public async Task<bool> Exists(int instrumentId, DateTime time)
        {
            return await _context.InstrumentPrices
                .AnyAsync(price => price.InstrumentId == instrumentId && price.Time == time);
        }
    }
}
