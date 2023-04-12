using Microsoft.EntityFrameworkCore;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;
using PortEval.Infrastructure.Repositories;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Integration.RepositoryTests
{
    public class CurrencyExchangeRateRepositoryTests : RepositoryTestBase
    {
        private readonly ICurrencyExchangeRateRepository _exchangeRateRepository;

        public CurrencyExchangeRateRepositoryTests() : base()
        {
            _exchangeRateRepository = new CurrencyExchangeRateRepository(DbContext);

            DbContext.Add(new Currency("USD", "US Dollar", "$", true));
            DbContext.Add(new Currency("EUR", "European Euro", "€", false));
            DbContext.Add(new Currency("CZK", "Czech Koruna", "Kč", false));
            DbContext.SaveChanges();
        }

        [Fact]
        public async Task ListExchangeRatesAsync_ReturnsAllExchangeRatesFromProvidedCurrency()
        {
            var first = new CurrencyExchangeRate(DateTime.UtcNow, 2, "USD", "EUR");
            var second = new CurrencyExchangeRate(DateTime.UtcNow.AddDays(-1), 3, "USD", "CZK");
            DbContext.Add(first);
            DbContext.Add(second);
            await DbContext.SaveChangesAsync();

            var exchangeRates = await _exchangeRateRepository.ListExchangeRatesAsync("USD");

            Assert.Collection(exchangeRates, er => AssertExchangeRatesAreEqual(first, er), er => AssertExchangeRatesAreEqual(second, er));
        }

        [Fact]
        public async Task GetExchangeRateAtAsync_ReturnsLatestAvailableExchangeRateBetweenProvidedCurrencies()
        {
            var exchangeRate = new CurrencyExchangeRate(DateTime.UtcNow.AddDays(-1), 2, "USD", "EUR");
            DbContext.Add(exchangeRate);
            await DbContext.SaveChangesAsync();

            var retrievedExchangeRate = await _exchangeRateRepository.GetExchangeRateAtAsync("USD", "EUR", DateTime.UtcNow);

            AssertExchangeRatesAreEqual(exchangeRate, retrievedExchangeRate);
        }

        [Fact]
        public async Task Add_CreatesNewExchangeRate()
        {
            var exchangeRate = new CurrencyExchangeRate(DateTime.UtcNow.AddDays(-1), 2, "USD", "EUR");

            _exchangeRateRepository.Add(exchangeRate);
            await _exchangeRateRepository.UnitOfWork.CommitAsync();

            var createdExchangeRate = await DbContext.CurrencyExchangeRates.FirstOrDefaultAsync();

            AssertExchangeRatesAreEqual(createdExchangeRate, exchangeRate);
        }

        private void AssertExchangeRatesAreEqual(CurrencyExchangeRate expected, CurrencyExchangeRate actual)
        {
            Assert.Equal(expected?.CurrencyFromCode, actual?.CurrencyFromCode);
            Assert.Equal(expected?.CurrencyToCode, actual?.CurrencyToCode);
            Assert.Equal(expected?.Time, actual?.Time);
            Assert.Equal(expected?.ExchangeRate, actual?.ExchangeRate);
        }
    }
}
