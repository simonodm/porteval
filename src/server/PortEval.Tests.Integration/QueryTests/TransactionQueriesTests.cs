using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Application.Services.Queries;
using PortEval.Application.Services.Queries.Interfaces;
using PortEval.Infrastructure;
using Xunit;

namespace PortEval.Tests.Integration.QueryTests
{
    [Collection("Query test collection")]
    public class TransactionQueriesTests
    {
        private readonly PortEvalDbContext _context;
        private readonly ITransactionQueries _transactionQueries;

        public TransactionQueriesTests(QueryTestFixture fixture)
        {
            var scope = fixture.Factory.Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<PortEvalDbContext>();
            _transactionQueries = scope.ServiceProvider.GetRequiredService<ITransactionQueries>();
        }

        [Fact]
        public async Task GetTransactions_ReturnsAllTransactions_WhenNoFiltersAreProvided()
        {
            var queryResult =
                await _transactionQueries.GetTransactions(new TransactionFilters(), new DateRangeParams());

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            Assert.Collection(queryResult.Response, AssertIsInitialAAPLTransaction, AssertIsInitialBTCTransaction);
        }

        [Fact]
        public async Task GetTransactions_ReturnsOnlyInstrumentTransactions_WhenInstrumentFilterIsProvided()
        {
            var appleInstrumentId =
                _context.Instruments.Where(i => i.Symbol == "AAPL").Select(i => i.Id).FirstOrDefault();

            var queryResult = await _transactionQueries.GetTransactions(new TransactionFilters
            {
                InstrumentId = appleInstrumentId
            }, new DateRangeParams());

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            Assert.Collection(queryResult.Response, AssertIsInitialAAPLTransaction);
        }

        [Fact]
        public async Task GetTransactions_ReturnsPortfolioTransactions_WhenPortfolioFilterIsProvided()
        {
            var portfolioId = _context.Portfolios.Where(p => p.Name == "Portfolio 1").Select(p => p.Id)
                .FirstOrDefault();

            var queryResult = await _transactionQueries.GetTransactions(new TransactionFilters
            {
                PortfolioId = portfolioId
            }, new DateRangeParams());

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            Assert.Collection(queryResult.Response, AssertIsInitialAAPLTransaction);
        }

        [Fact]
        public async Task GetTransactions_ReturnsPositionTransactions_WhenPositionFilterIsProvided()
        {
            var appleInstrumentId =
                _context.Instruments.Where(i => i.Symbol == "AAPL").Select(i => i.Id).FirstOrDefault();
            var positionId = _context.Positions.Where(p => p.InstrumentId == appleInstrumentId).Select(p => p.Id)
                .FirstOrDefault();

            var queryResult = await _transactionQueries.GetTransactions(new TransactionFilters
            {
                PositionId = positionId
            }, new DateRangeParams());

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            Assert.Collection(queryResult.Response, AssertIsInitialAAPLTransaction);
        }

        [Fact]
        public async Task GetTransactions_ReturnsCorrectTransactions_WhenMultipleFiltersAreProvided()
        {
            var appleInstrumentId =
                _context.Instruments.Where(i => i.Symbol == "AAPL").Select(i => i.Id).FirstOrDefault();
            var position = _context.Positions.FirstOrDefault(p => p.InstrumentId == appleInstrumentId);
            var positionId = position!.Id;
            var portfolioId = position!.PortfolioId;

            var queryResult = await _transactionQueries.GetTransactions(new TransactionFilters
            {
                InstrumentId = appleInstrumentId,
                PortfolioId = portfolioId,
                PositionId = positionId
            }, new DateRangeParams());

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            Assert.Collection(queryResult.Response, AssertIsInitialAAPLTransaction);
        }

        [Fact]
        public async Task GetTransactions_ReturnsTransactionsInRange_WhenDateRangeParamsAreProvided()
        {
            var queryResult = await _transactionQueries.GetTransactions(new TransactionFilters(), new DateRangeParams
            {
                From = DateTime.UtcNow.AddDays(-1).AddHours(-1)
            });

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            Assert.Collection(queryResult.Response, AssertIsInitialBTCTransaction);
        }

        [Fact]
        public async Task GetTransaction_ReturnsCorrectTransaction_WhenTransactionExists()
        {
            var appleInstrumentId =
                _context.Instruments.Where(i => i.Symbol == "AAPL").Select(i => i.Id).FirstOrDefault();
            var queryResult = await _transactionQueries.GetTransaction(appleInstrumentId);

            Assert.Equal(QueryStatus.Ok, queryResult.Status);
            AssertIsInitialAAPLTransaction(queryResult.Response);
        }

        [Fact]
        public async Task GetTransaction_ReturnsNotFound_WhenTransactionDoesNotExist()
        {
            var queryResult = await _transactionQueries.GetTransaction(-1);

            Assert.Equal(QueryStatus.NotFound, queryResult.Status);
        }

        private void AssertIsInitialAAPLTransaction(TransactionDto t)
        {
            Assert.Equal(1m, t.Amount);
            Assert.Equal(100m, t.Price);
            Assert.Equal("AAPL", t.Instrument.Symbol);
            Assert.Equal("Apple Inc.", t.Instrument.Name);
            Assert.Equal("NASDAQ", t.Instrument.Exchange);
            Assert.Equal("USD", t.Instrument.CurrencyCode);
            Assert.Equal("", t.Note);
            Assert.Equal(DateTime.UtcNow.AddDays(-2), t.Time, TimeSpan.FromHours(1));
            Assert.Equal("", t.Note);
        }

        private void AssertIsInitialBTCTransaction(TransactionDto t)
        {
            Assert.Equal(5m, t.Amount);
            Assert.Equal(5000m, t.Price);
            Assert.Equal("BTC", t.Instrument.Symbol);
            Assert.Equal("Bitcoin USD", t.Instrument.Name);
            Assert.Null(t.Instrument.Exchange);
            Assert.Equal("USD", t.Instrument.CurrencyCode);
            Assert.Equal("bitcoin", t.Instrument.Note);
            Assert.Equal(DateTime.UtcNow.AddDays(-1), t.Time, TimeSpan.FromHours(1));
            Assert.Equal("bitcoin", t.Note);
        }
    }
}
