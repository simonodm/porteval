using Microsoft.EntityFrameworkCore;
using PortEval.Application.Features.Interfaces.Repositories;
using PortEval.Domain.Models.Entities;
using PortEval.Infrastructure.Configurations;
using System.Threading.Tasks;
using MediatR;

namespace PortEval.Infrastructure
{
    public class PortEvalDbContext : DbContext, IUnitOfWork
    {
        private readonly IMediator _mediator;

        public PortEvalDbContext(DbContextOptions<PortEvalDbContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new CurrencyConfiguration().Configure(modelBuilder.Entity<Currency>());
            new CurrencyExchangeRateConfiguration().Configure(modelBuilder.Entity<CurrencyExchangeRate>());
            new InstrumentConfiguration().Configure(modelBuilder.Entity<Instrument>());
            new InstrumentPriceConfiguration().Configure(modelBuilder.Entity<InstrumentPrice>());
            new InstrumentSplitConfiguration().Configure(modelBuilder.Entity<InstrumentSplit>());
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
            _mediator.DispatchDomainEventsAsync(this).Wait();
        }

        public async Task CommitAsync()
        {
            await SaveChangesAsync();
            await _mediator.DispatchDomainEventsAsync(this);
        }

        public virtual DbSet<Portfolio> Portfolios { get; set; }
        public virtual DbSet<Position> Positions { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual DbSet<CurrencyExchangeRate> CurrencyExchangeRates { get; set; }
        public virtual DbSet<Instrument> Instruments { get; set; }
        public virtual DbSet<InstrumentPrice> InstrumentPrices { get; set; }
        public virtual DbSet<InstrumentSplit> InstrumentSplits { get; set; }
        public virtual DbSet<Chart> Charts { get; set; }
        public virtual DbSet<ChartLine> ChartLines { get; set; }
        public virtual DbSet<DashboardItem> DashboardItems { get; set; }
        public virtual DbSet<DataImport> Imports { get; set; }
        public virtual DbSet<Exchange> Exchanges { get; set; }
    }
}
