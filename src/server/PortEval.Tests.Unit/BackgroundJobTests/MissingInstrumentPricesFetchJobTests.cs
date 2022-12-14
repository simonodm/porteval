using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.BackgroundJobs.MissingPricesFetch;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.FinancialDataFetcher.Models;
using PortEval.Tests.Unit.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Range = Moq.Range;

namespace PortEval.Tests.Unit.BackgroundJobTests
{
    public class MissingInstrumentPricesFetchJobTests
    {
        [Fact]
        public async Task Run_ImportsMissingDailyPricesFromFetcherResult()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var baseTime = DateTime.UtcNow;
            var instrument = new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");
            instrument.SetTrackingFrom(DateTime.UtcNow.AddMonths(-1));
            var prices = new List<PricePoint>
            {
                fixture
                    .Build<PricePoint>()
                    .With(p => p.CurrencyCode, "USD")
                    .With(p => p.Time, baseTime.AddDays(-6))
                    .With(p => p.Price, 100m)
                    .Create()
            };

            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(m => m.ListAllAsync())
                .ReturnsAsync(new List<Instrument> { instrument });
            var priceRepository = fixture.CreateDefaultInstrumentPriceRepositoryMock();
            var priceFetcher = fixture.CreatePriceFetcherMockReturningHistoricalPrices(instrument, prices, null, null);


            var sut = fixture.Create<MissingInstrumentPricesFetchJob>();

            await sut.Run();

            priceFetcher.Verify(m => m.GetHistoricalDailyPrices(
                It.Is<Instrument>(i => i.Id == instrument.Id),
                instrument.TrackingInfo.StartTime,
                It.IsInRange(baseTime.AddDays(-5).AddMinutes(-5), baseTime.AddDays(-5).AddMinutes(5), Range.Inclusive)));
            priceRepository.Verify(m => m.BulkInsertAsync(It.Is<IList<InstrumentPrice>>(list =>
                list.Any(price => price.Time == prices[0].Time && price.Price == prices[0].Price && price.InstrumentId == instrument.Id)
            )));
        }

        [Fact]
        public async Task Run_ImportsMissingHourlyPricesFromFetcherResult()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var baseTime = DateTime.UtcNow;
            var instrument = new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");
            instrument.SetTrackingFrom(baseTime.AddMonths(-1));
            var prices = new List<PricePoint>
            {
                fixture
                    .Build<PricePoint>()
                    .With(p => p.CurrencyCode, "USD")
                    .With(p => p.Time, baseTime.AddHours(-48))
                    .With(p => p.Price, 100m)
                    .Create()
            };

            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(m => m.ListAllAsync())
                .ReturnsAsync(new List<Instrument> { instrument });
            var priceRepository = fixture.CreateDefaultInstrumentPriceRepositoryMock();
            var priceFetcher = fixture.CreatePriceFetcherMockReturningHistoricalPrices(instrument, null, prices, null);


            var sut = fixture.Create<MissingInstrumentPricesFetchJob>();

            await sut.Run();

