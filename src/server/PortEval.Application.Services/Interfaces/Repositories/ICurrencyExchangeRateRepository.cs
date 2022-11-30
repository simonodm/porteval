using PortEval.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Services.Interfaces.Repositories
{
    public interface ICurrencyExchangeRateRepository : IRepository
    {
        public Task<IEnumerable<CurrencyExchangeRate>> ListExchangeRatesAsync(string currencyFrom);
        public Task<CurrencyExchangeRate> GetExchangeRateAtAsync(string currencyFrom, string currencyTo, DateTime time);
        public CurrencyExchangeRate Add(CurrencyExchangeRate exchangeRate);
        public Task BulkInsertAsync(IList<CurrencyExchangeRate> rates);
    }
}
