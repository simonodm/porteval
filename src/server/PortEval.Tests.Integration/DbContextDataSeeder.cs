using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Domain.Models.ValueObjects;
using PortEval.Infrastructure;

namespace PortEval.Tests.Integration
{
    internal class DbContextDataSeeder
    {
        private readonly PortEvalDbContext _context;

        public DbContextDataSeeder(PortEvalDbContext context)
        {
            _context = context;
        }

        public async Task SeedDatabase()
        {
            
            var currencies = await SeedCurrencies();
            var exchangeRates = await SeedCurrencyExchangeRates();
            var exchanges = await SeedExchanges();
            var instruments = await SeedInstruments();
            var prices = await SeedInstrumentPrices(instruments);
            var portfolios = await SeedPortfolios();
            var positions = await SeedPositions(portfolios, instruments);
            var transactions = await SeedTransactions(positions);
            var charts = await SeedCharts(portfolios, positions, instruments);
            var dashboardItems = await SeedDashboardItems(charts);
            var dataImports = await SeedDataImports();
        }

        private async Task<List<Currency>> SeedCurrencies()
        {
            var currencies = new List<Currency>
            {
                new Currency("USD", "United States dollar", "US$", true),
                new Currency("EUR", "European Euro", "€"),
                new Currency("CZK", "Czech koruna", "Kč")
            };

            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [dbo].[Currencies]"); // removes currencies provided in migrations

            _context.Currencies.AddRange(currencies);
            await _context.CommitAsync();

            return currencies;
        }

        private async Task<List<Portfolio>> SeedPortfolios()
        {
            var portfolios = new List<Portfolio>()
            {
                new Portfolio("Portfolio 1", "Test note 1", "USD"),
                new Portfolio("Portfolio 2", "Test note 2", "EUR")
            };

            _context.Portfolios.AddRange(portfolios);
            await _context.CommitAsync();

            return portfolios;
        }

        private async Task<List<Instrument>> SeedInstruments()
        {
            var instruments = new List<Instrument>
            {
                new Instrument("Apple Inc.", "AAPL", "NASDAQ", InstrumentType.Stock, "USD", ""),
                new Instrument("Bitcoin USD", "BTC", null, InstrumentType.CryptoCurrency, "USD", "bitcoin"),
            };

            _context.Instruments.AddRange(instruments);
            await _context.CommitAsync();

            return instruments;
        }

        private async Task<List<Position>> SeedPositions(IEnumerable<Portfolio> portfolios, IEnumerable<Instrument> instruments)
        {
            var portfoliosList = portfolios.ToList();
            var instrumentsList = instruments.ToList();

            if (portfoliosList.Count < 2 || instrumentsList.Count < 2)
            {
                throw new ArgumentException("Not enough portfolios or instruments provided to seed positions.");
            }

            var positions = new List<Position>
            {
                new Position(portfoliosList[0].Id, instrumentsList[0].Id, ""),
                new Position(portfoliosList[1].Id, instrumentsList[1].Id, "bitcoin")
            };

            _context.Positions.AddRange(positions);
            await _context.CommitAsync();

            return positions;
        }

        private async Task<List<Transaction>> SeedTransactions(IEnumerable<Position> positions)
        {
            var positionsList = positions.ToList();

            if (positionsList.Count < 2)
            {
                throw new ArgumentException("Not enough positions provided to seed transactions.");
            }

            var transactions = new List<Transaction>
            {
                new Transaction(positionsList[0].Id, DateTime.UtcNow.AddDays(-2), 1m, 100m, ""),
                new Transaction(positionsList[1].Id, DateTime.UtcNow.AddDays(-1), 5m, 5000m, "bitcoin")
            };

            _context.Transactions.AddRange(transactions);
            await _context.CommitAsync();

            return transactions;
        }

        private async Task<List<Exchange>> SeedExchanges()
        {
            var exchanges = new List<Exchange>
            {
                new Exchange("NASDAQ", "NASDAQ"),
                new Exchange("NYSE", "NYSE")
            };

            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [dbo].[Exchanges]"); // removes exchanges provided in migrations

            _context.Exchanges.AddRange(exchanges);
            await _context.CommitAsync();

            return exchanges;
        }

