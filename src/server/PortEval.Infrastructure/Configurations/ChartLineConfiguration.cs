using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortEval.Domain.Models.Entities;
using System.Drawing;

namespace PortEval.Infrastructure.Configurations
{
    internal class ChartLineConfiguration : IEntityTypeConfiguration<ChartLine>
    {
        public void Configure(EntityTypeBuilder<ChartLine> builder)
        {
            builder
                .HasKey(line => line.Id);
            builder
                .HasOne(line => line.Chart)
                .WithMany(c => c.Lines);
            builder
                .HasDiscriminator<string>("Line_Type")
                .HasValue<ChartLinePortfolio>("Portfolio")
                .HasValue<ChartLinePosition>("Position")
                .HasValue<ChartLineInstrument>("Instrument");
            builder
                .Property(line => line.Color)
                .HasConversion(
                    c => c.ToArgb(),
                    c => Color.FromArgb(c)
                );
        }
    }
    internal class ChartLinePortfolioConfiguration : IEntityTypeConfiguration<ChartLinePortfolio>
    {
        public void Configure(EntityTypeBuilder<ChartLinePortfolio> builder)
        {
            builder
                .HasOne<Portfolio>()
                .WithMany()
                .HasForeignKey(line => line.PortfolioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
    internal class ChartLinePositionConfiguration : IEntityTypeConfiguration<ChartLinePosition>
    {
        public void Configure(EntityTypeBuilder<ChartLinePosition> builder)
        {
            builder
                .HasOne<Position>()
                .WithMany()
                .HasForeignKey(line => line.PositionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
    internal class ChartLineInstrumentConfiguration : IEntityTypeConfiguration<ChartLineInstrument>
    {
        public void Configure(EntityTypeBuilder<ChartLineInstrument> builder)
        {
            builder
                .HasOne<Instrument>()
                .WithMany()
                .HasForeignKey(line => line.InstrumentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
