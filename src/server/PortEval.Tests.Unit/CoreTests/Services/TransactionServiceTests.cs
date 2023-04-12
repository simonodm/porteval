using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Core;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Core.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Events;
using PortEval.Domain.Models.Entities;
using PortEval.Tests.Unit.Helpers.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.Services
{
    public class TransactionServiceTests
    {
        private IFixture _fixture;
        private Mock<IPortfolioRepository> _portfolioRepository;
        private Mock<IPositionRepository> _positionRepository;
        private Mock<IInstrumentRepository> _instrumentRepository;
        private Mock<ITransactionQueries> _transactionQueries;

        public TransactionServiceTests()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            _portfolioRepository = _fixture.CreateDefaultPortfolioRepositoryMock();
            _positionRepository = _fixture.CreateDefaultPositionRepositoryMock();
            _instrumentRepository = _fixture.CreateDefaultInstrumentRepositoryMock();
            _transactionQueries = _fixture.CreateDefaultTransactionQueriesMock();
        }

        [Fact]
        public async Task GetTransactionsAsync_ReturnsCorrectPositionTransactions_WhenPositionExists()
        {
            var transactions = _fixture.CreateMany<TransactionDto>();
            var filter = _fixture.Build<TransactionFilters>().With(f => f.PositionId, _fixture.Create<int>()).Create();

            _transactionQueries
                .Setup(m => m.GetTransactionsAsync(filter, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(transactions);

            var sut = _fixture.Create<TransactionService>();
            var result = await sut.GetTransactionsAsync(filter, _fixture.Create<DateRangeParams>());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(transactions, result.Response);
        }

        [Fact]
        public async Task GetTransactionsAsync_ReturnsError_WhenPositionDoesNotExist()
        {
            var filter = _fixture.Build<TransactionFilters>().With(f => f.PositionId, _fixture.Create<int>()).Create();

            _positionRepository
                .Setup(m => m.ExistsAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            var sut = _fixture.Create<TransactionService>();
            var result = await sut.GetTransactionsAsync(filter, _fixture.Create<DateRangeParams>());

            Assert.Equal(OperationStatus.Error, result.Status);
        }

        [Fact]
        public async Task GetTransactionsAsync_ReturnsInstrumentTransactions_WhenInstrumentExists()
        {
            var transactions = _fixture.CreateMany<TransactionDto>();
            var filter = _fixture.Build<TransactionFilters>().With(f => f.InstrumentId, _fixture.Create<int>()).Create();

            _transactionQueries
                .Setup(m => m.GetTransactionsAsync(filter, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(transactions);

            var sut = _fixture.Create<TransactionService>();
            var result = await sut.GetTransactionsAsync(filter, _fixture.Create<DateRangeParams>());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(transactions, result.Response);
        }

        [Fact]
        public async Task GetTransactionsAsync_ReturnsError_WhenInstrumentDoesNotExist()
        {
            var filter = _fixture.Build<TransactionFilters>().With(f => f.InstrumentId, _fixture.Create<int>()).Create();

            _instrumentRepository
                .Setup(m => m.ExistsAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            var sut = _fixture.Create<TransactionService>();
            var result = await sut.GetTransactionsAsync(filter, _fixture.Create<DateRangeParams>());

            Assert.Equal(OperationStatus.Error, result.Status);
        }

        [Fact]
        public async Task GetTransactionsAsync_ReturnsPortfolioTransactions_WhenPortfolioExists()
        {
            var transactions = _fixture.CreateMany<TransactionDto>();
            var filter = _fixture.Build<TransactionFilters>().With(f => f.PositionId, _fixture.Create<int>()).Create();

            _transactionQueries
                .Setup(m => m.GetTransactionsAsync(filter, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(transactions);

            var sut = _fixture.Create<TransactionService>();
            var result = await sut.GetTransactionsAsync(filter, _fixture.Create<DateRangeParams>());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(transactions, result.Response);
        }

        [Fact]
        public async Task GetTransactionsAsync_ReturnsError_WhenPortfolioDoesNotExist()
        {
            var filter = _fixture.Build<TransactionFilters>().With(f => f.PortfolioId, _fixture.Create<int>()).Create();

            _portfolioRepository
                .Setup(m => m.ExistsAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            var sut = _fixture.Create<TransactionService>();
            var result = await sut.GetTransactionsAsync(filter, _fixture.Create<DateRangeParams>());

            Assert.Equal(OperationStatus.Error, result.Status);
        }

        [Fact]
        public async Task GetTransactionAsync_ReturnsTransaction_WhenItExists()
        {
            var transaction = _fixture.Create<TransactionDto>();

            _transactionQueries
                .Setup(m => m.GetTransactionAsync(transaction.Id))
                .ReturnsAsync(transaction);

            var sut = _fixture.Create<TransactionService>();
            var result = await sut.GetTransactionAsync(transaction.Id);

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(transaction, result.Response);
        }

        [Fact]
        public async Task GetTransactionAsync_ReturnsNotFound_WhenTransactionDoesNotExist()
        {
            var transactionId = _fixture.Create<int>();

            _transactionQueries
                .Setup(m => m.GetTransactionAsync(transactionId))
                .ReturnsAsync((TransactionDto)null);

            var sut = _fixture.Create<TransactionService>();
            var result = await sut.GetTransactionAsync(transactionId);

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task AddingTransaction_AddsTransactionToPosition_WhenWellFormed()
        {
            var position = _fixture.Create<Position>();
            var transaction = _fixture
                .Build<TransactionDto>()
                .With(t => t.PositionId, position.Id)
                .Create();

            _positionRepository
                .Setup(r => r.FindAsync(position.Id))
                .ReturnsAsync(position);

            var sut = _fixture.Create<TransactionService>();

            await sut.AddTransactionAsync(transaction);

            _positionRepository.Verify(r => r.Update(It.Is<Position>(p =>
                p.Id == transaction.PositionId &&
                p.Transactions.FirstOrDefault(t =>
                    t.Time == transaction.Time && t.Price == transaction.Price && t.Note == transaction.Note) != null
            )));
        }

        [Fact]
        public async Task AddingTransaction_ReturnsError_WhenPositionDoesNotExist()
        {
            var transaction = _fixture.Create<TransactionDto>();

            var positionRepository = _fixture.CreateDefaultPositionRepositoryMock();
            positionRepository
                .Setup(r => r.FindAsync(transaction.PositionId))
                .ReturnsAsync((Position)null);
            positionRepository
                .Setup(r => r.ExistsAsync(transaction.PositionId))
                .ReturnsAsync(false);
            _fixture.Freeze<Mock<IInstrumentPriceService>>();

            var sut = _fixture.Create<TransactionService>();
            var response = await sut.AddTransactionAsync(transaction);

            Assert.Equal(OperationStatus.Error, response.Status);
        }

        [Fact]
        public async Task AddingTransaction_EmitsTransactionAddedToPositionDomainEvent()
        {
            var position = _fixture.Create<Position>();
            var transaction = _fixture
                .Build<TransactionDto>()
                .With(t => t.PositionId, position.Id)
                .Create();

            _positionRepository
                .Setup(r => r.FindAsync(position.Id))
                .ReturnsAsync(position);

            var sut = _fixture.Create<TransactionService>();

            await sut.AddTransactionAsync(transaction);

            Assert.Collection(position.DomainEvents, evt =>
            {
                Assert.IsAssignableFrom<TransactionAddedToPositionDomainEvent>(evt);
            });
        }
    }
}