        private async Task<List<InstrumentPrice>> SeedInstrumentPrices(IEnumerable<Instrument> instruments)
        {
            var instrumentsList = instruments.ToList();

            if (instrumentsList.Count < 2)
            {
                throw new ArgumentException("Not enough instruments provided to seed transactions.");
            }

            var prices = new List<InstrumentPrice>
            {
                new InstrumentPrice(DateTime.UtcNow.AddDays(-2), 130, instrumentsList[0].Id),
                new InstrumentPrice(DateTime.UtcNow.AddDays(-1), 140, instrumentsList[0].Id),
                new InstrumentPrice(DateTime.UtcNow, 150, instrumentsList[0].Id),
                new InstrumentPrice(DateTime.UtcNow.AddDays(-2), 4000, instrumentsList[1].Id),
                new InstrumentPrice(DateTime.UtcNow.AddDays(-1), 2000, instrumentsList[1].Id),
                new InstrumentPrice(DateTime.UtcNow.Date, 2000, instrumentsList[1].Id),
                new InstrumentPrice(DateTime.UtcNow, 1000, instrumentsList[1].Id),
            };

            _context.InstrumentPrices.AddRange(prices);
            await _context.CommitAsync();

            return prices;
        }

        private async Task<List<CurrencyExchangeRate>> SeedCurrencyExchangeRates()
        {
            var exchangeRates = new List<CurrencyExchangeRate>
            {
                new CurrencyExchangeRate(DateTime.UtcNow.AddDays(-2), 0.99m, "USD", "EUR"),
                new CurrencyExchangeRate(DateTime.UtcNow.AddDays(-1), 1m, "USD", "EUR"),
                new CurrencyExchangeRate(DateTime.UtcNow, 1.01m, "USD", "EUR"),
                new CurrencyExchangeRate(DateTime.UtcNow, 25, "USD", "CZK")
            };

            _context.CurrencyExchangeRates.AddRange(exchangeRates);
            await _context.CommitAsync();

            return exchangeRates;
        }

        private async Task<List<Chart>> SeedCharts(IEnumerable<Portfolio> portfolios, IEnumerable<Position> positions, IEnumerable<Instrument> instruments)
        {
            var portfoliosList = portfolios.ToList();
            var positionsList = positions.ToList();
            var instrumentsList = instruments.ToList();

            if (portfoliosList.Count == 0 || positionsList.Count == 0 || instrumentsList.Count == 0)
            {
                throw new ArgumentException("Too few portfolios, positions, or instruments to seed chart data.");
            }

            var chart1 = new Chart("Portfolio chart", new ChartDateRange(new ToDateRange(DateRangeUnit.MONTH, 1)),
                ChartTypeSettings.PriceChart("USD"));
            chart1.ReplaceLines(new[] { new ChartLinePortfolio(chart1.Id, 1, LineDashType.Solid, Color.Red, portfoliosList[0].Id)});

            var chart2 = new Chart("Position/instrument chart",
                new ChartDateRange(DateTime.Parse("2022-01-01"), DateTime.Parse("2022-01-15")), ChartTypeSettings.AggregatedPerformanceChart(AggregationFrequency.Day));
            chart2.ReplaceLines(new ChartLine[]
            {
                new ChartLinePosition(chart2.Id, 1, LineDashType.Dashed, Color.Blue, positionsList[0].Id),
                new ChartLineInstrument(chart2.Id, 2, LineDashType.Dotted, Color.Cyan, instrumentsList[0].Id)
            });

            _context.Charts.Add(chart1);
            _context.Charts.Add(chart2);

            await _context.CommitAsync();

            return new List<Chart> { chart1, chart2 };
        }

        private async Task<List<DashboardItem>> SeedDashboardItems(IEnumerable<Chart> charts)
        {
            var chartsList = charts.ToList();

            var dashboardItems = new List<DashboardItem>
            {
                new DashboardChartItem(chartsList[0].Id, new DashboardPosition(0, 0, 1, 1)),
                new DashboardChartItem(chartsList[1].Id, new DashboardPosition(1, 1, 2, 2))
            };

            _context.DashboardItems.AddRange(dashboardItems);
            await _context.CommitAsync();

            return dashboardItems;
        }

        private async Task<List<DataImport>> SeedDataImports()
        {
            var dataImports = new List<DataImport>
            {
                new DataImport(Guid.Parse("974c9b22-8276-4121-96ce-6bf3f0f70152"), TemplateType.Instruments, ImportStatus.Finished),
                new DataImport(Guid.Parse("4c0019c2-402f-41e8-9ddf-b3c98027e2d5"), TemplateType.Portfolios, ImportStatus.Error, "Internal error.")
            };

            _context.Imports.AddRange(dataImports);
            await _context.CommitAsync();

            return dataImports;
        }
    }
}
