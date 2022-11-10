using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Application.Services.Queries;
using PortEval.Application.Services.Queries.Interfaces;
using PortEval.Domain.Models.Enums;
using PortEval.Infrastructure;
using Xunit;

namespace PortEval.Tests.Functional.QueryTests
{
    [Collection("Integration test collection")]
    public class InstrumentQueriesTests
    {
        private readonly IInstrumentQueries _instrumentQueries;

        private readonly int _appleInstrumentId;
        private readonly int _btcInstrumentId;

        public InstrumentQueriesTests(IntegrationTestFixture fixture)
        {
            var scope = fixture.Factory.Services.CreateScope();
            _instrumentQueries = scope.ServiceProvider.GetRequiredService<IInstrumentQueries>();
            var context = scope.ServiceProvider.GetRequiredService<PortEvalDbContext>();

            _appleInstrumentId = context.Instruments.Where(i => i.Symbol == "AAPL").Select(i => i.Id).FirstOrDefault();
            _btcInstrumentId = context.Instruments.Where(i => i.Symbol == "BTC").Select(i => i.Id).FirstOrDefault();
        }

        [Fact]
        public async Task GetAllInstruments_ReturnsAllInstrumentsFromDb()
        {
            var queryResult = await _instrumentQueries.GetAllInstruments();

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            Assert.Collection(queryResult.Response, AssertIsAAPLInstrument, AssertIsBTCInstrument);
        }

        [Fact]
        public async Task GetInstrumentPage_ReturnsAllInstrumentsCorrectlyPaginated()
        {
            var firstPageQueryResult = await _instrumentQueries.GetInstrumentsPage(new PaginationParams
            {
                Limit = 1,
                Page = 1
            });
            var secondPageQueryResult = await _instrumentQueries.GetInstrumentsPage(new PaginationParams
            {
                Limit = 1,
                Page = 2
            });

            Assert.Equal(QueryStatus.Ok, firstPageQueryResult.Status);
            Assert.Equal(QueryStatus.Ok, secondPageQueryResult.Status);
            Assert.Equal(2, firstPageQueryResult.Response.TotalCount);
            Assert.Equal(1, firstPageQueryResult.Response.Page);
            Assert.Equal(2, secondPageQueryResult.Response.Page);
            Assert.Equal(1, firstPageQueryResult.Response.Count);
            Assert.Equal(1, secondPageQueryResult.Response.Count);
            Assert.Collection(firstPageQueryResult.Response.Data, AssertIsAAPLInstrument);
            Assert.Collection(secondPageQueryResult.Response.Data, AssertIsBTCInstrument);
        }

        [Fact]
        public async Task GetInstrument_ReturnsCorrectInstrument_WhenInstrumentExists()
        {
            var queryResult = await _instrumentQueries.GetInstrument(_appleInstrumentId);

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            AssertIsAAPLInstrument(queryResult.Response);
        }

        [Fact]
        public async Task GetInstrument_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var queryResult = await _instrumentQueries.GetInstrument(-1);

            Assert.Equal(QueryStatus.NotFound, queryResult.Status);
        }

        [Fact]
        public async Task GetInstrumentPrices_ReturnsInstrumentPricesInRange()
        {
            var queryResult = await _instrumentQueries.GetInstrumentPrices(_appleInstrumentId, new DateRangeParams
            {
                From = DateTime.UtcNow.AddDays(-1).AddHours(-1)
            });

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            Assert.Collection(queryResult.Response, AssertIsAAPLCurrentPrice, AssertIsAAPLYesterdayPrice);
        }

        [Fact]
        public async Task GetInstrumentPrices_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var queryResult = await _instrumentQueries.GetInstrumentPrices(-1, new DateRangeParams());

