using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Core.BackgroundJobs;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Models.FinancialDataFetcher;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;

namespace PortEval.Tests.Unit.BackgroundJobTests;

public class InitialPriceFetchJobTests
{
    private readonly TimeSpan _expectedPriceTimePrecision;
    private readonly IFixture _fixture;
    private readonly Mock<IInstrumentPriceRepository> _instrumentPriceRepository;
    private readonly Mock<IInstrumentRepository> _instrumentRepository;
    private Mock<ICurrencyExchangeRateRepository> _exchangeRateRepository;

    public InitialPriceFetchJobTests()
    {
        _expectedPriceTimePrecision = TimeSpan.FromMinutes(5);
        _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
        _exchangeRateRepository = _fixture.CreateDefaultCurrencyExchangeRateRepositoryMock();
        _instrumentRepository = _fixture.CreateDefaultInstrumentRepositoryMock();
        _instrumentPriceRepository = _fixture.CreateDefaultInstrumentPriceRepositoryMock();
    }

    [Fact]
    public async Task Run_ImportsDailyPricesFromFetcherResult()
    {
        var now = DateTime.UtcNow;

        var instrument = new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");

        var dailyPrices = new List<PricePoint>
        {
            _fixture.Build<PricePoint>().With(p => p.Time, now.AddDays(-7)).With(p => p.Price, 100).Create(),
            _fixture.Build<PricePoint>().With(p => p.Time, now.AddDays(-6)).With(p => p.Price, 101).Create(),
            _fixture.Build<PricePoint>().With(p => p.Time, now.AddDays(-5)).With(p => p.Price, 102).Create()
        };
        _fixture.CreatePriceFetcherMockReturningHistoricalPrices(instrument, dailyPrices, null, null);
        _instrumentRepository
            .Setup(m => m.FindAsync(instrument.Id))
            .ReturnsAsync(instrument);

        var sut = _fixture.Create<InitialPriceFetchJob>();

        await sut.RunAsync(instrument.Id);

        _instrumentPriceRepository.Verify(m => m.BulkUpsertAsync(It.Is<IList<InstrumentPrice>>(prices =>
            prices.Any(p =>
                p.InstrumentId == instrument.Id && p.Time.InRange(now.AddDays(-7), _expectedPriceTimePrecision) &&
                p.Price == 100m) &&
            prices.Any(p =>
                p.InstrumentId == instrument.Id && p.Time.InRange(now.AddDays(-6), _expectedPriceTimePrecision) &&
                p.Price == 101m) &&
            prices.Any(p =>
                p.InstrumentId == instrument.Id && p.Time.InRange(now.AddDays(-5), _expectedPriceTimePrecision) &&
                p.Price == 102m)
        )));
    }

    [Fact]
    public async Task Run_ImportsHourlyPricesFromFetcherResult()
    {
        var now = DateTime.UtcNow;

        var instrument = new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");

        var hourlyPrices = new List<PricePoint>
        {
            _fixture.Build<PricePoint>().With(p => p.Time, now.AddHours(-48)).With(p => p.Price, 100).Create(),
            _fixture.Build<PricePoint>().With(p => p.Time, now.AddHours(-47)).With(p => p.Price, 101).Create(),
            _fixture.Build<PricePoint>().With(p => p.Time, now.AddHours(-46)).With(p => p.Price, 102).Create()
        };

        _fixture.CreatePriceFetcherMockReturningHistoricalPrices(instrument, null, hourlyPrices, null);
        _instrumentRepository
            .Setup(m => m.FindAsync(instrument.Id))
            .ReturnsAsync(instrument);

        var sut = _fixture.Create<InitialPriceFetchJob>();

        await sut.RunAsync(instrument.Id);

        _instrumentPriceRepository.Verify(m => m.BulkUpsertAsync(It.Is<IList<InstrumentPrice>>(prices =>
            prices.Any(p =>
                p.InstrumentId == instrument.Id && p.Time.InRange(now.AddHours(-48), _expectedPriceTimePrecision) &&
                p.Price == 100m) &&
            prices.Any(p =>
                p.InstrumentId == instrument.Id && p.Time.InRange(now.AddHours(-47), _expectedPriceTimePrecision) &&
                p.Price == 101m) &&
            prices.Any(p =>
                p.InstrumentId == instrument.Id && p.Time.InRange(now.AddHours(-46), _expectedPriceTimePrecision) &&
                p.Price == 102m)
        )));
    }

