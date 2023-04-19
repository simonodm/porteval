using System;
using System.Collections.Generic;
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
using PortEval.Tests.Unit.Helpers.Extensions;
using Xunit;

namespace PortEval.Tests.Unit.BackgroundJobTests;

public class LatestPricesFetchJobTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IInstrumentPriceRepository> _instrumentPriceRepository;
    private readonly Mock<IInstrumentRepository> _instrumentRepository;
    private Mock<ICurrencyExchangeRateRepository> _exchangeRateRepository;

    public LatestPricesFetchJobTests()
    {
        _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
        _instrumentRepository = _fixture.CreateDefaultInstrumentRepositoryMock();
        _instrumentPriceRepository = _fixture.CreateDefaultInstrumentPriceRepositoryMock();
        _exchangeRateRepository = _fixture.CreateDefaultCurrencyExchangeRateRepositoryMock();
    }

    [Fact]
    public async Task Run_ImportsLatestPriceRetrievedFromFetcher()
    {
        var instrument = new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");
        instrument.SetTrackingFrom(DateTime.Parse("2022-01-01"));
        var price = _fixture.Build<PricePoint>().With(p => p.Time, DateTime.UtcNow.AddMinutes(-10)).Create();

        CreatePriceFetcherMockReturningLatestPriceData(_fixture, instrument, price);
        _instrumentRepository
            .Setup(m => m.ListAllAsync())
            .ReturnsAsync(new List<Instrument> { instrument });
        _instrumentPriceRepository
            .Setup(m => m.Add(It.IsAny<InstrumentPrice>()))
            .Returns<InstrumentPrice>(p => p);

        var sut = _fixture.Create<LatestPricesFetchJob>();

        await sut.RunAsync();

        _instrumentPriceRepository.Verify(m => m.Add(It.Is<InstrumentPrice>(p => p.Price == price.Price)));
    }

    [Fact]
    public async Task Run_UpdatesInstrumentsTrackingInfo_WhenPriceFetchSucceeds()
    {
        var instrument = new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");
        instrument.SetTrackingFrom(DateTime.Parse("2022-01-01"));
        var price = _fixture.Build<PricePoint>().With(p => p.Time, DateTime.UtcNow).Create();

        CreatePriceFetcherMockReturningLatestPriceData(_fixture, instrument, price);
        _instrumentRepository
            .Setup(m => m.ListAllAsync())
            .ReturnsAsync(new List<Instrument> { instrument });

        var sut = _fixture.Create<LatestPricesFetchJob>();

        await sut.RunAsync();

        _instrumentRepository.Verify(m =>
            m.Update(It.Is<Instrument>(i => i.Id == instrument.Id && i.TrackingInfo.LastUpdate >= price.Time)));
    }

    [Fact]
    public async Task Run_DoesNotUpdateTrackingInfo_WhenNoPricesAreRetrieved()
    {
        var instrument = new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");
        instrument.SetTrackingFrom(DateTime.Parse("2022-01-01"));

        CreatePriceFetcherMockReturningLatestPriceData(_fixture, instrument, null);
        _instrumentRepository
            .Setup(m => m.ListAllAsync())
            .ReturnsAsync(new List<Instrument> { instrument });

        var sut = _fixture.Create<LatestPricesFetchJob>();

        await sut.RunAsync();

        _instrumentRepository.Verify(m => m.Update(It.Is<Instrument>(i => i.Id == instrument.Id)), Times.Never());
    }

    private Mock<IFinancialDataFetcher> CreatePriceFetcherMockReturningLatestPriceData(IFixture fixture,
        Instrument instrument, PricePoint price)
    {
        var priceFetcher = fixture.Freeze<Mock<IFinancialDataFetcher>>();
        priceFetcher
            .Setup(m => m.GetLatestInstrumentPriceAsync(instrument.Symbol, instrument.CurrencyCode))
            .ReturnsAsync(price);

        return priceFetcher;
    }
}