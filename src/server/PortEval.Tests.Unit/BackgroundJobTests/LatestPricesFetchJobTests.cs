using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.BackgroundJobs;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.FinancialDataFetcher.Interfaces;
using PortEval.FinancialDataFetcher.Models;
using PortEval.FinancialDataFetcher.Responses;
using PortEval.Tests.Unit.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(m => m.ListAllAsync())
                .ReturnsAsync(new List<Instrument> { instrument });
            var exchangeRateRepository = fixture.CreateDefaultCurrencyExchangeRateRepositoryMock();
            var instrumentPriceRepository = fixture.CreateDefaultInstrumentPriceRepositoryMock();

            var sut = fixture.Create<LatestPricesFetchJob>();

            await sut.Run();

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
            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(m => m.ListAllAsync())
                .ReturnsAsync(new List<Instrument> { instrument });
            var exchangeRateRepository = fixture.CreateDefaultCurrencyExchangeRateRepositoryMock();
            var instrumentPriceRepository = fixture.CreateDefaultInstrumentPriceRepositoryMock();

            var sut = fixture.Create<LatestPricesFetchJob>();

            await sut.Run();

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
            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(m => m.ListAllAsync())
                .ReturnsAsync(new List<Instrument> { instrument });
            var exchangeRateRepository = fixture.CreateDefaultCurrencyExchangeRateRepositoryMock();
            var instrumentPriceRepository = fixture.CreateDefaultInstrumentPriceRepositoryMock();

            var sut = fixture.Create<LatestPricesFetchJob>();

            await sut.Run();

            instrumentRepository.Verify(m => m.Update(It.Is<Instrument>(i => i.Id == instrument.Id)), Times.Never());
        }

        private Mock<IPriceFetcher> CreatePriceFetcherMockReturningLatestPriceData(IFixture fixture, Instrument instrument, PricePoint price)
        {
            var priceFetcher = fixture.Freeze<Mock<IPriceFetcher>>();
            priceFetcher
                .Setup(m => m.GetLatestInstrumentPrice(instrument.Symbol, instrument.CurrencyCode))
                .ReturnsAsync(new Response<PricePoint>
                {
                    StatusCode = price == null ? StatusCode.ConnectionError : StatusCode.Ok,
                    Result = price
                });

            return priceFetcher;
        }
    }
}
