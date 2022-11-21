using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Services;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;

namespace PortEval.Tests.Unit.ServiceTests
{
    public class TransactionServiceTests
    {
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

            var positionRepository = fixture.CreateDefaultPositionRepositoryMock();
            positionRepository
                .Setup(r => r.FindAsync(position.Id))
                .ReturnsAsync(position);
            fixture.CreateDefaultInstrumentPriceServiceMock();

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

            var positionRepository = fixture.CreateDefaultPositionRepositoryMock();
            positionRepository
                .Setup(r => r.FindAsync(transaction.PositionId))
                .ReturnsAsync((Position)null);
            positionRepository
                .Setup(r => r.Exists(transaction.PositionId))
                .ReturnsAsync(false);
            fixture.CreateDefaultInstrumentPriceServiceMock();

            var sut = fixture.Create<TransactionService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await sut.AddTransactionAsync(transaction));
        }

        [Fact]
        public async Task AddingTransaction_AddsInstrumentPrice_WhenPriceDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Create<Position>();
            var transaction = fixture
                .Build<TransactionDto>()
                .With(t => t.PositionId, position.Id)
                .Create();

            var positionRepository = fixture.CreateDefaultPositionRepositoryMock();
            positionRepository
                .Setup(r => r.FindAsync(position.Id))
                .ReturnsAsync(position);
            var priceService = fixture.CreateDefaultInstrumentPriceServiceMock();

            var sut = fixture.Create<TransactionService>();

            await sut.AddTransactionAsync(transaction);

            priceService.Verify(s =>
                s.AddPriceIfNotExistsAsync(position.InstrumentId, transaction.Time, transaction.Price));
        }
    }
}