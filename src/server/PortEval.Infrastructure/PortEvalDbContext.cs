using Microsoft.EntityFrameworkCore;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;
using PortEval.Infrastructure.Configurations;
using System.Threading.Tasks;

namespace PortEval.Infrastructure
{
    public class PortEvalDbContext : DbContext, IUnitOfWork
    {
        public PortEvalDbContext(DbContextOptions<PortEvalDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new CurrencyConfiguration().Configure(modelBuilder.Entity<Currency>());
            new CurrencyExchangeRateConfiguration().Configure(modelBuilder.Entity<CurrencyExchangeRate>());
            new InstrumentConfiguration().Configure(modelBuilder.Entity<Instrument>());
            new InstrumentPriceConfiguration().Configure(modelBuilder.Entity<InstrumentPrice>());
            new PortfolioConfiguration().Configure(modelBuilder.Entity<Portfolio>());
            new PositionConfiguration().Configure(modelBuilder.Entity<Position>());
            new TransactionConfiguration().Configure(modelBuilder.Entity<Transaction>());
            new ChartConfiguration().Configure(modelBuilder.Entity<Chart>());
            new ChartLineConfiguration().Configure(modelBuilder.Entity<ChartLine>());
            new ChartLinePortfolioConfiguration().Configure(modelBuilder.Entity<ChartLinePortfolio>());
            new ChartLinePositionConfiguration().Configure(modelBuilder.Entity<ChartLinePosition>());
            new ChartLineInstrumentConfiguration().Configure(modelBuilder.Entity<ChartLineInstrument>());
            new DashboardItemConfiguration().Configure(modelBuilder.Entity<DashboardItem>());
            new DashboardChartItemConfiguration().Configure(modelBuilder.Entity<DashboardChartItem>());
            new DataImportConfiguration().Configure(modelBuilder.Entity<DataImport>());
            new ExchangeConfiguration().Configure(modelBuilder.Entity<Exchange>());
        }

        public void Commit()
        {
            SaveChanges();
        }

        public async Task CommitAsync()
        {
            await SaveChangesAsync();
        }

        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<CurrencyExchangeRate> CurrencyExchangeRates { get; set; }
        public DbSet<Instrument> Instruments { get; set; }
        public DbSet<InstrumentPrice> InstrumentPrices { get; set; }
        public DbSet<Chart> Charts { get; set; }
        public DbSet<ChartLine> ChartLines { get; set; }
        public DbSet<DashboardItem> DashboardItems { get; set; }
        public DbSet<DataImport> Imports { get; set; }
        public DbSet<Exchange> Exchanges { get; set; }
    }
}
