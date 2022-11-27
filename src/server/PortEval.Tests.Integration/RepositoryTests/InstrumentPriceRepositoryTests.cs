using Microsoft.EntityFrameworkCore;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Integration.RepositoryTests
{
    public class InstrumentPriceRepositoryTests : RepositoryTestBase
    {
        private readonly IInstrumentPriceRepository _instrumentPriceRepository;

        private readonly Instrument _instrument;

        public InstrumentPriceRepositoryTests() : base()
        {
            _instrumentPriceRepository = new InstrumentPriceRepository(DbContext);

            DbContext.Add(new Currency("USD", "US Dollar", "$", true));
            DbContext.Add(new Exchange("NASDAQ", "NASDAQ"));

            _instrument = new Instrument("Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");
            DbContext.Add(_instrument);
            DbContext.SaveChanges();
        }

        [Fact]
        public async Task ListInstrumentPricesAsync_ReturnsInstrumentPrices()
        {
            var first = new InstrumentPrice(DateTime.UtcNow, 100m, _instrument.Id);
            var second = new InstrumentPrice(DateTime.UtcNow.AddMinutes(-5), 99m, _instrument.Id);
            DbContext.Add(first);
            DbContext.Add(second);
            await DbContext.SaveChangesAsync();

            var prices = await _instrumentPriceRepository.ListInstrumentPricesAsync(_instrument.Id);

            Assert.Collection(prices, price => AssertInstrumentPricesAreEqual(second, price), price => AssertInstrumentPricesAreEqual(first, price));
        }

        [Fact]
        public async Task FindPriceAtAsync_ReturnsCorrectInstrumentPrice()
        {
            var instrumentPrice = new InstrumentPrice(DateTime.UtcNow, 100m, _instrument.Id);
            DbContext.Add(instrumentPrice);
            await DbContext.SaveChangesAsync();

            var foundInstrumentPrice = await _instrumentPriceRepository.FindPriceAtAsync(instrumentPrice.InstrumentId, DateTime.UtcNow);

            AssertInstrumentPricesAreEqual(instrumentPrice, foundInstrumentPrice);
        }

        [Fact]
        public async Task FindPriceByIdAsync_ReturnsCorrectInstrumentPrice()
        {
            var instrumentPrice = new InstrumentPrice(DateTime.UtcNow, 100m, _instrument.Id);
            DbContext.Add(instrumentPrice);
            await DbContext.SaveChangesAsync();

            var foundInstrumentPrice = await _instrumentPriceRepository.FindPriceByIdAsync(instrumentPrice.InstrumentId, instrumentPrice.Id);

            AssertInstrumentPricesAreEqual(instrumentPrice, foundInstrumentPrice);
        }

        [Fact]
        public async Task Add_CreatesNewInstrumentPrice()
        {
            var instrumentPrice = new InstrumentPrice(DateTime.UtcNow, 100m, _instrument.Id);

            _instrumentPriceRepository.Add(instrumentPrice);
            await _instrumentPriceRepository.UnitOfWork.CommitAsync();

            var createdInstrumentPrice = DbContext.InstrumentPrices.FirstOrDefault();

            AssertInstrumentPricesAreEqual(instrumentPrice, createdInstrumentPrice);
        }

        [Fact]
        public async Task DeleteAsync_DeletesInstrumentPrice()
        {
            var instrumentPrice = new InstrumentPrice(DateTime.UtcNow, 100m, _instrument.Id);
            DbContext.Add(instrumentPrice);
            await DbContext.SaveChangesAsync();

            await _instrumentPriceRepository.DeleteAsync(instrumentPrice.InstrumentId, instrumentPrice.Id);
            await _instrumentPriceRepository.UnitOfWork.CommitAsync();

            var instrumentPriceDeleted = !DbContext.InstrumentPrices.Any();

            Assert.True(instrumentPriceDeleted);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenInstrumentPriceWithSpecifiedIdExists()
        {
            var instrumentPrice = new InstrumentPrice(DateTime.UtcNow, 100m, _instrument.Id);
            DbContext.InstrumentPrices.Add(instrumentPrice);
            await DbContext.SaveChangesAsync();

            var exists = await _instrumentPriceRepository.ExistsAsync(instrumentPrice.InstrumentId, instrumentPrice.Id);

            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenInstrumentPriceAtSpecifiedTimeExists()
        {
            var instrumentPrice = new InstrumentPrice(DateTime.UtcNow, 100m, _instrument.Id);
            DbContext.InstrumentPrices.Add(instrumentPrice);
            await DbContext.SaveChangesAsync();

            var exists = await _instrumentPriceRepository.ExistsAsync(instrumentPrice.InstrumentId, instrumentPrice.Time);

            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsFalse_WhenInstrumentPriceWithSpecifiedIdDoesNotExist()
        {
            var exists = await _instrumentPriceRepository.ExistsAsync(_instrument.Id, 1);

            Assert.False(exists);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsFalse_WhenInstrumentPriceAtSpecifiedTimeDoesNotExist()
        {
            var exists = await _instrumentPriceRepository.ExistsAsync(_instrument.Id, DateTime.UtcNow);

            Assert.False(exists);
        }

        private void AssertInstrumentPricesAreEqual(InstrumentPrice expected, InstrumentPrice actual)
        {
            Assert.Equal(expected?.InstrumentId, actual?.InstrumentId);
            Assert.Equal(expected?.Time, actual?.Time);
            Assert.Equal(expected?.Price, actual?.Price);
        }
    }
}
