using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using PortEval.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Repositories;

namespace PortEval.Infrastructure.Repositories
{
    /// <inheritdoc cref="IInstrumentPriceRepository" />
    public class InstrumentPriceRepository : IInstrumentPriceRepository
    {
        public IUnitOfWork UnitOfWork => _context;
        private readonly PortEvalDbContext _context;

        public InstrumentPriceRepository(PortEvalDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<InstrumentPrice>> ListInstrumentPricesAsync(int instrumentId)
        {
            return await _context.InstrumentPrices
                .Where(price => price.InstrumentId == instrumentId)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<InstrumentPrice> FindPriceByIdAsync(int instrumentId, int priceId)
        {
            return await _context.InstrumentPrices
                .Where(price => price.InstrumentId == instrumentId && price.Id == priceId)
                .FirstOrDefaultAsync();
        }

        /// <inheritdoc />
        public async Task<InstrumentPrice> FindPriceAtAsync(int instrumentId, DateTime time)
        {
            return await _context.InstrumentPrices
                .OrderByDescending(price => price.Time)
                .FirstOrDefaultAsync(price => price.InstrumentId == instrumentId && price.Time <= time);
        }

        /// <inheritdoc />
        public InstrumentPrice Add(InstrumentPrice price)
        {
            return _context.InstrumentPrices.Add(price).Entity;
        }

        /// <inheritdoc />
        public InstrumentPrice Update(InstrumentPrice price)
        {
            return _context.InstrumentPrices.Update(price).Entity;
        }

        /// <inheritdoc />
        public async Task BulkUpsertAsync(IList<InstrumentPrice> prices)
        {
            var bulkConfig = new BulkConfig
            {
                UpdateByProperties = new List<string> { nameof(InstrumentPrice.InstrumentId), nameof(InstrumentPrice.Time) },
                PropertiesToIncludeOnUpdate = new List<string> { nameof(InstrumentPrice.Id) }
            };

            await _context.BulkInsertOrUpdateAsync(prices, bulkConfig);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(int instrumentId, int priceId)
        {
            var foundPrice = await _context.InstrumentPrices
                .FirstOrDefaultAsync(price => price.InstrumentId == instrumentId && price.Id == priceId);

            if (foundPrice != null)
            {
                Delete(foundPrice);
            }
        }

        /// <inheritdoc />
        public void Delete(InstrumentPrice price)
        {
            _context.InstrumentPrices.Remove(price);
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync(int instrumentId, int priceId)
        {
            return await _context.InstrumentPrices
                .AnyAsync(price => price.Id == priceId && price.InstrumentId == instrumentId);
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync(int instrumentId, DateTime time)
        {
            return await _context.InstrumentPrices
                .AnyAsync(price => price.InstrumentId == instrumentId && price.Time == time);
        }
    }
}
