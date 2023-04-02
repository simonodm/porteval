using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Infrastructure.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Repositories;
using Xunit;

namespace PortEval.Tests.Integration.RepositoryTests
{
    public class PositionRepositoryTests : RepositoryTestBase
    {
        private readonly IPositionRepository _positionRepository;

        private readonly Instrument _firstInstrument;
        private readonly Instrument _secondInstrument;
        private readonly Portfolio _portfolio;

        public PositionRepositoryTests() : base()
        {
            _positionRepository = new PositionRepository(DbContext);

            DbContext.Add(new Currency("USD", "US Dollar", "$", true));
            DbContext.Add(new Exchange("NASDAQ", "NASDAQ"));

            _portfolio = new Portfolio("Test", "", "USD");
            _firstInstrument = new Instrument("Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");
            _secondInstrument = new Instrument("Alphabet Inc.", "GOOGL", "NASDAQ", InstrumentType.Stock, "USD", "");

            DbContext.Add(_portfolio);
            DbContext.Add(_firstInstrument);
            DbContext.Add(_secondInstrument);
            DbContext.SaveChanges();
        }

        [Fact]
        public async Task ListPortfolioPositionsAsync_ReturnsAllPortfoliosPositions()
        {
            var first = new Position(_portfolio.Id, _firstInstrument.Id, "");
            var second = new Position(_portfolio.Id, _secondInstrument.Id, "TestNote");

            DbContext.Add(first);
            DbContext.Add(second);
            await DbContext.SaveChangesAsync();

            var positions = await _positionRepository.ListPortfolioPositionsAsync(_portfolio.Id);

            Assert.Collection(positions, position => AssertPositionsAreEqual(first, position),
                position => AssertPositionsAreEqual(second, position));
        }

        [Fact]
        public async Task FindAsync_ReturnsCorrectPosition()
        {
            var position = new Position(_portfolio.Id, _firstInstrument.Id, "");
            DbContext.Add(position);
            await DbContext.SaveChangesAsync();

            var foundPosition = await _positionRepository.FindAsync(position.Id);

            AssertPositionsAreEqual(foundPosition, position);
        }

        [Fact]
        public async Task Add_CreatesNewPosition()
        {
            var position = new Position(_portfolio.Id, _firstInstrument.Id, "");

            _positionRepository.Add(position);
            await _positionRepository.UnitOfWork.CommitAsync();

            var createdPosition = DbContext.Positions.FirstOrDefault();

            AssertPositionsAreEqual(position, createdPosition);
        }

        [Fact]
        public async Task Add_CreatesPositionsTransactions()
        {
            var position = new Position(_portfolio.Id, _firstInstrument.Id, "");
            var transaction = position.AddTransaction(1m, 100m, DateTime.UtcNow, "");

            _positionRepository.Add(position);
            await _positionRepository.UnitOfWork.CommitAsync();

            var createdTransaction = DbContext.Transactions.FirstOrDefault();

            AssertTransactionsAreEqual(transaction, createdTransaction);
        }

        [Fact]
        public async Task Update_UpdatesPosition()
        {
            var position = new Position(_portfolio.Id, _firstInstrument.Id, "");
            DbContext.Add(position);
            await DbContext.SaveChangesAsync();

            position.SetNote("TEST");

            _positionRepository.Update(position);
            await _positionRepository.UnitOfWork.CommitAsync();

            var updatedPosition = DbContext.Positions.FirstOrDefault();

            AssertPositionsAreEqual(position, updatedPosition);
        }

        [Fact]
        public async Task Update_UpdatesPositionsTransactions()
        {
            var position = new Position(_portfolio.Id, _firstInstrument.Id, "");
            DbContext.Add(position);
            await DbContext.SaveChangesAsync();

            position.SetNote("TEST");
            var transaction = position.AddTransaction(1m, 100m, DateTime.UtcNow, "");

            _positionRepository.Update(position);
            await _positionRepository.UnitOfWork.CommitAsync();

            var updatedTransaction = DbContext.Transactions.FirstOrDefault();

            AssertTransactionsAreEqual(transaction, updatedTransaction);
        }

        [Fact]
        public async Task DeleteAsync_DeletesPosition()
        {
            var position = new Position(_portfolio.Id, _firstInstrument.Id, "");
            DbContext.Add(position);
            await DbContext.SaveChangesAsync();

            await _positionRepository.DeleteAsync(position.Id);
            await _positionRepository.UnitOfWork.CommitAsync();

            var positionDeleted = !DbContext.Positions.Any();

            Assert.True(positionDeleted);
        }

        [Fact]
        public async Task Delete_DeletesPosition()
        {
            var position = new Position(_portfolio.Id, _firstInstrument.Id, "");
            DbContext.Add(position);
            await DbContext.SaveChangesAsync();

            _positionRepository.Delete(position);
            await _positionRepository.UnitOfWork.CommitAsync();

            var positionDeleted = !DbContext.Positions.Any();

            Assert.True(positionDeleted);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenPositionExists()
        {
            var position = new Position(_portfolio.Id, _firstInstrument.Id, "");
            DbContext.Positions.Add(position);
            await DbContext.SaveChangesAsync();

            var exists = await _positionRepository.ExistsAsync(position.Id);

            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsAsyncb_ReturnsFalse_WhenPositionDoesNotExist()
        {
            var exists = await _positionRepository.ExistsAsync(1);

            Assert.False(exists);
        }

        private void AssertPositionsAreEqual(Position expected, Position actual)
        {
            Assert.Equal(expected?.PortfolioId, actual?.PortfolioId);
            Assert.Equal(expected?.InstrumentId, actual?.InstrumentId);
            Assert.Equal(expected?.Note, actual?.Note);
            Assert.Equal(expected?.Transactions, actual?.Transactions);
        }

        private void AssertTransactionsAreEqual(Transaction expected, Transaction actual)
        {
            Assert.Equal(expected?.PositionId, actual?.PositionId);
            Assert.Equal(expected?.Amount, actual?.Amount);
            Assert.Equal(expected?.Price, actual?.Price);
            Assert.Equal(expected?.Time, actual?.Time);
            Assert.Equal(expected?.Note, actual?.Note);
        }
    }
}
