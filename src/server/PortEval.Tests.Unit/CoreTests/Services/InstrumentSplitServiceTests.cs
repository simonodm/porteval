using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Core;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Core.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Domain.Models.ValueObjects;
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.Services
{
    public class InstrumentSplitServiceTests
    {
        [Fact]
        public async Task GetInstrumentSplitsAsync_ReturnsAllInstrumentSplits_WhenInstrumentExists()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();
            var splits = fixture.CreateMany<InstrumentSplitDto>();

            var instrumentQueries = fixture.CreateDefaultInstrumentQueriesMock();
            instrumentQueries
                .Setup(m => m.GetInstrumentSplitsAsync(instrumentId))
                .ReturnsAsync(splits);

            var sut = fixture.Create<InstrumentSplitService>();
            var result = await sut.GetInstrumentSplitsAsync(instrumentId);

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(splits, result.Response);
        }

        [Fact]
        public async Task GetInstrumentSplitsAsync_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            
            var instrumentQueries = fixture.CreateDefaultInstrumentQueriesMock();

            instrumentQueries
                .Setup(m => m.GetInstrumentAsync(It.IsAny<int>()))
                .ReturnsAsync((InstrumentDto)null);

            var sut = fixture.Create<InstrumentSplitService>();
            var result = await sut.GetInstrumentSplitsAsync(fixture.Create<int>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetInstrumentSplitAsync_ReturnsSplit_WhenInstrumentAndSplitExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();
            var splitId = fixture.Create<int>();

            var split = fixture.Create<InstrumentSplitDto>();

            var instrumentQueries = fixture.CreateDefaultInstrumentQueriesMock();
            instrumentQueries
                .Setup(m => m.GetInstrumentSplitAsync(instrumentId, splitId))
                .ReturnsAsync(split);

            var sut = fixture.Create<InstrumentSplitService>();
            var result = await sut.GetInstrumentSplitAsync(instrumentId, splitId);

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(split, result.Response);
        }

        [Fact]
        public async Task GetInstrumentSplitAsync_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentQueries = fixture.CreateDefaultInstrumentQueriesMock();
            instrumentQueries
                .Setup(m => m.GetInstrumentAsync(It.IsAny<int>()))
                .ReturnsAsync((InstrumentDto)null);

            var sut = fixture.Create<InstrumentSplitService>();
            var result = await sut.GetInstrumentSplitAsync(fixture.Create<int>(), fixture.Create<int>());

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetInstrumentSplitAsync_ReturnsNotFound_WhenSplitDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();
            var splitId = fixture.Create<int>();

            var instrumentQueries = fixture.CreateDefaultInstrumentQueriesMock();
            instrumentQueries
                .Setup(m => m.GetInstrumentSplitAsync(instrumentId, splitId))
                .ReturnsAsync((InstrumentSplitDto)null);

            var sut = fixture.Create<InstrumentSplitService>();
            var result = await sut.GetInstrumentSplitAsync(instrumentId, splitId);

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task CreateSplitAsync_SavesSplitToRepository_WhenWellFormed()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var split = fixture.Create<InstrumentSplitDto>();

            fixture.CreateDefaultInstrumentRepositoryMock();
            var splitRepository = fixture.CreateDefaultSplitRepositoryMock();

            var sut = fixture.Create<InstrumentSplitService>();

            await sut.CreateSplitAsync(split);

            splitRepository.Verify(r => r.Add(It.Is<InstrumentSplit>(s =>
                split.InstrumentId == s.InstrumentId &&
                split.SplitRatioDenominator == s.SplitRatio.Denominator &&
                split.SplitRatioNumerator == s.SplitRatio.Numerator &&
                split.Time == s.Time
            )));
        }

        [Fact]
        public async Task CreateSplitAsync_ReturnsCreatedSplit_WhenSuccessfullyCreated()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var split = fixture.Create<InstrumentSplitDto>();

            fixture.CreateDefaultInstrumentRepositoryMock();
            fixture.CreateDefaultSplitRepositoryMock();

            var sut = fixture.Create<InstrumentSplitService>();

            var result = await sut.CreateSplitAsync(split);

            Assert.Equal(OperationStatus.Ok, result.Status);
            Assert.Equal(split.InstrumentId, result.Response.InstrumentId);
            Assert.Equal(split.Time, result.Response.Time);
            Assert.Equal(split.SplitRatioDenominator, result.Response.SplitRatioDenominator);
            Assert.Equal(split.SplitRatioNumerator, result.Response.SplitRatioNumerator);
        }

        [Fact]
        public async Task CreateSplitAsync_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var split = fixture.Create<InstrumentSplitDto>();

            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(r => r.ExistsAsync(split.InstrumentId))
                .ReturnsAsync(false);
            instrumentRepository
                .Setup(r => r.FindAsync(split.InstrumentId))
                .ReturnsAsync((Instrument)null);
            fixture.CreateDefaultSplitRepositoryMock();

            var sut = fixture.Create<InstrumentSplitService>();
            var result = await sut.CreateSplitAsync(split);

            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task UpdateSplitAsync_SavesSplitToRepository_WhenRollbackRequestedStatusIsSet()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var updatedSplit = fixture.Build<InstrumentSplitDto>()
                .With(s => s.Status, InstrumentSplitProcessingStatus.RollbackRequested)
                .Create();
            var existingSplit = new InstrumentSplit(updatedSplit.Id, updatedSplit.InstrumentId, updatedSplit.Time,
                new SplitRatio(updatedSplit.SplitRatioDenominator, updatedSplit.SplitRatioNumerator));
            existingSplit.MarkAsProcessed();

            fixture.CreateDefaultInstrumentRepositoryMock();
            var splitRepository = fixture.CreateDefaultSplitRepositoryMock();
            splitRepository
                .Setup(r => r.FindAsync(updatedSplit.Id))
                .ReturnsAsync(existingSplit);

            var sut = fixture.Create<InstrumentSplitService>();

            await sut.UpdateSplitAsync(updatedSplit.InstrumentId, updatedSplit);

            splitRepository.Verify(r => r.Update(It.Is<InstrumentSplit>(s =>
                updatedSplit.InstrumentId == s.InstrumentId &&
                updatedSplit.SplitRatioDenominator == s.SplitRatio.Denominator &&
                updatedSplit.SplitRatioNumerator == s.SplitRatio.Numerator &&
                updatedSplit.Time == s.Time &&
                updatedSplit.Status == s.ProcessingStatus
            )));
        }

        [Fact]
        public async Task UpdateSplitAsync_ReturnsUpdatedSplit_WhenSuccessfullyCreated()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var updatedSplit = fixture.Build<InstrumentSplitDto>()
                .With(s => s.Status, InstrumentSplitProcessingStatus.RollbackRequested)
                .Create();
            var existingSplit = new InstrumentSplit(updatedSplit.Id, updatedSplit.InstrumentId, updatedSplit.Time,
                new SplitRatio(updatedSplit.SplitRatioDenominator, updatedSplit.SplitRatioNumerator));
            existingSplit.MarkAsProcessed();

            fixture.CreateDefaultInstrumentRepositoryMock();
            var splitRepository = fixture.CreateDefaultSplitRepositoryMock();
            splitRepository
                .Setup(r => r.FindAsync(updatedSplit.Id))
                .ReturnsAsync(existingSplit);

            var sut = fixture.Create<InstrumentSplitService>();

            var result = await sut.UpdateSplitAsync(updatedSplit.InstrumentId, updatedSplit);

            Assert.Equal(updatedSplit.InstrumentId, result.Response.InstrumentId);
            Assert.Equal(updatedSplit.Time, result.Response.Time);
            Assert.Equal(updatedSplit.SplitRatioDenominator, result.Response.SplitRatioDenominator);
            Assert.Equal(updatedSplit.SplitRatioNumerator, result.Response.SplitRatioNumerator);
            Assert.Equal(updatedSplit.Status, result.Response.Status);
        }

        [Fact]
        public async Task UpdateSplitAsync_ThrowsNotFoundException_WhenSplitDoesNotExist()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var split = fixture.Create<InstrumentSplitDto>();

            fixture.CreateDefaultInstrumentRepositoryMock();
            var splitRepository = fixture.CreateDefaultSplitRepositoryMock();
            splitRepository
                .Setup(m => m.ExistsAsync(split.Id))
                .ReturnsAsync(false);
            splitRepository
                .Setup(m => m.FindAsync(split.Id))
                .ReturnsAsync((InstrumentSplit)null);

            var sut = fixture.Create<InstrumentSplitService>();

            await Assert.ThrowsAsync<ItemNotFoundException>(() => sut.UpdateSplitAsync(split.InstrumentId, split));
        }
    }
}