            Assert.Equal(QueryStatus.NotFound, queryResult.Status);
        }

        [Fact]
        public async Task GetInstrumentPricesPage_ReturnsCorrectlyPaginatedPrices()
        {
            var firstPageQueryResult = await _instrumentQueries.GetInstrumentPricesPage(_appleInstrumentId,
                new PaginationParams
                {
                    Limit = 2,
                    Page = 1
                }, new DateRangeParams());

            var secondPageQueryResult = await _instrumentQueries.GetInstrumentPricesPage(_appleInstrumentId,
                new PaginationParams
                {
                    Limit = 2,
                    Page = 2
                }, new DateRangeParams());

            Assert.Equal(QueryStatus.Ok, firstPageQueryResult.Status);
            Assert.Equal(QueryStatus.Ok, secondPageQueryResult.Status);
            Assert.Equal(3, firstPageQueryResult.Response.TotalCount);
            Assert.Equal(2, firstPageQueryResult.Response.Count);
            Assert.Equal(1, firstPageQueryResult.Response.Page);
            Assert.Equal(1, secondPageQueryResult.Response.Count);
            Assert.Equal(2, secondPageQueryResult.Response.Page);
            Assert.Collection(firstPageQueryResult.Response.Data, AssertIsAAPLCurrentPrice, AssertIsAAPLYesterdayPrice);
            Assert.Collection(secondPageQueryResult.Response.Data, AssertIsAAPLTwoDaysOldPrice);
        }

        [Fact]
        public async Task GetInstrumentPricesPage_ReturnsCompressedPrices_WhenCompressedFlagIsTrue()
        {
            var queryResult = await _instrumentQueries.GetInstrumentPricesPage(_btcInstrumentId, new PaginationParams
            {
                Limit = 300,
                Page = 1
            }, new DateRangeParams(), true);

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            Assert.Collection(queryResult.Response.Data, AssertIsBTCCurrentPrice, AssertIsBTCYesterdaysPrice, AssertIsBTCTwoDaysOldPrice);
        }

        [Fact]
        public async Task GetInstrumentPricesPage_ReturnsCorrectlyAggregatedPrices_WhenAggregationFrequencyIsProvided()
        {
            var queryResult = await _instrumentQueries.GetInstrumentPricesPage(_btcInstrumentId, new PaginationParams
            {
                Limit = 300,
                Page = 1
            }, new DateRangeParams(), false, AggregationFrequency.Day);

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            Assert.Collection(queryResult.Response.Data, AssertIsBTCCurrentPrice, AssertIsBTCYesterdaysPrice, AssertIsBTCTwoDaysOldPrice);
        }

        [Fact]
        public async Task GetInstrumentPrice_ReturnsLatestPriceAtTime_WhenPriceExists()
        {
            var queryResult = await _instrumentQueries.GetInstrumentPrice(_btcInstrumentId, DateTime.UtcNow.AddDays(-1));

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            AssertIsBTCYesterdaysPrice(queryResult.Response);
        }

        [Fact]
        public async Task GetInstrumentPrice_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var queryResult = await _instrumentQueries.GetInstrumentPrice(-1, DateTime.UtcNow);

            Assert.Equal(QueryStatus.NotFound, queryResult.Status);
        }

        [Fact]
        public async Task GetInstrumentPrice_ReturnsNull_WhenNoPriceAtProvidedTimeExists()
        {
            var queryResult = await _instrumentQueries.GetInstrumentPrice(_btcInstrumentId, DateTime.UtcNow.AddDays(-7));

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            Assert.Null(queryResult.Response);
        }

        [Fact]
        public async Task GetInstrumentProfit_ReturnsCorrectProfit()
        {
            var queryResult = await _instrumentQueries.GetInstrumentProfit(_btcInstrumentId, new DateRangeParams
            {
                From = DateTime.UtcNow.AddDays(-2)
            });

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            Assert.Equal(-3000m, queryResult.Response.Profit);
            Assert.Equal("USD", queryResult.Response.CurrencyCode);
        }

        [Fact]
        public async Task GetInstrumentProfit_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var queryResult = await _instrumentQueries.GetInstrumentProfit(-1, new DateRangeParams());

            Assert.Equal(QueryStatus.NotFound, queryResult.Status);
        }

        [Fact]
        public async Task GetInstrumentPerformance_ReturnsCorrectPerformance()
        {
            var queryResult = await _instrumentQueries.GetInstrumentPerformance(_btcInstrumentId, new DateRangeParams
            {
                From = DateTime.UtcNow.AddDays(-2)
            });

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            Assert.Equal(-0.75m, queryResult.Response.Performance);
        }

        [Fact]
        public async Task GetInstrumentPerformance_ReturnsNotFound_WhenInstrumentDoesNotExist()
        {
            var queryResult = await _instrumentQueries.GetInstrumentPerformance(-1, new DateRangeParams());

            Assert.Equal(QueryStatus.NotFound, queryResult.Status);
        }

        [Fact]
        public async Task ChartInstrumentPrices_ReturnsChartedInstrumentPricesConvertedToTargetCurrency_WhenTargetCurrencyIsNotInstrumentCurrency()
        {
            var queryResult =
                await _instrumentQueries.ChartInstrumentPrices(_btcInstrumentId, new DateRangeParams { From = DateTime.UtcNow.AddDays(-2).AddHours(1)},
                    AggregationFrequency.Day, "EUR");

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            Assert.Collection(queryResult.Response, p =>
            {
                Assert.Equal(3960m, p.Value);
                Assert.Equal(DateTime.UtcNow.Date.AddDays(-1), p.Time, TimeSpan.FromHours(1));
            }, p =>
            {
                Assert.Equal(2000m, p.Value);
                Assert.Equal(DateTime.UtcNow.Date, p.Time, TimeSpan.FromHours(1));
            }, p =>
            {
                Assert.Equal(1010m, p.Value);
                Assert.Equal(DateTime.UtcNow, p.Time, TimeSpan.FromHours(1));
            });
        }

        [Fact]
        public async Task
            ChartInstrumentProfit_ReturnsChartedInstrumentProfitConvertedToTargetCurrency_WhenTargetCurrencyIsNotInstrumentCurrency()
        {
            var queryResult =
                await _instrumentQueries.ChartInstrumentProfit(_btcInstrumentId, new DateRangeParams(),
                    AggregationFrequency.Day, "EUR");

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            Assert.Collection(queryResult.Response, p =>
            {
                Assert.Equal(0m, p.Value);
                Assert.Equal(DateTime.UtcNow.AddDays(-2), p.Time, TimeSpan.FromHours(1));
            }, p =>
            {
                Assert.Equal(0m, p.Value);
                Assert.Equal(DateTime.UtcNow.Date.AddDays(-1), p.Time, TimeSpan.FromHours(1));
            }, p =>
            {
                Assert.Equal(-2000m, p.Value);
                Assert.Equal(DateTime.UtcNow.Date, p.Time, TimeSpan.FromHours(1));
            }, p =>
            {
                Assert.Equal(-3030m, p.Value);
                Assert.Equal(DateTime.UtcNow, p.Time, TimeSpan.FromHours(1));
            });
        }

        [Fact]
        public async Task ChartInstrumentPerformance_ReturnsChartedInstrumentPerformance()
        {
            var queryResult = await _instrumentQueries.ChartInstrumentPerformance(_btcInstrumentId,
                new DateRangeParams(), AggregationFrequency.Day);

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            Assert.Collection(queryResult.Response, p =>
            {
                Assert.Equal(0m, p.Value);
                Assert.Equal(DateTime.UtcNow.AddDays(-2), p.Time, TimeSpan.FromHours(1));
            }, p =>
            {
                Assert.Equal(0m, p.Value);
                Assert.Equal(DateTime.UtcNow.Date.AddDays(-1), p.Time, TimeSpan.FromHours(1));
            }, p =>
            {
                Assert.Equal(-0.5m, p.Value);
                Assert.Equal(DateTime.UtcNow.Date, p.Time, TimeSpan.FromHours(1));
            }, p =>
            {
                Assert.Equal(-0.75m, p.Value);
                Assert.Equal(DateTime.UtcNow, p.Time, TimeSpan.FromHours(1));
            });
        }

        [Fact]
        public async Task ChartInstrumentAggregatedProfit_ReturnsCorrectAggregatedProfit()
        {
            var queryResult = await _instrumentQueries.ChartInstrumentProfitAggregated(_btcInstrumentId,
                new DateRangeParams(), AggregationFrequency.Day);

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            Assert.Collection(queryResult.Response, p =>
            {
                Assert.Equal(0m, p.Value);
                Assert.Equal(DateTime.UtcNow.Date.AddDays(-1), p.Time, TimeSpan.FromHours(1));
            }, p =>
            {
                Assert.Equal(-2000m, p.Value);
                Assert.Equal(DateTime.UtcNow.Date, p.Time, TimeSpan.FromHours(1));
            }, p =>
            {
                Assert.Equal(-1000m, p.Value);
                Assert.Equal(DateTime.UtcNow, p.Time, TimeSpan.FromHours(1));
            });
        }

        [Fact]
        public async Task ChartInstrumentAggregatedPerformance_ReturnsCorrectAggregatedPerformance()
        {
            var queryResult = await _instrumentQueries.ChartInstrumentPerformanceAggregated(_btcInstrumentId,
                new DateRangeParams(), AggregationFrequency.Day);

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            Assert.Collection(queryResult.Response, p =>
            {
                Assert.Equal(0, p.Value);
                Assert.Equal(DateTime.UtcNow.Date.AddDays(-1), p.Time, TimeSpan.FromHours(1));
            }, p =>
            {
                Assert.Equal(-0.5m, p.Value);
                Assert.Equal(DateTime.UtcNow.Date, p.Time, TimeSpan.FromHours(1));
            }, p =>
            {
                Assert.Equal(-0.5m, p.Value);
                Assert.Equal(DateTime.UtcNow, p.Time, TimeSpan.FromHours(1));
            });
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

        private void AssertIsAAPLYesterdayPrice(InstrumentPriceDto p)
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
    }
}