    [Fact]
    public async Task Run_ImportsFiveMinutePricesFromFetcherResult()
    {
        var now = DateTime.UtcNow;

        var instrument = new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");

        var latestPrices = new List<PricePoint>
        {
            _fixture.Build<PricePoint>().With(p => p.Time, now.AddMinutes(-15)).With(p => p.Price, 100).Create(),
            _fixture.Build<PricePoint>().With(p => p.Time, now.AddMinutes(-10)).With(p => p.Price, 101).Create(),
            _fixture.Build<PricePoint>().With(p => p.Time, now.AddMinutes(-5)).With(p => p.Price, 102).Create()
        };

        _fixture.CreatePriceFetcherMockReturningHistoricalPrices(instrument, null, null, latestPrices);
        _instrumentRepository
            .Setup(m => m.FindAsync(instrument.Id))
            .ReturnsAsync(instrument);

        var sut = _fixture.Create<InitialPriceFetchJob>();

        await sut.RunAsync(instrument.Id);

        _instrumentPriceRepository.Verify(m => m.BulkUpsertAsync(It.Is<IList<InstrumentPrice>>(prices =>
            prices.Any(p =>
                p.InstrumentId == instrument.Id && p.Time.InRange(now.AddMinutes(-15), _expectedPriceTimePrecision) &&
                p.Price == 100m) &&
            prices.Any(p =>
                p.InstrumentId == instrument.Id && p.Time.InRange(now.AddMinutes(-10), _expectedPriceTimePrecision) &&
                p.Price == 101m) &&
            prices.Any(p =>
                p.InstrumentId == instrument.Id && p.Time.InRange(now.AddMinutes(-5), _expectedPriceTimePrecision) &&
                p.Price == 102m)
        )));
    }

    [Fact]
    public async Task Run_FillsMissingDailyPrices()
    {
        var now = DateTime.UtcNow;

        var instrument = new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");

        var dailyPrices = new List<PricePoint>
        {
            _fixture.Build<PricePoint>().With(p => p.Time, now.AddDays(-8)).With(p => p.Price, 100).Create(),
            _fixture.Build<PricePoint>().With(p => p.Time, now.AddDays(-6)).With(p => p.Price, 102).Create()
        };

        _fixture.CreatePriceFetcherMockReturningHistoricalPrices(instrument, dailyPrices, null, null);
        _instrumentRepository
            .Setup(m => m.FindAsync(instrument.Id))
            .ReturnsAsync(instrument);

        var sut = _fixture.Create<InitialPriceFetchJob>();

        await sut.RunAsync(instrument.Id);

        _instrumentPriceRepository.Verify(m => m.BulkUpsertAsync(It.Is<IList<InstrumentPrice>>(prices =>
            prices.Any(p =>
                p.InstrumentId == instrument.Id && p.Time.InRange(now.AddDays(-7), TimeSpan.FromDays(1)) &&
                p.Price == 100m) &&
            prices.Any(p =>
                p.InstrumentId == instrument.Id && p.Time.InRange(now.AddDays(-5), TimeSpan.FromDays(1)) &&
                p.Price == 102m)
        )));
    }

