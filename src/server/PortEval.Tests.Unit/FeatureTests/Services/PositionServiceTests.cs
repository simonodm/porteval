using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Features.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Domain;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Tests.Unit.Helpers.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.FeatureTests.Services
{
    public class PositionServiceTests
    {
        [Fact]
        public async Task OpeningPosition_AddsMatchingPositionToRepository_WhenWellFormed()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Create<PositionDto>();

            fixture.CreateDefaultPortfolioRepositoryMock();
            var positionRepository = fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultInstrumentPriceServiceMock();

            var sut = fixture.Create<PositionService>();

            await sut.OpenPositionAsync(position);

            positionRepository.Verify(r => r.Add(It.Is<Position>(p =>
                p.InstrumentId == position.InstrumentId &&
                p.PortfolioId == position.PortfolioId &&
                p.Note == position.Note &&
                p.Transactions.Count == 1 &&
                p.Transactions.First().Amount == position.Amount &&
                p.Transactions.First().Price == position.Price &&
                p.Transactions.First().Time == position.Time
            )), Times.Once());
        }

        [Fact]
        public async Task OpeningPosition_ThrowsException_WhenParentPortfolioDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Create<PositionDto>();

            var portfolioRepository = fixture.CreateDefaultPortfolioRepositoryMock();
            portfolioRepository
                .Setup(r => r.ExistsAsync(position.PortfolioId))
                .Returns(Task.FromResult(false));
            portfolioRepository
                .Setup(r => r.FindAsync(position.PortfolioId))
                .Returns(Task.FromResult<Portfolio>(null));
            fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultInstrumentPriceServiceMock();

            var sut = fixture.Create<PositionService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await sut.OpenPositionAsync(position));
        }

        [Fact]
        public async Task OpeningPosition_ThrowsException_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Create<PositionDto>();

            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(r => r.ExistsAsync(position.InstrumentId))
                .Returns(Task.FromResult(false));
            instrumentRepository
                .Setup(r => r.FindAsync(position.InstrumentId))
                .Returns(Task.FromResult<Instrument>(null));
            fixture.CreateDefaultInstrumentPriceServiceMock();

            var sut = fixture.Create<PositionService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await sut.OpenPositionAsync(position));
        }

        [Fact]
        public async Task OpeningPosition_ThrowsException_WhenInstrumentTypeIsIndex()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Create<PositionDto>();

            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(r => r.FindAsync(position.InstrumentId))
                .Returns(Task.FromResult(
                    new Instrument(
                        fixture.Create<string>(),
                        fixture.Create<string>(),
                        fixture.Create<string>(),
                        InstrumentType.Index,
                        fixture.Create<string>(),
                        fixture.Create<string>()
                    )
                ));
            fixture.CreateDefaultInstrumentPriceServiceMock();

            var sut = fixture.Create<PositionService>();

            await Assert.ThrowsAsync<OperationNotAllowedException>(async () => await sut.OpenPositionAsync(position));
        }

        [Fact]
        public async Task OpeningPosition_ThrowsException_WhenAmountIsNull()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Build<PositionDto>().With(p => p.Amount, (decimal?)null).Create();

            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultInstrumentPriceServiceMock();

            var sut = fixture.Create<PositionService>();

            await Assert.ThrowsAsync<OperationNotAllowedException>(async () => await sut.OpenPositionAsync(position));
        }

        [Fact]
        public async Task OpeningPosition_ThrowsException_WhenAmountIsZero()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Build<PositionDto>().With(p => p.Amount, 0).Create();

            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultInstrumentPriceServiceMock();

            var sut = fixture.Create<PositionService>();

            await Assert.ThrowsAsync<OperationNotAllowedException>(async () => await sut.OpenPositionAsync(position));
        }

        [Fact]
        public async Task OpeningPosition_ThrowsException_WhenPriceIsNull()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Build<PositionDto>().With(p => p.Price, (decimal?)null).Create();

            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultInstrumentPriceServiceMock();

            var sut = fixture.Create<PositionService>();

            await Assert.ThrowsAsync<OperationNotAllowedException>(async () => await sut.OpenPositionAsync(position));
        }

        [Fact]
        public async Task OpeningPosition_ThrowsException_WhenPriceIsZero()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Build<PositionDto>().With(p => p.Price, 0).Create();

            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultInstrumentPriceServiceMock();

            var sut = fixture.Create<PositionService>();

            await Assert.ThrowsAsync<OperationNotAllowedException>(async () => await sut.OpenPositionAsync(position));
        }

        [Fact]
        public async Task OpeningPosition_ThrowsException_WhenTimeIsNull()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Build<PositionDto>().With(p => p.Time, (DateTime?)null).Create();

            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultInstrumentPriceServiceMock();

            var sut = fixture.Create<PositionService>();

            await Assert.ThrowsAsync<OperationNotAllowedException>(async () => await sut.OpenPositionAsync(position));
        }

        [Fact]
        public async Task OpeningPosition_ThrowsException_WhenTimeIsBeforeMinimumStartTime()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Build<PositionDto>()
                .With(p => p.Time, PortEvalConstants.FinancialDataStartTime.AddDays(-1)).Create();

            fixture.CreateDefaultPortfolioRepositoryMock();
            fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultInstrumentPriceServiceMock();

            var sut = fixture.Create<PositionService>();

            await Assert.ThrowsAsync<OperationNotAllowedException>(async () => await sut.OpenPositionAsync(position));
        }

        [Fact]
        public async Task UpdatingPosition_UpdatesNote_WhenWellFormed()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Create<PositionDto>();

            fixture.CreateDefaultPortfolioRepositoryMock();
            var positionRepository = fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultInstrumentPriceServiceMock();

            var sut = fixture.Create<PositionService>();

            await sut.UpdatePositionAsync(position);

            positionRepository.Verify(r => r.Update(It.Is<Position>(p => p.Note == position.Note)));
        }

        [Fact]
        public async Task UpdatingPosition_ThrowsException_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var position = fixture.Create<PositionDto>();

            fixture.CreateDefaultPortfolioRepositoryMock();
            var positionRepository = fixture.CreateDefaultPositionRepositoryMock();
            positionRepository
                .Setup(r => r.FindAsync(position.Id))
                .Returns(Task.FromResult<Position>(null));
            positionRepository
                .Setup(r => r.ExistsAsync(position.Id))
                .Returns(Task.FromResult(false));
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultInstrumentPriceServiceMock();

            var sut = fixture.Create<PositionService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await sut.UpdatePositionAsync(position));
        }

        [Fact]
        public async Task DeletingPosition_DeletesPosition_WhenPositionExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionId = fixture.Create<int>();

            fixture.CreateDefaultPortfolioRepositoryMock();
            var positionRepository = fixture.CreateDefaultPositionRepositoryMock();
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultInstrumentPriceServiceMock();

            var sut = fixture.Create<PositionService>();

            await sut.RemovePositionAsync(positionId);

            positionRepository.Verify(r => r.DeleteAsync(positionId), Times.Once());
        }

        [Fact]
        public async Task DeletingPosition_ThrowsException_WhenPositionDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positionId = fixture.Create<int>();

            fixture.CreateDefaultPortfolioRepositoryMock();
            var positionRepository = fixture.CreateDefaultPositionRepositoryMock();
            positionRepository
                .Setup(r => r.ExistsAsync(positionId))
                .Returns(Task.FromResult(false));
            positionRepository
                .Setup(r => r.FindAsync(positionId))
                .Returns(Task.FromResult<Position>(null));
            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultInstrumentPriceServiceMock();

            var sut = fixture.Create<PositionService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(async () => await sut.RemovePositionAsync(positionId));
        }
    }
}