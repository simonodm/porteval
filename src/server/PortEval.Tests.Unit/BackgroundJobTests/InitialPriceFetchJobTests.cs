using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Features.Interfaces.Repositories;
using PortEval.Application.Models.PriceFetcher;
using PortEval.BackgroundJobs;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Tests.Unit.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.BackgroundJobTests
{
    public class InitialPriceFetchJobTests
    {
        private TimeSpan _expectedPriceTimePrecision;

        public InitialPriceFetchJobTests()
        {
            _expectedPriceTimePrecision = TimeSpan.FromMinutes(5);
        }

        [Fact]
        public async Task Run_ImportsDailyPricesFromFetcherResult()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var now = DateTime.UtcNow;

            var instrument = new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");

            var dailyPrices = new List<PricePoint>
            {
                fixture.Build<PricePoint>().With(p => p.Time, now.AddDays(-7)).With(p => p.Price, 100).Create(),
                fixture.Build<PricePoint>().With(p => p.Time, now.AddDays(-6)).With(p => p.Price, 101).Create(),
                fixture.Build<PricePoint>().With(p => p.Time, now.AddDays(-5)).With(p => p.Price, 102).Create(),
            };

            var exchangeRateRepository = fixture.CreateDefaultCurrencyExchangeRateRepositoryMock();
            var priceFetcher = fixture.CreatePriceFetcherMockReturningHistoricalPrices(instrument, dailyPrices, null, null);
            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(m => m.FindAsync(instrument.Id))
                .ReturnsAsync(instrument);
            var instrumentPriceRepository = fixture.Freeze<Mock<IInstrumentPriceRepository>>();

            var sut = fixture.Create<InitialPriceFetchJob>();

            await sut.Run(instrument.Id);

            instrumentPriceRepository.Verify(m => m.BulkInsertAsync(It.Is<IList<InstrumentPrice>>(prices =>
                prices.Any(p => p.InstrumentId == instrument.Id && p.Time.InRange(now.AddDays(-7), _expectedPriceTimePrecision) && p.Price == 100m) &&
                prices.Any(p => p.InstrumentId == instrument.Id && p.Time.InRange(now.AddDays(-6), _expectedPriceTimePrecision) && p.Price == 101m) &&
                prices.Any(p => p.InstrumentId == instrument.Id && p.Time.InRange(now.AddDays(-5), _expectedPriceTimePrecision) && p.Price == 102m)
            )));
        }

        [Fact]
        public async Task Run_ImportsHourlyPricesFromFetcherResult()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var now = DateTime.UtcNow;

            var instrument = new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");

            var hourlyPrices = new List<PricePoint>
            {
                fixture.Build<PricePoint>().With(p => p.Time, now.AddHours(-48)).With(p => p.Price, 100).Create(),
                fixture.Build<PricePoint>().With(p => p.Time, now.AddHours(-47)).With(p => p.Price, 101).Create(),
                fixture.Build<PricePoint>().With(p => p.Time, now.AddHours(-46)).With(p => p.Price, 102).Create(),
            };

            var exchangeRateRepository = fixture.CreateDefaultCurrencyExchangeRateRepositoryMock();
            var priceFetcher = fixture.CreatePriceFetcherMockReturningHistoricalPrices(instrument, null, hourlyPrices, null);
            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(m => m.FindAsync(instrument.Id))
                .ReturnsAsync(instrument);
            var instrumentPriceRepository = fixture.Freeze<Mock<IInstrumentPriceRepository>>();

            var sut = fixture.Create<InitialPriceFetchJob>();

            await sut.Run(instrument.Id);

            instrumentPriceRepository.Verify(m => m.BulkInsertAsync(It.Is<IList<InstrumentPrice>>(prices =>
                prices.Any(p => p.InstrumentId == instrument.Id && p.Time.InRange(now.AddHours(-48), _expectedPriceTimePrecision) && p.Price == 100m) &&
                prices.Any(p => p.InstrumentId == instrument.Id && p.Time.InRange(now.AddHours(-47), _expectedPriceTimePrecision) && p.Price == 101m) &&
                prices.Any(p => p.InstrumentId == instrument.Id && p.Time.InRange(now.AddHours(-46), _expectedPriceTimePrecision) && p.Price == 102m)
            )));
        }

        [Fact]
        public async Task Run_ImportsFiveMinutePricesFromFetcherResult()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var now = DateTime.UtcNow;

            var instrument = new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");

            var latestPrices = new List<PricePoint>
            {
                fixture.Build<PricePoint>().With(p => p.Time, now.AddMinutes(-15)).With(p => p.Price, 100).Create(),
                fixture.Build<PricePoint>().With(p => p.Time, now.AddMinutes(-10)).With(p => p.Price, 101).Create(),
                fixture.Build<PricePoint>().With(p => p.Time, now.AddMinutes(-5)).With(p => p.Price, 102).Create(),
            };

            var exchangeRateRepository = fixture.CreateDefaultCurrencyExchangeRateRepositoryMock();
            var priceFetcher = fixture.CreatePriceFetcherMockReturningHistoricalPrices(instrument, null, null, latestPrices);
            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(m => m.FindAsync(instrument.Id))
                .ReturnsAsync(instrument);
            var instrumentPriceRepository = fixture.Freeze<Mock<IInstrumentPriceRepository>>();

            var sut = fixture.Create<InitialPriceFetchJob>();

            await sut.Run(instrument.Id);

            instrumentPriceRepository.Verify(m => m.BulkInsertAsync(It.Is<IList<InstrumentPrice>>(prices =>
                prices.Any(p => p.InstrumentId == instrument.Id && p.Time.InRange(now.AddMinutes(-15), _expectedPriceTimePrecision) && p.Price == 100m) &&
                prices.Any(p => p.InstrumentId == instrument.Id && p.Time.InRange(now.AddMinutes(-10), _expectedPriceTimePrecision) && p.Price == 101m) &&
                prices.Any(p => p.InstrumentId == instrument.Id && p.Time.InRange(now.AddMinutes(-5), _expectedPriceTimePrecision) && p.Price == 102m)
            )));
        }

        [Fact]
        public async Task Run_FillsMissingDailyPrices()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var now = DateTime.UtcNow;

            var instrument = new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");

            var dailyPrices = new List<PricePoint>
            {
                fixture.Build<PricePoint>().With(p => p.Time, now.AddDays(-8)).With(p => p.Price, 100).Create(),
                fixture.Build<PricePoint>().With(p => p.Time, now.AddDays(-6)).With(p => p.Price, 102).Create(),
            };

            var exchangeRateRepository = fixture.CreateDefaultCurrencyExchangeRateRepositoryMock();
            var priceFetcher = fixture.CreatePriceFetcherMockReturningHistoricalPrices(instrument, dailyPrices, null, null);
            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(m => m.FindAsync(instrument.Id))
                .ReturnsAsync(instrument);
            var instrumentPriceRepository = fixture.Freeze<Mock<IInstrumentPriceRepository>>();

            var sut = fixture.Create<InitialPriceFetchJob>();

            await sut.Run(instrument.Id);

            instrumentPriceRepository.Verify(m => m.BulkInsertAsync(It.Is<IList<InstrumentPrice>>(prices =>
                prices.Any(p => p.InstrumentId == instrument.Id && p.Time.InRange(now.AddDays(-7), TimeSpan.FromDays(1)) && p.Price == 100m) &&
                prices.Any(p => p.InstrumentId == instrument.Id && p.Time.InRange(now.AddDays(-5), TimeSpan.FromDays(1)) && p.Price == 102m)
            )));
        }

        [Fact]
        public async Task Run_FillsMissingHourlyPrices()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var now = DateTime.UtcNow;

            var instrument = new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");

            var hourlyPrices = new List<PricePoint>
            {
                fixture.Build<PricePoint>().With(p => p.Time, now.AddHours(-48)).With(p => p.Price, 100).Create(),
                fixture.Build<PricePoint>().With(p => p.Time, now.AddHours(-46)).With(p => p.Price, 102).Create(),
            };

            var exchangeRateRepository = fixture.CreateDefaultCurrencyExchangeRateRepositoryMock();
            var priceFetcher = fixture.CreatePriceFetcherMockReturningHistoricalPrices(instrument, null, hourlyPrices, null);
            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(m => m.FindAsync(instrument.Id))
                .ReturnsAsync(instrument);
            var instrumentPriceRepository = fixture.Freeze<Mock<IInstrumentPriceRepository>>();

            var sut = fixture.Create<InitialPriceFetchJob>();

            await sut.Run(instrument.Id);

            instrumentPriceRepository.Verify(m => m.BulkInsertAsync(It.Is<IList<InstrumentPrice>>(prices =>
                prices.Any(p => p.InstrumentId == instrument.Id && p.Time.InRange(now.AddHours(-47), TimeSpan.FromHours(1)) && p.Price == 100m) &&
                prices.Any(p => p.InstrumentId == instrument.Id && p.Time.InRange(now.AddHours(-1), TimeSpan.FromHours(1)) && p.Price == 102m)
            )));
        }

        [Fact]
        public async Task Run_FillsMissingFiveMinutePrices()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var now = DateTime.UtcNow;

            var instrument = new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");

            var latestPrices = new List<PricePoint>
            {
                fixture.Build<PricePoint>().With(p => p.Time, now.AddMinutes(-30)).With(p => p.Price, 100).Create(),
            };

            var exchangeRateRepository = fixture.CreateDefaultCurrencyExchangeRateRepositoryMock();
            var priceFetcher = fixture.CreatePriceFetcherMockReturningHistoricalPrices(instrument, null, null, latestPrices);
            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(m => m.FindAsync(instrument.Id))
                .ReturnsAsync(instrument);
            var instrumentPriceRepository = fixture.Freeze<Mock<IInstrumentPriceRepository>>();

            var sut = fixture.Create<InitialPriceFetchJob>();

            await sut.Run(instrument.Id);

            instrumentPriceRepository.Verify(m => m.BulkInsertAsync(It.Is<IList<InstrumentPrice>>(prices =>
                prices.Any(p => p.InstrumentId == instrument.Id && p.Time.InRange(now.AddMinutes(-25), _expectedPriceTimePrecision) && p.Price == 100m) &&
                prices.Any(p => p.InstrumentId == instrument.Id && p.Time.InRange(now.AddMinutes(-15), _expectedPriceTimePrecision) && p.Price == 100m) &&
                prices.Any(p => p.InstrumentId == instrument.Id && p.Time.InRange(now.AddMinutes(-10), _expectedPriceTimePrecision) && p.Price == 100m)
            )));
        }

        [Fact]
        public async Task Run_SetsInstrumentTrackingInformationToFirstPriceTime_WhenAnyPricesAreDownloaded()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var now = DateTime.UtcNow;

            var instrument = new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");

            var latestPrices = new List<PricePoint>
            {
                fixture.Build<PricePoint>().With(p => p.Time, now.AddMinutes(-30)).With(p => p.Price, 100).Create(),
            };

            var exchangeRateRepository = fixture.CreateDefaultCurrencyExchangeRateRepositoryMock();
            var priceFetcher = fixture.CreatePriceFetcherMockReturningHistoricalPrices(instrument, null, null, latestPrices);
            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(m => m.FindAsync(instrument.Id))
                .ReturnsAsync(instrument);
            var instrumentPriceRepository = fixture.Freeze<Mock<IInstrumentPriceRepository>>();

            var sut = fixture.Create<InitialPriceFetchJob>();

            await sut.Run(instrument.Id);

            instrumentRepository.Verify(m => m.Update(It.Is<Instrument>(i =>
                i.Id == instrument.Id &&
                i.TrackingInfo.StartTime == now.AddMinutes(-30) &&
                i.TrackingInfo.LastUpdate >= now &&
                i.TrackingStatus == InstrumentTrackingStatus.Tracked
            )));
        }

        [Fact]
        public async Task Run_DoesNotChangeInstrumentTrackingInformation_WhenNoPricesAreDownloaded()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var now = DateTime.UtcNow;

            var instrument = new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");

            var exchangeRateRepository = fixture.CreateDefaultCurrencyExchangeRateRepositoryMock();
            var priceFetcher = fixture.CreatePriceFetcherMockReturningHistoricalPrices(instrument, null, null, null);
            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(m => m.FindAsync(instrument.Id))
                .ReturnsAsync(instrument);
            var instrumentPriceRepository = fixture.Freeze<Mock<IInstrumentPriceRepository>>();

            var sut = fixture.Create<InitialPriceFetchJob>();

            await sut.Run(instrument.Id);

            instrumentRepository.Verify(m => m.Update(It.IsAny<Instrument>()), Times.Never());
        }
    }
}
