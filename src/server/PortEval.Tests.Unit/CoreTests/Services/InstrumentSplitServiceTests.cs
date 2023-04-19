using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Core;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Core.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Domain.Models.ValueObjects;
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.Services;

public class InstrumentSplitServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IInstrumentQueries> _instrumentQueries;
    private readonly Mock<IInstrumentRepository> _instrumentRepository;
    private readonly Mock<IInstrumentSplitRepository> _splitRepository;

    public InstrumentSplitServiceTests()
    {
        _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
        _splitRepository = _fixture.CreateDefaultSplitRepositoryMock();
        _instrumentRepository = _fixture.CreateDefaultInstrumentRepositoryMock();
        _instrumentQueries = _fixture.CreateDefaultInstrumentQueriesMock();
    }

    [Fact]
    public async Task GetInstrumentSplitsAsync_ReturnsAllInstrumentSplits_WhenInstrumentExists()
    {
        var instrumentId = _fixture.Create<int>();
        var splits = _fixture.CreateMany<InstrumentSplitDto>();

        _instrumentQueries
            .Setup(m => m.GetInstrumentSplitsAsync(instrumentId))
            .ReturnsAsync(splits);

        var sut = _fixture.Create<InstrumentSplitService>();
        var result = await sut.GetInstrumentSplitsAsync(instrumentId);

        Assert.Equal(OperationStatus.Ok, result.Status);
        Assert.Equal(splits, result.Response);
    }

    [Fact]
    public async Task GetInstrumentSplitsAsync_ReturnsNotFound_WhenInstrumentDoesNotExist()
    {
        _instrumentQueries
            .Setup(m => m.GetInstrumentAsync(It.IsAny<int>()))
            .ReturnsAsync((InstrumentDto)null);
        _instrumentRepository
            .Setup(m => m.ExistsAsync(It.IsAny<int>()))
            .ReturnsAsync(false);

        var sut = _fixture.Create<InstrumentSplitService>();
        var result = await sut.GetInstrumentSplitsAsync(_fixture.Create<int>());

        Assert.Equal(OperationStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task GetInstrumentSplitAsync_ReturnsSplit_WhenInstrumentAndSplitExist()
    {
        var instrumentId = _fixture.Create<int>();
        var splitId = _fixture.Create<int>();

        var split = _fixture.Create<InstrumentSplitDto>();

        _instrumentQueries
            .Setup(m => m.GetInstrumentSplitAsync(instrumentId, splitId))
            .ReturnsAsync(split);

        var sut = _fixture.Create<InstrumentSplitService>();
        var result = await sut.GetInstrumentSplitAsync(instrumentId, splitId);

        Assert.Equal(OperationStatus.Ok, result.Status);
        Assert.Equal(split, result.Response);
    }

    [Fact]
    public async Task GetInstrumentSplitAsync_ReturnsNotFound_WhenInstrumentDoesNotExist()
    {
        _instrumentQueries
            .Setup(m => m.GetInstrumentAsync(It.IsAny<int>()))
            .ReturnsAsync((InstrumentDto)null);

        var sut = _fixture.Create<InstrumentSplitService>();
        var result = await sut.GetInstrumentSplitAsync(_fixture.Create<int>(), _fixture.Create<int>());

        Assert.Equal(OperationStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task GetInstrumentSplitAsync_ReturnsNotFound_WhenSplitDoesNotExist()
    {
        var instrumentId = _fixture.Create<int>();
        var splitId = _fixture.Create<int>();

        _instrumentQueries
            .Setup(m => m.GetInstrumentSplitAsync(instrumentId, splitId))
            .ReturnsAsync((InstrumentSplitDto)null);

        var sut = _fixture.Create<InstrumentSplitService>();
        var result = await sut.GetInstrumentSplitAsync(instrumentId, splitId);

        Assert.Equal(OperationStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task CreateSplitAsync_SavesSplitToRepository_WhenWellFormed()
    {
        var split = _fixture.Create<InstrumentSplitDto>();

        var sut = _fixture.Create<InstrumentSplitService>();

        await sut.CreateSplitAsync(split);

        _splitRepository.Verify(r => r.Add(It.Is<InstrumentSplit>(s =>
            split.InstrumentId == s.InstrumentId &&
            split.SplitRatioDenominator == s.SplitRatio.Denominator &&
            split.SplitRatioNumerator == s.SplitRatio.Numerator &&
            split.Time == s.Time
        )));
    }

    [Fact]
    public async Task CreateSplitAsync_ReturnsNotFound_WhenInstrumentDoesNotExist()
    {
        var split = _fixture.Create<InstrumentSplitDto>();

        _instrumentRepository
            .Setup(r => r.ExistsAsync(split.InstrumentId))
            .ReturnsAsync(false);
        _instrumentRepository
            .Setup(r => r.FindAsync(split.InstrumentId))
            .ReturnsAsync((Instrument)null);

        var sut = _fixture.Create<InstrumentSplitService>();
        var result = await sut.CreateSplitAsync(split);

        Assert.Equal(OperationStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task UpdateSplitAsync_SavesSplitToRepository_WhenRollbackRequestedStatusIsSet()
    {
        var updatedSplit = _fixture.Build<InstrumentSplitDto>()
            .With(s => s.Status, InstrumentSplitProcessingStatus.RollbackRequested)
            .Create();
        var existingSplit = new InstrumentSplit(updatedSplit.Id, updatedSplit.InstrumentId, updatedSplit.Time,
            new SplitRatio(updatedSplit.SplitRatioDenominator, updatedSplit.SplitRatioNumerator));
        existingSplit.MarkAsProcessed();

        _splitRepository
            .Setup(r => r.FindAsync(updatedSplit.Id))
            .ReturnsAsync(existingSplit);

        var sut = _fixture.Create<InstrumentSplitService>();

        await sut.UpdateSplitAsync(updatedSplit.InstrumentId, updatedSplit);

        _splitRepository.Verify(r => r.Update(It.Is<InstrumentSplit>(s =>
            updatedSplit.InstrumentId == s.InstrumentId &&
            updatedSplit.SplitRatioDenominator == s.SplitRatio.Denominator &&
            updatedSplit.SplitRatioNumerator == s.SplitRatio.Numerator &&
            updatedSplit.Time == s.Time &&
            updatedSplit.Status == s.ProcessingStatus
        )));
    }

    [Fact]
    public async Task UpdateSplitAsync_ReturnsNotFound_WhenSplitDoesNotExist()
    {
        var split = _fixture.Create<InstrumentSplitDto>();

        _splitRepository
            .Setup(m => m.ExistsAsync(split.Id))
            .ReturnsAsync(false);
        _splitRepository
            .Setup(m => m.FindAsync(split.Id))
            .ReturnsAsync((InstrumentSplit)null);

        var sut = _fixture.Create<InstrumentSplitService>();
        var response = await sut.UpdateSplitAsync(split.InstrumentId, split);

        Assert.Equal(OperationStatus.NotFound, response.Status);
    }
}