            priceFetcher.Verify(m => m.GetIntradayPrices(
                It.Is<Instrument>(i => i.Id == instrument.Id),
                It.IsInRange(baseTime.AddDays(-5).AddMinutes(-5), baseTime.AddDays(-5).AddMinutes(5), Range.Inclusive),
                It.IsInRange(baseTime.AddDays(-1).AddMinutes(-5), baseTime.AddDays(-1).AddMinutes(5), Range.Inclusive),
                IntradayInterval.OneHour    
            ));
            priceRepository.Verify(m => m.BulkInsertAsync(It.Is<IList<InstrumentPrice>>(list =>
                list.Any(price => price.Time == prices[0].Time && price.Price == prices[0].Price && price.InstrumentId == instrument.Id)
            )));
        }

        [Fact]
        public async Task Run_ImportsMissingFiveMinutePricesFromFetcherResult()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var baseTime = DateTime.UtcNow;
            var instrument = new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");
            instrument.SetTrackingFrom(baseTime.AddMonths(-1));
            var prices = new List<PricePoint>
            {
                fixture
                    .Build<PricePoint>()
                    .With(p => p.CurrencyCode, "USD")
                    .With(p => p.Time, baseTime.AddMinutes(-10))
                    .With(p => p.Price, 100m)
                    .Create()
            };

            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(m => m.ListAllAsync())
                .ReturnsAsync(new List<Instrument> { instrument });
            var priceRepository = fixture.CreateDefaultInstrumentPriceRepositoryMock();
            var priceFetcher = fixture.CreatePriceFetcherMockReturningHistoricalPrices(instrument, null, null, prices);


            var sut = fixture.Create<MissingInstrumentPricesFetchJob>();

            await sut.Run();

            priceFetcher.Verify(m => m.GetIntradayPrices(
                It.Is<Instrument>(i => i.Id == instrument.Id),
                It.IsInRange(baseTime.AddDays(-1).AddMinutes(-5), baseTime.AddDays(-1).AddMinutes(5), Range.Inclusive),
                It.IsInRange(baseTime.AddMinutes(-5), baseTime.AddMinutes(5), Range.Inclusive),
                IntradayInterval.FiveMinutes
            ));
            priceRepository.Verify(m => m.BulkInsertAsync(It.Is<IList<InstrumentPrice>>(list =>
                list.Any(price => price.Time == prices[0].Time && price.Price == prices[0].Price && price.InstrumentId == instrument.Id)
            )));
        }

        [Fact]
        public async Task Run_SplitsMissingRangesCorrectly()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var baseTime = DateTime.UtcNow;
            var instrument = new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", "");
            instrument.SetTrackingFrom(baseTime.AddMonths(-1));
            var dailyPrices = new List<PricePoint>
            {
                fixture
                    .Build<PricePoint>()
                    .With(p => p.CurrencyCode, "USD")
                    .With(p => p.Time, baseTime.AddMonths(-1).AddDays(1))
                    .With(p => p.Price, 100m)
                    .Create()
            };
            var hourlyPrices = new List<PricePoint>
            {
                fixture
                    .Build<PricePoint>()
                    .With(p => p.CurrencyCode, "USD")
                    .With(p => p.Time, baseTime.AddDays(-4))
                    .With(p => p.Price, 110m)
                    .Create()
            };
            var fiveMinPrices = new List<PricePoint>
            {
                fixture
                    .Build<PricePoint>()
                    .With(p => p.CurrencyCode, "USD")
                    .With(p => p.Time, baseTime.AddHours(-1))
                    .With(p => p.Price, 120m)
                    .Create()
            };

            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(m => m.ListAllAsync())
                .ReturnsAsync(new List<Instrument> { instrument });
            var priceRepository = fixture.CreateDefaultInstrumentPriceRepositoryMock();
            var priceFetcher = fixture.CreatePriceFetcherMockReturningHistoricalPrices(instrument, dailyPrices, hourlyPrices, fiveMinPrices);

            var sut = fixture.Create<MissingInstrumentPricesFetchJob>();

            await sut.Run();

            priceFetcher.Verify(m => m.GetHistoricalDailyPrices(
                It.Is<Instrument>(i => i.Id == instrument.Id),
                It.IsInRange(instrument.TrackingInfo.StartTime.AddMinutes(-5), instrument.TrackingInfo.StartTime.AddMinutes(5), Range.Inclusive),
                It.IsInRange(baseTime.AddDays(-5).AddMinutes(-5), baseTime.AddDays(-5).AddMinutes(5), Range.Inclusive)
            ));
            priceFetcher.Verify(m => m.GetIntradayPrices(
                It.Is<Instrument>(i => i.Id == instrument.Id),
                It.IsInRange(baseTime.AddDays(-5).AddMinutes(-5), baseTime.AddDays(-5).AddMinutes(5), Range.Inclusive),
                It.IsInRange(baseTime.AddDays(-1).AddMinutes(-5), baseTime.AddDays(-1).AddMinutes(5), Range.Inclusive),
                IntradayInterval.OneHour
            ));
            priceFetcher.Verify(m => m.GetIntradayPrices(
                It.Is<Instrument>(i => i.Id == instrument.Id),
                It.IsInRange(baseTime.AddDays(-1).AddMinutes(-5), baseTime.AddDays(-1).AddMinutes(5), Range.Inclusive),
                It.IsInRange(baseTime.AddMinutes(-5), baseTime.AddMinutes(5), Range.Inclusive),
                IntradayInterval.FiveMinutes
            ));
        }

        [Fact]
        public async Task Run_ConvertsPriceToInstrumentCurrency_IfCurrenciesAreDifferent()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var baseTime = DateTime.UtcNow;
            var instrument = new Instrument(1, "Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "EUR", "");
            instrument.SetTrackingFrom(baseTime.AddMonths(-1));
            var prices = new List<PricePoint>
            {
                fixture
                    .Build<PricePoint>()
                    .With(p => p.CurrencyCode, "USD")
                    .With(p => p.Time, baseTime.AddHours(-12))
                    .With(p => p.Price, 100m)
                    .Create()
            };

            var instrumentRepository = fixture.CreateDefaultInstrumentRepositoryMock();
            instrumentRepository
                .Setup(m => m.ListAllAsync())
                .ReturnsAsync(new List<Instrument> { instrument });
            var priceRepository = fixture.CreateDefaultInstrumentPriceRepositoryMock();
            var priceFetcher = fixture.CreatePriceFetcherMockReturningHistoricalPrices(instrument, null, null, prices);
            var exchangeRateRepository = fixture.CreateDefaultCurrencyExchangeRateRepositoryMock();

            var sut = fixture.Create<MissingInstrumentPricesFetchJob>();

            await sut.Run();

            exchangeRateRepository.Verify(m =>
                m.GetExchangeRateAtAsync(
                    prices[0].CurrencyCode,
                    instrument.CurrencyCode,
                    It.IsInRange(prices[0].Time.AddMinutes(-5), prices[0].Time.AddMinutes(5), Range.Inclusive)));
        }
    }
}
