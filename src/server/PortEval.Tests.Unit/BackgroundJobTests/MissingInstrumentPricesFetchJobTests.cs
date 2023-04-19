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
using PortEval.Domain;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;
using Range = Moq.Range;

namespace PortEval.Tests.Unit.BackgroundJobTests;

public class MissingInstrumentPricesFetchJobTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IInstrumentRepository> _instrumentRepository;
    private readonly Mock<IInstrumentPriceRepository> _priceRepository;

    public MissingInstrumentPricesFetchJobTests()
    {
        _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
        _instrumentRepository = _fixture.CreateDefaultInstrumentRepositoryMock();
        _priceRepository = _fixture.CreateDefaultInstrumentPriceRepositoryMock();
    }

    [Fact]
    public async Task Run_ImportsMissingDailyPricesFromFetcherResult()
    {
        var baseTime = DateTime.UtcNow;
        var instrument = new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");
        instrument.SetTrackingFrom(DateTime.UtcNow.AddMonths(-1));
        var prices = new List<PricePoint>
        {
            _fixture
                .Build<PricePoint>()
                .With(p => p.CurrencyCode, "USD")
                .With(p => p.Time, baseTime.AddDays(-6))
                .With(p => p.Price, 100m)
                .Create()
        };

        _instrumentRepository
            .Setup(m => m.ListAllAsync())
            .ReturnsAsync(new List<Instrument> { instrument });
        var priceFetcher = _fixture.CreatePriceFetcherMockReturningHistoricalPrices(instrument, prices, null, null);

        var sut = _fixture.Create<MissingInstrumentPricesFetchJob>();

        await sut.RunAsync();

        priceFetcher.Verify(m => m.GetHistoricalDailyPricesAsync(
            instrument.Symbol,
            instrument.CurrencyCode,
            PortEvalConstants.FinancialDataStartTime,
            It.IsInRange(baseTime.AddDays(-5).AddMinutes(-5), baseTime.AddDays(-5).AddMinutes(5), Range.Inclusive)));

        _priceRepository.Verify(m => m.BulkUpsertAsync(It.Is<IList<InstrumentPrice>>(list =>
            list.Any(price => price.Time == prices[0].Time && price.Price == prices[0].Price &&
                              price.InstrumentId == instrument.Id)
        )));
    }

    [Fact]
    public async Task Run_ImportsMissingHourlyPricesFromFetcherResult()
    {
        var baseTime = DateTime.UtcNow;
        var instrument = new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");
        instrument.SetTrackingFrom(baseTime.AddMonths(-1));
        var prices = new List<PricePoint>
        {
            _fixture
                .Build<PricePoint>()
                .With(p => p.CurrencyCode, "USD")
                .With(p => p.Time, baseTime.AddHours(-48))
                .With(p => p.Price, 100m)
                .Create()
        };

        _instrumentRepository
            .Setup(m => m.ListAllAsync())
            .ReturnsAsync(new List<Instrument> { instrument });
        var priceFetcher = _fixture.CreatePriceFetcherMockReturningHistoricalPrices(instrument, null, prices, null);


        var sut = _fixture.Create<MissingInstrumentPricesFetchJob>();

        await sut.RunAsync();

        priceFetcher.Verify(m => m.GetIntradayPricesAsync(
            instrument.Symbol,
            instrument.CurrencyCode,
            It.IsInRange(baseTime.AddDays(-5).AddMinutes(-5), baseTime.AddDays(-5).AddMinutes(5), Range.Inclusive),
            It.IsInRange(baseTime.AddDays(-1).AddMinutes(-5), baseTime.AddDays(-1).AddMinutes(5), Range.Inclusive),
            IntradayInterval.OneHour
        ));
        _priceRepository.Verify(m => m.BulkUpsertAsync(It.Is<IList<InstrumentPrice>>(list =>
            list.Any(price => price.Time == prices[0].Time && price.Price == prices[0].Price &&
                              price.InstrumentId == instrument.Id)
        )));
    }

    [Fact]
    public async Task Run_ImportsMissingFiveMinutePricesFromFetcherResult()
    {
        var baseTime = DateTime.UtcNow;
        var instrument = new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");
        instrument.SetTrackingFrom(baseTime.AddMonths(-1));
        var prices = new List<PricePoint>
        {
            _fixture
                .Build<PricePoint>()
                .With(p => p.CurrencyCode, "USD")
                .With(p => p.Time, baseTime.AddMinutes(-10))
                .With(p => p.Price, 100m)
                .Create()
        };

        _instrumentRepository
            .Setup(m => m.ListAllAsync())
            .ReturnsAsync(new List<Instrument> { instrument });
        var priceFetcher = _fixture.CreatePriceFetcherMockReturningHistoricalPrices(instrument, null, null, prices);


        var sut = _fixture.Create<MissingInstrumentPricesFetchJob>();

        await sut.RunAsync();

        priceFetcher.Verify(m => m.GetIntradayPricesAsync(
            instrument.Symbol,
            instrument.CurrencyCode,
            It.IsInRange(baseTime.AddDays(-1).AddMinutes(-5), baseTime.AddDays(-1).AddMinutes(5), Range.Inclusive),
            It.IsInRange(baseTime.AddMinutes(-5), baseTime.AddMinutes(5), Range.Inclusive),
            IntradayInterval.FiveMinutes
        ));
        _priceRepository.Verify(m => m.BulkUpsertAsync(It.Is<IList<InstrumentPrice>>(list =>
            list.Any(price => price.Time == prices[0].Time && price.Price == prices[0].Price &&
                              price.InstrumentId == instrument.Id)
        )));
    }

    [Fact]
    public async Task Run_SplitsMissingRangesCorrectly()
    {
        var baseTime = DateTime.UtcNow;
        var instrument = new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");
        instrument.SetTrackingFrom(baseTime.AddMonths(-1));
        var dailyPrices = new List<PricePoint>
        {
            _fixture
                .Build<PricePoint>()
                .With(p => p.CurrencyCode, "USD")
                .With(p => p.Time, baseTime.AddMonths(-1).AddDays(1))
                .With(p => p.Price, 100m)
                .Create()
        };
        var hourlyPrices = new List<PricePoint>
        {
            _fixture
                .Build<PricePoint>()
                .With(p => p.CurrencyCode, "USD")
                .With(p => p.Time, baseTime.AddDays(-4))
                .With(p => p.Price, 110m)
                .Create()
        };
        var fiveMinPrices = new List<PricePoint>
        {
            _fixture
                .Build<PricePoint>()
                .With(p => p.CurrencyCode, "USD")
                .With(p => p.Time, baseTime.AddHours(-1))
                .With(p => p.Price, 120m)
                .Create()
        };

        _instrumentRepository
            .Setup(m => m.ListAllAsync())
            .ReturnsAsync(new List<Instrument> { instrument });
        var priceFetcher =
            _fixture.CreatePriceFetcherMockReturningHistoricalPrices(instrument, dailyPrices, hourlyPrices,
                fiveMinPrices);

        var sut = _fixture.Create<MissingInstrumentPricesFetchJob>();

        await sut.RunAsync();

        priceFetcher.Verify(m => m.GetHistoricalDailyPricesAsync(
            instrument.Symbol,
            instrument.CurrencyCode,
            PortEvalConstants.FinancialDataStartTime,
            It.IsInRange(baseTime.AddDays(-5).AddMinutes(-5), baseTime.AddDays(-5).AddMinutes(5), Range.Inclusive)
        ));
        priceFetcher.Verify(m => m.GetIntradayPricesAsync(
            instrument.Symbol,
            instrument.CurrencyCode,
            It.IsInRange(baseTime.AddDays(-5).AddMinutes(-5), baseTime.AddDays(-5).AddMinutes(5), Range.Inclusive),
            It.IsInRange(baseTime.AddDays(-1).AddMinutes(-5), baseTime.AddDays(-1).AddMinutes(5), Range.Inclusive),
            IntradayInterval.OneHour
        ));
        priceFetcher.Verify(m => m.GetIntradayPricesAsync(
            instrument.Symbol,
            instrument.CurrencyCode,
            It.IsInRange(baseTime.AddDays(-1).AddMinutes(-5), baseTime.AddDays(-1).AddMinutes(5), Range.Inclusive),
            It.IsInRange(baseTime.AddMinutes(-5), baseTime.AddMinutes(5), Range.Inclusive),
            IntradayInterval.FiveMinutes
        ));
    }
}