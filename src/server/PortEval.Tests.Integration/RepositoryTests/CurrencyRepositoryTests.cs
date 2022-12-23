using PortEval.Application.Features.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;
using PortEval.Infrastructure.Repositories;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Integration.RepositoryTests
{
    public class CurrencyRepositoryTests : RepositoryTestBase
    {
        private readonly ICurrencyRepository _currencyRepository;

        public CurrencyRepositoryTests() : base()
        {
            _currencyRepository = new CurrencyRepository(DbContext);
        }

        [Fact]
        public async Task ListAllAsync_ReturnsAllCurrencies()
        {
            var usd = new Currency("USD", "US Dollar", "$", true);
            var eur = new Currency("EUR", "European Euro", "€");
            DbContext.Add(usd);
            DbContext.Add(eur);
            await DbContext.SaveChangesAsync();

            var currencies = await _currencyRepository.ListAllAsync();

            Assert.Collection(currencies, currency => AssertCurrenciesAreEqual(eur, currency),
                currency => AssertCurrenciesAreEqual(usd, currency));
        }

        [Fact]
        public async Task FindAsync_ReturnsCorrectCurrency()
        {
            var usd = new Currency("USD", "US Dollar", "$", true);
            DbContext.Add(usd);
            await DbContext.SaveChangesAsync();

            var currency = await _currencyRepository.FindAsync("USD");

            AssertCurrenciesAreEqual(usd, currency);
        }

        [Fact]
        public async Task GetDefaultCurrencyAsync_ReturnsDefaultCurrency()
        {
            var usd = new Currency("USD", "US Dollar", "$", true);
            DbContext.Add(usd);
            await DbContext.SaveChangesAsync();

            var currency = await _currencyRepository.GetDefaultCurrencyAsync();

            AssertCurrenciesAreEqual(usd, currency);
        }

        [Fact]
        public async Task Update_UpdatesCurrency()
        {
            var usd = new Currency("USD", "US Dollar", "$", true);
            DbContext.Add(usd);
            await DbContext.SaveChangesAsync();

            usd.UnsetDefault();

            _currencyRepository.Update(usd);
            await _currencyRepository.UnitOfWork.CommitAsync();

            var updatedCurrency = DbContext.Currencies.FirstOrDefault(c => c.Code == "USD");

            AssertCurrenciesAreEqual(usd, updatedCurrency);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenCurrencyExists()
        {
            DbContext.Add(new Currency("USD", "US Dollar", "$", true));
            await DbContext.SaveChangesAsync();

            var exists = await _currencyRepository.ExistsAsync("USD");

            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsFalse_WhenCurrencyDoesNotExist()
        {
            var exists = await _currencyRepository.ExistsAsync("USD");

            Assert.False(exists);
        }

        private void AssertCurrenciesAreEqual(Currency expected, Currency actual)
        {
            Assert.Equal(expected?.Symbol, actual?.Symbol);
            Assert.Equal(expected?.Name, actual?.Name);
            Assert.Equal(expected?.Code, actual?.Code);
            Assert.Equal(expected?.IsDefault, actual?.IsDefault);
        }
    }
}
