using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using PortEval.Application.Features.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Infrastructure.Repositories
{
    public class CurrencyExchangeRateRepository : ICurrencyExchangeRateRepository
    {
        public IUnitOfWork UnitOfWork => _context;
        private readonly PortEvalDbContext _context;

        public CurrencyExchangeRateRepository(PortEvalDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CurrencyExchangeRate>> ListExchangeRatesAsync(string currencyFrom)
        {
            return await _context.CurrencyExchangeRates
                .AsNoTracking()
                .Where(er => er.CurrencyFromCode == currencyFrom)
                .OrderByDescending(er => er.Time)
                .ToListAsync();
        }

        public async Task<CurrencyExchangeRate> GetExchangeRateAtAsync(string currencyFrom, string currencyTo, DateTime time)
        {
            return await _context.CurrencyExchangeRates
                .AsNoTracking()
                .Where(er => er.CurrencyFromCode == currencyFrom && er.CurrencyToCode == currencyTo)
                .Where(er => er.Time <= time)
                .OrderByDescending(er => er.Time)
                .FirstOrDefaultAsync();
        }

        public CurrencyExchangeRate Add(CurrencyExchangeRate exchangeRate)
        {
            return _context.CurrencyExchangeRates.Add(exchangeRate).Entity;
        }

        public async Task BulkInsertAsync(IList<CurrencyExchangeRate> rates)
        {
            await _context.BulkInsertAsync(rates);
        }
    }
}
