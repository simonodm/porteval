using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Features.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Domain.Models.ValueObjects;
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;

namespace PortEval.Tests.Unit.FeatureTests.Services
{
    public class InstrumentSplitServiceTests
    {
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

            Assert.Equal(split.InstrumentId, result.InstrumentId);
            Assert.Equal(split.Time, result.Time);
            Assert.Equal(split.SplitRatioDenominator, result.SplitRatio.Denominator);
            Assert.Equal(split.SplitRatioNumerator, result.SplitRatio.Numerator);
        }

        [Fact]
        public async Task CreateSplitAsync_ThrowsNotFoundException_WhenInstrumentDoesNotExist()
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

            await Assert.ThrowsAsync<ItemNotFoundException>(() => sut.CreateSplitAsync(split));
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

            Assert.Equal(updatedSplit.InstrumentId, result.InstrumentId);
            Assert.Equal(updatedSplit.Time, result.Time);
            Assert.Equal(updatedSplit.SplitRatioDenominator, result.SplitRatio.Denominator);
            Assert.Equal(updatedSplit.SplitRatioNumerator, result.SplitRatio.Numerator);
            Assert.Equal(updatedSplit.Status, result.ProcessingStatus);
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
