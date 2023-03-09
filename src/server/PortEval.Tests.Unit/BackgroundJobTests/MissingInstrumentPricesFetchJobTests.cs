using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Models.PriceFetcher;
using PortEval.BackgroundJobs;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Tests.Unit.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Domain;
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
                instrument.Symbol,
                instrument.CurrencyCode,
                PortEvalConstants.FinancialDataStartTime,
                It.IsInRange(baseTime.AddDays(-5).AddMinutes(-5), baseTime.AddDays(-5).AddMinutes(5), Range.Inclusive)));

            priceRepository.Verify(m => m.BulkUpsertAsync(It.Is<IList<InstrumentPrice>>(list =>
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
                instrument.Symbol,
                instrument.CurrencyCode,
                It.IsInRange(baseTime.AddDays(-5).AddMinutes(-5), baseTime.AddDays(-5).AddMinutes(5), Range.Inclusive),
                It.IsInRange(baseTime.AddDays(-1).AddMinutes(-5), baseTime.AddDays(-1).AddMinutes(5), Range.Inclusive),
                IntradayInterval.OneHour
            ));
            priceRepository.Verify(m => m.BulkUpsertAsync(It.Is<IList<InstrumentPrice>>(list =>
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
                instrument.Symbol,
                instrument.CurrencyCode,
                It.IsInRange(baseTime.AddDays(-1).AddMinutes(-5), baseTime.AddDays(-1).AddMinutes(5), Range.Inclusive),
                It.IsInRange(baseTime.AddMinutes(-5), baseTime.AddMinutes(5), Range.Inclusive),
                IntradayInterval.FiveMinutes
            ));
            priceRepository.Verify(m => m.BulkUpsertAsync(It.Is<IList<InstrumentPrice>>(list =>
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
                instrument.Symbol,
                instrument.CurrencyCode,
                PortEvalConstants.FinancialDataStartTime,
                It.IsInRange(baseTime.AddDays(-5).AddMinutes(-5), baseTime.AddDays(-5).AddMinutes(5), Range.Inclusive)
            ));
            priceFetcher.Verify(m => m.GetIntradayPrices(
                instrument.Symbol,
                instrument.CurrencyCode,
                It.IsInRange(baseTime.AddDays(-5).AddMinutes(-5), baseTime.AddDays(-5).AddMinutes(5), Range.Inclusive),
                It.IsInRange(baseTime.AddDays(-1).AddMinutes(-5), baseTime.AddDays(-1).AddMinutes(5), Range.Inclusive),
                IntradayInterval.OneHour
            ));
            priceFetcher.Verify(m => m.GetIntradayPrices(
                instrument.Symbol,
                instrument.CurrencyCode,
                It.IsInRange(baseTime.AddDays(-1).AddMinutes(-5), baseTime.AddDays(-1).AddMinutes(5), Range.Inclusive),
                It.IsInRange(baseTime.AddMinutes(-5), baseTime.AddMinutes(5), Range.Inclusive),
                IntradayInterval.FiveMinutes
            ));
        }
    }
}
