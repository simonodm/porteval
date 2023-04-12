using Microsoft.Extensions.DependencyInjection;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;
using PortEval.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Integration.QueryTests
{
    [Collection("Query test collection")]
    public class InstrumentQueriesTests
    {
        private readonly IInstrumentQueries _instrumentQueries;

        private readonly int _appleInstrumentId;
        private readonly int _appleSplitId;
        private readonly int _btcInstrumentId;

        public InstrumentQueriesTests(QueryTestFixture fixture)
        {
            var scope = fixture.Factory.Services.CreateScope();
            _instrumentQueries = scope.ServiceProvider.GetRequiredService<IInstrumentQueries>();
            var context = scope.ServiceProvider.GetRequiredService<PortEvalDbContext>();

            _appleInstrumentId = context.Instruments.Where(i => i.Symbol == "AAPL").Select(i => i.Id).FirstOrDefault();
            _appleSplitId = context.InstrumentSplits.Where(s => s.InstrumentId == _appleInstrumentId).Select(s => s.Id)
                .FirstOrDefault();
            _btcInstrumentId = context.Instruments.Where(i => i.Symbol == "BTC").Select(i => i.Id).FirstOrDefault();
        }

        [Fact]
        public async Task GetAllInstruments_ReturnsAllInstrumentsFromDb()
        {
            var queryResult = await _instrumentQueries.GetAllInstrumentsAsync();

            Assert.Collection(queryResult, AssertIsAAPLInstrument, AssertIsBTCInstrument);
        }

        [Fact]
        public async Task GetInstrumentPage_ReturnsAllInstrumentsCorrectlyPaginated()
        {
            var firstPageQueryResult = await _instrumentQueries.GetInstrumentPageAsync(new PaginationParams
            {
                Limit = 1,
                Page = 1
            });
            var secondPageQueryResult = await _instrumentQueries.GetInstrumentPageAsync(new PaginationParams
            {
                Limit = 1,
                Page = 2
            });

            Assert.Collection(firstPageQueryResult, AssertIsAAPLInstrument);
            Assert.Collection(secondPageQueryResult, AssertIsBTCInstrument);
        }

        [Fact]
        public async Task GetInstrument_ReturnsCorrectInstrument_WhenInstrumentExists()
        {
            var queryResult = await _instrumentQueries.GetInstrumentAsync(_appleInstrumentId);

            AssertIsAAPLInstrument(queryResult);
        }

        [Fact]
        public async Task GetInstrument_ReturnsNull_WhenInstrumentDoesNotExist()
        {
            var queryResult = await _instrumentQueries.GetInstrumentAsync(-1);

            Assert.Null(queryResult);
        }

        [Fact]
        public async Task GetInstrumentPrices_ReturnsInstrumentPricesInRange()
        {
            var queryResult = await _instrumentQueries.GetInstrumentPricesAsync(_appleInstrumentId, DateTime.UtcNow.AddDays(-1).AddHours(-1), DateTime.UtcNow);

            Assert.Collection(queryResult, AssertIsAAPLYesterdaysPrice, AssertIsAAPLCurrentPrice);
        }

        [Fact]
        public async Task GetInstrumentPricesPage_ReturnsCorrectlyPaginatedPrices()
        {
            var firstPageQueryResult = await _instrumentQueries.GetInstrumentPricesPageAsync(_appleInstrumentId, DateTime.MinValue, DateTime.UtcNow,
                new PaginationParams
                {
                    Limit = 2,
                    Page = 1
                }, null);

            var secondPageQueryResult = await _instrumentQueries.GetInstrumentPricesPageAsync(_appleInstrumentId, DateTime.MinValue, DateTime.UtcNow,
                new PaginationParams
                {
                    Limit = 2,
                    Page = 2
                }, null);

            Assert.Collection(firstPageQueryResult, AssertIsAAPLCurrentPrice, AssertIsAAPLYesterdaysPrice);
            Assert.Collection(secondPageQueryResult, AssertIsAAPLTwoDaysOldPrice);
        }

        [Fact]
        public async Task GetInstrumentPricesPageCompressed_ReturnsCompressedPrices_WhenCompressedFlagIsTrue()
        {
            var queryResult = await _instrumentQueries.GetInstrumentPricesPageCompressedAsync(_btcInstrumentId, DateTime.MinValue, DateTime.UtcNow, new PaginationParams
            {
                Limit = 300,
                Page = 1
            });

            Assert.Collection(queryResult, AssertIsBTCCurrentPrice, AssertIsBTCYesterdaysPrice, AssertIsBTCTwoDaysOldPrice);
        }

        [Fact]
        public async Task GetInstrumentPricesPage_ReturnsCorrectlyAggregatedPrices_WhenAggregationFrequencyIsProvided()
        {
            var queryResult = await _instrumentQueries.GetInstrumentPricesPageAsync(_btcInstrumentId, DateTime.MinValue, DateTime.UtcNow, new PaginationParams
            {
                Limit = 300,
                Page = 1
            }, AggregationFrequency.Day);

            Assert.Collection(queryResult, AssertIsBTCCurrentPrice, AssertIsBTCYesterdaysPrice, AssertIsBTCTwoDaysOldPrice);
        }

        [Fact]
        public async Task GetInstrumentSplits_ReturnsInstrumentSplits()
        {
            var queryResult = await _instrumentQueries.GetInstrumentSplitsAsync(_appleInstrumentId);

            Assert.Collection(queryResult, AssertIsAAPLYesterdaysSplit);
        }

        [Fact]
        public async Task GetInstrumentSplit_ReturnsInstrumentSplit()
        {
            var queryResult = await _instrumentQueries.GetInstrumentSplitAsync(_appleInstrumentId, _appleSplitId);

            AssertIsAAPLYesterdaysSplit(queryResult);
        }

        [Fact]
        public async Task GetInstrumentPrice_ReturnsLatestPriceAtTime_WhenPriceExists()
        {
            var queryResult = await _instrumentQueries.GetInstrumentPriceAsync(_btcInstrumentId, DateTime.UtcNow.AddDays(-1));

            AssertIsBTCYesterdaysPrice(queryResult);
        }

        [Fact]
        public async Task GetInstrumentPrice_ReturnsNull_WhenInstrumentDoesNotExist()
        {
            var queryResult = await _instrumentQueries.GetInstrumentPriceAsync(-1, DateTime.UtcNow);

            Assert.Null(queryResult);
        }

        [Fact]
        public async Task GetInstrumentPrice_ReturnsNull_WhenNoPriceAtProvidedTimeExists()
        {
            var queryResult = await _instrumentQueries.GetInstrumentPriceAsync(_btcInstrumentId, DateTime.UtcNow.AddDays(-7));

            Assert.Null(queryResult);
        }

        private void AssertIsAAPLInstrument(InstrumentDto i)
        {
            Assert.Equal(_appleInstrumentId, i.Id);
            Assert.Equal("AAPL", i.Symbol);
            Assert.Equal("Apple Inc.", i.Name);
            Assert.Equal("USD", i.CurrencyCode);
            Assert.Equal("NASDAQ", i.Exchange);
            Assert.Equal("", i.Note);
        }

        private void AssertIsBTCInstrument(InstrumentDto i)
        {
            Assert.Equal(_btcInstrumentId, i.Id);
            Assert.Equal("BTC", i.Symbol);
            Assert.Equal("Bitcoin USD", i.Name);
            Assert.Equal("USD", i.CurrencyCode);
            Assert.True(string.IsNullOrEmpty(i.Exchange));
            Assert.Equal("bitcoin", i.Note);
        }

        private void AssertIsAAPLCurrentPrice(InstrumentPriceDto p)
        {
            Assert.Equal(_appleInstrumentId, p.InstrumentId);
            Assert.Equal(150m, p.Price);
            Assert.Equal(DateTime.UtcNow, p.Time, TimeSpan.FromHours(1));
        }

        private void AssertIsAAPLYesterdaysPrice(InstrumentPriceDto p)
        {
            Assert.Equal(_appleInstrumentId, p.InstrumentId);
            Assert.Equal(140m, p.Price);
            Assert.Equal(DateTime.UtcNow.AddDays(-1), p.Time, TimeSpan.FromHours(1));
        }

        private void AssertIsAAPLTwoDaysOldPrice(InstrumentPriceDto p)
        {
            Assert.Equal(_appleInstrumentId, p.InstrumentId);
            Assert.Equal(130m, p.Price);
            Assert.Equal(DateTime.UtcNow.AddDays(-2), p.Time, TimeSpan.FromHours(1));
        }

        private void AssertIsBTCCurrentPrice(InstrumentPriceDto p)
        {
            Assert.Equal(_btcInstrumentId, p.InstrumentId);
            Assert.Equal(1000m, p.Price);
            Assert.Equal(DateTime.UtcNow, p.Time, TimeSpan.FromHours(1));
        }

        private void AssertIsBTCYesterdaysPrice(InstrumentPriceDto p)
        {
            Assert.Equal(_btcInstrumentId, p.InstrumentId);
            Assert.Equal(2000m, p.Price);
            Assert.Equal(DateTime.UtcNow.AddDays(-1), p.Time, TimeSpan.FromHours(1));
        }

        private void AssertIsBTCTwoDaysOldPrice(InstrumentPriceDto p)
        {
            Assert.Equal(_btcInstrumentId, p.InstrumentId);
            Assert.Equal(4000m, p.Price);
            Assert.Equal(DateTime.UtcNow.AddDays(-2), p.Time, TimeSpan.FromHours(1));
        }

        private void AssertIsAAPLYesterdaysSplit(InstrumentSplitDto split)
        {
            Assert.Equal(_appleSplitId, split.Id);
            Assert.Equal(_appleInstrumentId, split.InstrumentId);
            Assert.Equal(3, split.SplitRatioNumerator);
            Assert.Equal(1, split.SplitRatioDenominator);
            Assert.Equal(DateTime.UtcNow.AddDays(-1), split.Time, TimeSpan.FromHours(1));
        }
    }
}
