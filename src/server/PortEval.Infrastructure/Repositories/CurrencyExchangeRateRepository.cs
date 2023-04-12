using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Infrastructure.Repositories
{
    /// <inheritdoc cref="ICurrencyExchangeRateRepository"/>
    public class CurrencyExchangeRateRepository : ICurrencyExchangeRateRepository
    {
        public IUnitOfWork UnitOfWork => _context;
        private readonly PortEvalDbContext _context;

        public CurrencyExchangeRateRepository(PortEvalDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CurrencyExchangeRate>> ListExchangeRatesAsync(string currencyFrom)
        {
            return await _context.CurrencyExchangeRates
                .AsNoTracking()
                .Where(er => er.CurrencyFromCode == currencyFrom)
                .OrderByDescending(er => er.Time)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<CurrencyExchangeRate> GetExchangeRateAtAsync(string currencyFrom, string currencyTo, DateTime time)
        {
            return await _context.CurrencyExchangeRates
                .AsNoTracking()
                .Where(er => er.CurrencyFromCode == currencyFrom && er.CurrencyToCode == currencyTo)
                .Where(er => er.Time <= time)
                .OrderByDescending(er => er.Time)
                .FirstOrDefaultAsync();
        }

        /// <inheritdoc />
        public CurrencyExchangeRate Add(CurrencyExchangeRate exchangeRate)
        {
            return _context.CurrencyExchangeRates.Add(exchangeRate).Entity;
        }

        /// <inheritdoc />
        public async Task BulkUpsertAsync(IList<CurrencyExchangeRate> rates)
        {
            var bulkConfig = new BulkConfig
            {
                UpdateByProperties = new List<string>
                {
                    nameof(CurrencyExchangeRate.CurrencyFromCode),
                    nameof(CurrencyExchangeRate.CurrencyToCode),
                    nameof(CurrencyExchangeRate.Time)
                },
                PropertiesToIncludeOnUpdate = new List<string> { nameof(CurrencyExchangeRate.Id) }
            };

            await _context.BulkInsertOrUpdateAsync(rates, bulkConfig);
        }
    }
}
