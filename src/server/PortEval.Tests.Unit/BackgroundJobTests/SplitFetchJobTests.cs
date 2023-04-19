using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Core.BackgroundJobs;
using PortEval.Application.Core.Interfaces;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Models.FinancialDataFetcher;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Domain.Models.ValueObjects;
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;
using Range = Moq.Range;

namespace PortEval.Tests.Unit.BackgroundJobTests;

public class SplitFetchJobTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IInstrumentRepository> _instrumentRepository;
    private readonly Mock<IInstrumentSplitRepository> _splitRepository;

    public SplitFetchJobTests()
    {
        _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
        _instrumentRepository = _fixture.CreateDefaultInstrumentRepositoryMock();
        _splitRepository = _fixture.CreateDefaultSplitRepositoryMock();
    }

    [Fact]
    public async Task
        Run_FetchesSplitsFromPriceFetcherBasedOnInstrumentTrackingInfo__WhenNoOtherSplitsAreInTheRepository()
    {
        var instrument = new Instrument(1, _fixture.Create<string>(), _fixture.Create<string>(),
            _fixture.Create<string>(), InstrumentType.Stock, _fixture.Create<string>(), _fixture.Create<string>());
        instrument.SetTrackingFrom(DateTime.Parse("2020-01-01"), DateTime.Parse("2021-01-01"));
        var instruments = new[]
        {
            instrument
        };

        var splitData = new InstrumentSplitData
        {
            Numerator = 5,
            Denominator = 1,
            Time = DateTime.Parse("2022-01-01")
        };

        _splitRepository
            .Setup(r => r.ListInstrumentSplitsAsync(instrument.Id))
            .ReturnsAsync(Enumerable.Empty<InstrumentSplit>());
        var priceFetcher = CreatePriceFetcherMockReturningSplitData(_fixture, new[] { splitData });
        _instrumentRepository
            .Setup(r => r.ListAllAsync())
            .ReturnsAsync(instruments);

        var sut = _fixture.Create<SplitFetchJob>();

        await sut.RunAsync();

        priceFetcher.Verify(f => f.GetInstrumentSplitsAsync(
            instrument.Symbol,
            instrument.TrackingInfo.TrackedSince,
            It.IsInRange(DateTime.UtcNow.AddHours(-1), DateTime.UtcNow.AddHours(1), Range.Inclusive)
        ));
    }

    [Fact]
    public async Task Run_FetchesSplitsFromPriceFetcherBasedOnLastSplitTime__WhenAnotherSplitExistsInRepository()
    {
        var instrument = new Instrument(1, _fixture.Create<string>(), _fixture.Create<string>(),
            _fixture.Create<string>(), InstrumentType.Stock, _fixture.Create<string>(), _fixture.Create<string>());
        instrument.SetTrackingFrom(DateTime.Parse("2020-01-01"), DateTime.Parse("2021-01-01"));
        var instruments = new[]
        {
            instrument
        };

        var existingSplit =
            new InstrumentSplit(1, instrument.Id, DateTime.Parse("2022-06-12"), new SplitRatio(3, 1));

        var splitData = new InstrumentSplitData
        {
            Numerator = 5,
            Denominator = 1,
            Time = DateTime.Parse("2022-01-01")
        };

        _splitRepository
            .Setup(r => r.ListInstrumentSplitsAsync(instrument.Id))
            .ReturnsAsync(new[] { existingSplit });
        var priceFetcher = CreatePriceFetcherMockReturningSplitData(_fixture, new[] { splitData });
        _instrumentRepository
            .Setup(r => r.ListAllAsync())
            .ReturnsAsync(instruments);

        var sut = _fixture.Create<SplitFetchJob>();

        await sut.RunAsync();

        priceFetcher.Verify(f => f.GetInstrumentSplitsAsync(
            instrument.Symbol,
            existingSplit.Time,
            It.IsInRange(DateTime.UtcNow.AddHours(-1), DateTime.UtcNow.AddHours(1), Range.Inclusive)
        ));
    }

    [Fact]
    public async Task Run_ImportsRetrievedSplits()
    {
        var instrument = new Instrument(1, _fixture.Create<string>(), _fixture.Create<string>(),
            _fixture.Create<string>(), InstrumentType.Stock, _fixture.Create<string>(), _fixture.Create<string>());
        instrument.SetTrackingFrom(DateTime.Parse("2020-01-01"), DateTime.Parse("2021-01-01"));
        var instruments = new[]
        {
            instrument
        };

        var splitData = new InstrumentSplitData
        {
            Numerator = 5,
            Denominator = 1,
            Time = DateTime.Parse("2022-01-01")
        };

        _splitRepository
            .Setup(r => r.ListInstrumentSplitsAsync(instrument.Id))
            .ReturnsAsync(Enumerable.Empty<InstrumentSplit>());
        CreatePriceFetcherMockReturningSplitData(_fixture, new[] { splitData });
        _instrumentRepository
            .Setup(r => r.ListAllAsync())
            .ReturnsAsync(instruments);

        var sut = _fixture.Create<SplitFetchJob>();

        await sut.RunAsync();

        _splitRepository.Verify(r => r.Add(It.Is<InstrumentSplit>(s =>
            s.Time == splitData.Time &&
            s.SplitRatio.Numerator == splitData.Numerator &&
            s.SplitRatio.Denominator == splitData.Denominator &&
            s.InstrumentId == instrument.Id &&
            s.ProcessingStatus == InstrumentSplitProcessingStatus.NotProcessed
        )));
    }

    private Mock<IFinancialDataFetcher> CreatePriceFetcherMockReturningSplitData(IFixture fixture,
        IEnumerable<InstrumentSplitData> splitData)
    {
        var mock = fixture.Freeze<Mock<IFinancialDataFetcher>>();
        mock
            .Setup(m => m.GetInstrumentSplitsAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(splitData);

        return mock;
    }
}