using Microsoft.EntityFrameworkCore;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Domain.Models.ValueObjects;
using PortEval.Infrastructure.Repositories;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Integration.RepositoryTests
{
    public class InstrumentSplitRepositoryTests : RepositoryTestBase
    {
        private readonly IInstrumentSplitRepository _splitRepository;

        private readonly Instrument _instrument;

        public InstrumentSplitRepositoryTests() : base()
        {
            _splitRepository = new InstrumentSplitRepository(DbContext);

            DbContext.Add(new Currency("USD", "US Dollar", "$", true));
            DbContext.Add(new Exchange("NASDAQ", "NASDAQ"));

            _instrument = new Instrument("Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");
            DbContext.Add(_instrument);
            DbContext.SaveChanges();
        }

        [Fact]
        public async Task ListInstrumentSplitsAsync_ReturnsInstrumentSplits()
        {
            var first = new InstrumentSplit(_instrument.Id, DateTime.UtcNow.AddDays(-2), new SplitRatio(3, 1));
            var second = new InstrumentSplit(_instrument.Id, DateTime.UtcNow.AddDays(-1), new SplitRatio(1, 3));

            DbContext.Add(first);
            DbContext.Add(second);
            await DbContext.SaveChangesAsync();

            var splits = await _splitRepository.ListInstrumentSplitsAsync(_instrument.Id);

            Assert.Collection(splits,
                s => AssertInstrumentSplitsAreEqual(first, s),
                s => AssertInstrumentSplitsAreEqual(second, s)
            );
        }

        [Fact]
        public async Task ListNonProcessedSplitsAsync_ReturnsNonProcessedSplits()
        {
            var processedSplit = new InstrumentSplit(_instrument.Id, DateTime.UtcNow.AddDays(-2), new SplitRatio(3, 1));
            processedSplit.MarkAsProcessed();

            var nonProcessedSplit =
                new InstrumentSplit(_instrument.Id, DateTime.UtcNow.AddDays(-1), new SplitRatio(2, 1));

            DbContext.Add(processedSplit);
            DbContext.Add(nonProcessedSplit);
            await DbContext.SaveChangesAsync();

            var nonProcessedSplits = await _splitRepository.ListNonProcessedSplitsAsync();

            Assert.Collection(nonProcessedSplits, s => AssertInstrumentSplitsAreEqual(nonProcessedSplit, s));
        }

        [Fact]
        public async Task ListRollbackRequestedSplitsAsync_ReturnsRollbackRequestedSplits()
        {
            var processedSplit = new InstrumentSplit(_instrument.Id, DateTime.UtcNow.AddDays(-2), new SplitRatio(3, 1));
            processedSplit.MarkAsProcessed();

            var nonProcessedSplit =
                new InstrumentSplit(_instrument.Id, DateTime.UtcNow.AddDays(-1), new SplitRatio(2, 1));

            var rollbackRequestedSplit = new InstrumentSplit(_instrument.Id, DateTime.UtcNow, new SplitRatio(99, 1));
            rollbackRequestedSplit.MarkAsProcessed();
            rollbackRequestedSplit.Rollback();

            DbContext.Add(processedSplit);
            DbContext.Add(nonProcessedSplit);
            DbContext.Add(rollbackRequestedSplit);
            await DbContext.SaveChangesAsync();

            var rollbackRequestedSplits = await _splitRepository.ListRollbackRequestedSplitsAsync();

            Assert.Collection(rollbackRequestedSplits, s => AssertInstrumentSplitsAreEqual(rollbackRequestedSplit, s));
        }

        [Fact]
        public async Task FindAsync_ReturnsSplit_WhenExists()
        {
            var split = new InstrumentSplit(_instrument.Id, DateTime.UtcNow.AddDays(-1), new SplitRatio(2, 1));

            DbContext.Add(split);
            await DbContext.SaveChangesAsync();

            var foundSplit = await _splitRepository.FindAsync(split.Id);

            AssertInstrumentSplitsAreEqual(split, foundSplit);
        }

        [Fact]
        public async Task Add_CreatesNewSplit()
        {
            var split = new InstrumentSplit(_instrument.Id, DateTime.UtcNow.AddDays(-1), new SplitRatio(2, 1));
            _splitRepository.Add(split);
            await _splitRepository.UnitOfWork.CommitAsync();

            var createdSplit = await DbContext.InstrumentSplits.FirstOrDefaultAsync();
            AssertInstrumentSplitsAreEqual(split, createdSplit);
        }

        [Fact]
        public async Task Update_UpdatesSplit()
        {
            var split = new InstrumentSplit(_instrument.Id, DateTime.UtcNow.AddDays(-1), new SplitRatio(2, 1));
            DbContext.Add(split);
            await DbContext.SaveChangesAsync();

            split.MarkAsProcessed();
            _splitRepository.Update(split);
            await _splitRepository.UnitOfWork.CommitAsync();

            var updatedSplit = await DbContext.InstrumentSplits.FirstOrDefaultAsync();
            AssertInstrumentSplitsAreEqual(split, updatedSplit);
        }

        [Fact]
        public async Task DeleteAsync_DeletesSplit_WhenItExists()
        {
            var split = new InstrumentSplit(_instrument.Id, DateTime.UtcNow.AddDays(-1), new SplitRatio(2, 1));
            DbContext.Add(split);
            await DbContext.SaveChangesAsync();

            await _splitRepository.DeleteAsync(split.Id);
            await _splitRepository.UnitOfWork.CommitAsync();

            var splitExists = await DbContext.InstrumentSplits.AnyAsync(s => s.Id == split.Id);

            Assert.False(splitExists);
        }

        [Fact]
        public async Task Delete_DeletesSplit_WhenItExists()
        {
            var split = new InstrumentSplit(_instrument.Id, DateTime.UtcNow.AddDays(-1), new SplitRatio(2, 1));
            DbContext.Add(split);
            await DbContext.SaveChangesAsync();

            _splitRepository.Delete(split);
            await _splitRepository.UnitOfWork.CommitAsync();

            var splitExists = await DbContext.InstrumentSplits.AnyAsync(s => s.Id == split.Id);

            Assert.False(splitExists);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenSplitExists()
        {
            var split = new InstrumentSplit(_instrument.Id, DateTime.UtcNow.AddDays(-1), new SplitRatio(2, 1));
            DbContext.Add(split);
            await DbContext.SaveChangesAsync();

            var splitExists = await _splitRepository.ExistsAsync(split.Id);

            Assert.True(splitExists);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsFalse_WhenSplitDoesNotExist()
        {
            var splitExists = await _splitRepository.ExistsAsync(999);
            Assert.False(splitExists);
        }

        private void AssertInstrumentSplitsAreEqual(InstrumentSplit expected, InstrumentSplit actual)
        {
            Assert.Equal(expected?.InstrumentId, actual?.InstrumentId);
            Assert.Equal(expected?.Time, actual?.Time);
            Assert.Equal(expected?.SplitRatio.Factor, actual?.SplitRatio.Factor);
        }
    }
}
