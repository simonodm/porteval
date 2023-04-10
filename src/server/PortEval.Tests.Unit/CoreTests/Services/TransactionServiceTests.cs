using System;
using System.Linq;
using System.Threading.Tasks;
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
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.Services
{
    public class TransactionServiceTests
    {
        [Fact]
        public async Task GetTransactionsAsync_ReturnsCorrectPositionTransactions_WhenPositionExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var transactions = fixture.CreateMany<TransactionDto>();
            var filter = fixture.Build<TransactionFilters>().With(f => f.PositionId, fixture.Create<int>()).Create();

            var positionQueries = fixture.Freeze<Mock<IPositionQueries>>();
            positionQueries
                .Setup(m => m.GetPositionAsync((int)filter.PositionId))
                .ReturnsAsync(fixture.Create<PositionDto>());

            var transactionQueries = fixture.Freeze<Mock<ITransactionQueries>>();
            transactionQueries
                .Setup(m => m.GetTransactionsAsync(filter, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(transactions);

            var sut = fixture.Create<TransactionService>();
            var result = await sut.GetTransactionsAsync(filter, fixture.Create<DateRangeParams>());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(transactions, result.Response);
        }

        [Fact]
        public async Task GetTransactionsAsync_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var filter = fixture.Build<TransactionFilters>().With(f => f.PositionId, fixture.Create<int>()).Create();

            var positionQueries = fixture.Freeze<Mock<IPositionQueries>>();
            positionQueries
                .Setup(m => m.GetPositionAsync((int)filter.PositionId))
                .ReturnsAsync((PositionDto)null);

            var sut = fixture.Create<TransactionService>();
            var result = await sut.GetTransactionsAsync(filter, fixture.Create<DateRangeParams>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetTransactionsAsync_ReturnsInstrumentTransactions_WhenInstrumentExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var transactions = fixture.CreateMany<TransactionDto>();
            var filter = fixture.Build<TransactionFilters>().With(f => f.InstrumentId, fixture.Create<int>()).Create();

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.GetInstrumentAsync((int)filter.InstrumentId))
                .ReturnsAsync(fixture.Create<InstrumentDto>());

            var transactionQueries = fixture.Freeze<Mock<ITransactionQueries>>();
            transactionQueries
                .Setup(m => m.GetTransactionsAsync(filter, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(transactions);

            var sut = fixture.Create<TransactionService>();
            var result = await sut.GetTransactionsAsync(filter, fixture.Create<DateRangeParams>());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(transactions, result.Response);
        }

        [Fact]
        public async Task GetTransactionsAsync_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var filter = fixture.Build<TransactionFilters>().With(f => f.InstrumentId, fixture.Create<int>()).Create();

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.GetInstrumentAsync((int)filter.InstrumentId))
                .ReturnsAsync((InstrumentDto)null);

            var sut = fixture.Create<TransactionService>();
            var result = await sut.GetTransactionsAsync(filter, fixture.Create<DateRangeParams>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetTransactionsAsync_ReturnsPortfolioTransactions_WhenPortfolioExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var transactions = fixture.CreateMany<TransactionDto>();
            var filter = fixture.Build<TransactionFilters>().With(f => f.PositionId, fixture.Create<int>()).Create();

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.GetPortfolioAsync((int)filter.PortfolioId))
                .ReturnsAsync(fixture.Create<PortfolioDto>());

            var transactionQueries = fixture.Freeze<Mock<ITransactionQueries>>();
            transactionQueries
                .Setup(m => m.GetTransactionsAsync(filter, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(transactions);

            var sut = fixture.Create<TransactionService>();
            var result = await sut.GetTransactionsAsync(filter, fixture.Create<DateRangeParams>());

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(transactions, result.Response);
        }

        [Fact]
        public async Task GetTransactionsAsync_ReturnsNotFound_WhenPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var filter = fixture.Build<TransactionFilters>().With(f => f.PortfolioId, fixture.Create<int>()).Create();

            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.GetPortfolioAsync((int)filter.PortfolioId))
                .ReturnsAsync((PortfolioDto)null);

            var sut = fixture.Create<TransactionService>();
            var result = await sut.GetTransactionsAsync(filter, fixture.Create<DateRangeParams>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetTransactionAsync_ReturnsTransaction_WhenItExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var transaction = fixture.Create<TransactionDto>();

            var transactionQueries = fixture.Freeze<Mock<ITransactionQueries>>();
            transactionQueries
                .Setup(m => m.GetTransactionAsync(transaction.Id))
                .ReturnsAsync(transaction);

            var sut = fixture.Create<TransactionService>();
            var result = await sut.GetTransactionAsync(transaction.Id);

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(transaction, result.Response);
        }

        [Fact]
        public async Task GetTransactionAsync_ReturnsNotFound_WhenTransactionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var transactionId = fixture.Create<int>();

            var transactionQueries = fixture.Freeze<Mock<ITransactionQueries>>();
            transactionQueries
                .Setup(m => m.GetTransactionAsync(transactionId))
                .ReturnsAsync((TransactionDto)null);

            var sut = fixture.Create<TransactionService>();
            var result = await sut.GetTransactionAsync(transactionId);

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task AddingTransaction_AddsTransactionToPosition_WhenWellFormed()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Create<Position>();
            var transaction = fixture
                .Build<TransactionDto>()
                .With(t => t.PositionId, position.Id)
                .Create();

            var positionRepository = fixture.Freeze<Mock<IPositionRepository>>();
            positionRepository
                .Setup(r => r.FindAsync(position.Id))
                .ReturnsAsync(position);
            fixture.Freeze<Mock<IInstrumentPriceService>>();

            var sut = fixture.Create<TransactionService>();

            await sut.AddTransactionAsync(transaction);

            positionRepository.Verify(r => r.Update(It.Is<Position>(p =>
                p.Id == transaction.PositionId &&
                p.Transactions.FirstOrDefault(t =>
                    t.Time == transaction.Time && t.Price == transaction.Price && t.Note == transaction.Note) != null
            )));
        }

        [Fact]
        public async Task AddingTransaction_ThrowsException_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var transaction = fixture.Create<TransactionDto>();

            var positionRepository = fixture.Freeze<Mock<IPositionRepository>>();
            positionRepository
                .Setup(r => r.FindAsync(transaction.PositionId))
                .ReturnsAsync((Position)null);
            positionRepository
                .Setup(r => r.ExistsAsync(transaction.PositionId))
                .ReturnsAsync(false);
            fixture.Freeze<Mock<IInstrumentPriceService>>();

            var sut = fixture.Create<TransactionService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await sut.AddTransactionAsync(transaction));
        }

        [Fact]
        public async Task AddingTransaction_EmitsTransactionAddedToPositionDomainEvent()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Create<Position>();
            var transaction = fixture
                .Build<TransactionDto>()
                .With(t => t.PositionId, position.Id)
                .Create();

            var positionRepository = fixture.Freeze<Mock<IPositionRepository>>();
            positionRepository
                .Setup(r => r.FindAsync(position.Id))
                .ReturnsAsync(position);
            var priceService = fixture.Freeze<Mock<IInstrumentPriceService>>();

            var sut = fixture.Create<TransactionService>();

            await sut.AddTransactionAsync(transaction);

            Assert.Collection(position.DomainEvents, evt =>
            {
                Assert.IsAssignableFrom<TransactionAddedToPositionDomainEvent>(evt);
            });
        }
    }
}