    [Fact]
    public async Task Run_FillsMissingHourlyPrices()
    {
        var now = DateTime.UtcNow;

        var instrument = new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");

        var hourlyPrices = new List<PricePoint>
        {
            _fixture.Build<PricePoint>().With(p => p.Time, now.AddHours(-48)).With(p => p.Price, 100).Create(),
            _fixture.Build<PricePoint>().With(p => p.Time, now.AddHours(-46)).With(p => p.Price, 102).Create()
        };

        _fixture.CreatePriceFetcherMockReturningHistoricalPrices(instrument, null, hourlyPrices, null);
        _instrumentRepository
            .Setup(m => m.FindAsync(instrument.Id))
            .ReturnsAsync(instrument);

        var sut = _fixture.Create<InitialPriceFetchJob>();

        await sut.RunAsync(instrument.Id);

        _instrumentPriceRepository.Verify(m => m.BulkUpsertAsync(It.Is<IList<InstrumentPrice>>(prices =>
            prices.Any(p =>
                p.InstrumentId == instrument.Id && p.Time.InRange(now.AddHours(-47), TimeSpan.FromHours(1)) &&
                p.Price == 100m) &&
            prices.Any(p =>
                p.InstrumentId == instrument.Id && p.Time.InRange(now.AddHours(-1), TimeSpan.FromHours(1)) &&
                p.Price == 102m)
        )));
    }

    [Fact]
    public async Task Run_FillsMissingFiveMinutePrices()
    {
        var now = DateTime.UtcNow;

        var instrument = new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");

        var latestPrices = new List<PricePoint>
        {
            _fixture.Build<PricePoint>().With(p => p.Time, now.AddMinutes(-30)).With(p => p.Price, 100).Create()
        };

        _fixture.CreatePriceFetcherMockReturningHistoricalPrices(instrument, null, null, latestPrices);
        _instrumentRepository
            .Setup(m => m.FindAsync(instrument.Id))
            .ReturnsAsync(instrument);

        var sut = _fixture.Create<InitialPriceFetchJob>();

        await sut.RunAsync(instrument.Id);

        _instrumentPriceRepository.Verify(m => m.BulkUpsertAsync(It.Is<IList<InstrumentPrice>>(prices =>
            prices.Any(p =>
                p.InstrumentId == instrument.Id && p.Time.InRange(now.AddMinutes(-25), _expectedPriceTimePrecision) &&
                p.Price == 100m) &&
            prices.Any(p =>
                p.InstrumentId == instrument.Id && p.Time.InRange(now.AddMinutes(-15), _expectedPriceTimePrecision) &&
                p.Price == 100m) &&
            prices.Any(p =>
                p.InstrumentId == instrument.Id && p.Time.InRange(now.AddMinutes(-10), _expectedPriceTimePrecision) &&
                p.Price == 100m)
        )));
    }

    [Fact]
    public async Task Run_SetsInstrumentTrackingInformationToFirstPriceTime_WhenAnyPricesAreDownloaded()
    {
        var now = DateTime.UtcNow;

        var instrument = new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");

        var latestPrices = new List<PricePoint>
        {
            _fixture.Build<PricePoint>().With(p => p.Time, now.AddMinutes(-30)).With(p => p.Price, 100).Create()
        };

        _fixture.CreatePriceFetcherMockReturningHistoricalPrices(instrument, null, null, latestPrices);
        _instrumentRepository
            .Setup(m => m.FindAsync(instrument.Id))
            .ReturnsAsync(instrument);

        var sut = _fixture.Create<InitialPriceFetchJob>();

        await sut.RunAsync(instrument.Id);

        _instrumentRepository.Verify(m => m.Update(It.Is<Instrument>(i =>
            i.Id == instrument.Id &&
            i.TrackingInfo.StartTime == now.AddMinutes(-30) &&
            i.TrackingInfo.LastUpdate >= now &&
            i.TrackingStatus == InstrumentTrackingStatus.Tracked
        )));
    }

    [Fact]
    public async Task Run_SetsInstrumentTrackingStatusToUntracked_WhenNoPricesAreDownloaded()
    {
        var instrument = new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");

        _fixture.CreatePriceFetcherMockReturningHistoricalPrices(instrument, null, null, null);
        _instrumentRepository
            .Setup(m => m.FindAsync(instrument.Id))
            .ReturnsAsync(instrument);

        var sut = _fixture.Create<InitialPriceFetchJob>();

        await sut.RunAsync(instrument.Id);

        _instrumentRepository.Verify(m => m.Update(It.Is<Instrument>(i =>
            i.Id == instrument.Id &&
            i.TrackingStatus == InstrumentTrackingStatus.Untracked
        )));
    }
}