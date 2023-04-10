using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Tests.Unit.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Core.BackgroundJobs;
using PortEval.Application.Core.Interfaces;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Models.FinancialDataFetcher;
using Xunit;

namespace PortEval.Tests.Unit.BackgroundJobTests
{
    public class LatestPricesFetchJobTests
    {
        [Fact]
        public async Task Run_ImportsLatestPriceRetrievedFromFetcher()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrument = new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");
            instrument.SetTrackingFrom(DateTime.Parse("2022-01-01"));
            var price = fixture.Build<PricePoint>().With(p => p.Time, DateTime.UtcNow.AddMinutes(-10)).Create();

            var priceFetcher = CreatePriceFetcherMockReturningLatestPriceData(fixture, instrument, price);
            var instrumentRepository = fixture.Freeze<Mock<IInstrumentRepository>>();
            instrumentRepository
                .Setup(m => m.ListAllAsync())
                .ReturnsAsync(new List<Instrument> { instrument });
            instrumentRepository
                .Setup(m => m.Update(It.IsAny<Instrument>()))
                .Returns<Instrument>(i => i);
            var exchangeRateRepository = fixture.Freeze<Mock<ICurrencyExchangeRateRepository>>();
            var instrumentPriceRepository = fixture.Freeze<Mock<IInstrumentPriceRepository>>();
            instrumentPriceRepository
                .Setup(m => m.Add(It.IsAny<InstrumentPrice>()))
                .Returns<InstrumentPrice>(p => p);

            var sut = fixture.Create<LatestPricesFetchJob>();

            await sut.RunAsync();

            instrumentPriceRepository.Verify(m => m.Add(It.Is<InstrumentPrice>(p => p.Price == price.Price)));
        }

        [Fact]
        public async Task Run_UpdatesInstrumentsTrackingInfo_WhenPriceFetchSucceeds()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrument = new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");
            instrument.SetTrackingFrom(DateTime.Parse("2022-01-01"));
            var price = fixture.Build<PricePoint>().With(p => p.Time, DateTime.UtcNow).Create();

            var priceFetcher = CreatePriceFetcherMockReturningLatestPriceData(fixture, instrument, price);
            var instrumentRepository = fixture.Freeze<Mock<IInstrumentRepository>>();
            instrumentRepository
                .Setup(m => m.ListAllAsync())
                .ReturnsAsync(new List<Instrument> { instrument });
            instrumentRepository
                .Setup(m => m.Update(It.IsAny<Instrument>()))
                .Returns<Instrument>(i => i);
            var exchangeRateRepository = fixture.Freeze<Mock<ICurrencyExchangeRateRepository>>();
            var instrumentPriceRepository = fixture.Freeze<Mock<IInstrumentPriceRepository>>();
            instrumentPriceRepository
                .Setup(m => m.Add(It.IsAny<InstrumentPrice>()))
                .Returns<InstrumentPrice>(p => p);

            var sut = fixture.Create<LatestPricesFetchJob>();

            await sut.RunAsync();

            instrumentRepository.Verify(m => m.Update(It.Is<Instrument>(i => i.Id == instrument.Id && i.TrackingInfo.LastUpdate >= price.Time)));
        }

        [Fact]
        public async Task Run_DoesNotUpdateTrackingInfo_WhenNoPricesAreRetrieved()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrument = new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");
            instrument.SetTrackingFrom(DateTime.Parse("2022-01-01"));

            var priceFetcher = CreatePriceFetcherMockReturningLatestPriceData(fixture, instrument, null);
            var instrumentRepository = fixture.Freeze<Mock<IInstrumentRepository>>();
            instrumentRepository
                .Setup(m => m.ListAllAsync())
                .ReturnsAsync(new List<Instrument> { instrument });
            instrumentRepository
                .Setup(m => m.Update(It.IsAny<Instrument>()))
                .Returns<Instrument>(i => i);
            var exchangeRateRepository = fixture.Freeze<Mock<ICurrencyExchangeRateRepository>>();
            var instrumentPriceRepository = fixture.Freeze<Mock<IInstrumentPriceRepository>>();

            var sut = fixture.Create<LatestPricesFetchJob>();

            await sut.RunAsync();

            instrumentRepository.Verify(m => m.Update(It.Is<Instrument>(i => i.Id == instrument.Id)), Times.Never());
        }

        private Mock<IFinancialDataFetcher> CreatePriceFetcherMockReturningLatestPriceData(IFixture fixture, Instrument instrument, PricePoint price)
        {
            var priceFetcher = fixture.Freeze<Mock<IFinancialDataFetcher>>();
            priceFetcher
                .Setup(m => m.GetLatestInstrumentPriceAsync(instrument.Symbol, instrument.CurrencyCode))
                .ReturnsAsync(price);

            return priceFetcher;
        }
    }
}
