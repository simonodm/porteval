using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortEval.Domain.Models.Entities;

namespace PortEval.Infrastructure.Configurations
{
    internal class PositionConfiguration : IEntityTypeConfiguration<Position>
    {
        public void Configure(EntityTypeBuilder<Position> builder)
        {
            builder
                .HasKey(p => p.Id);
            builder
                .HasOne<Portfolio>()
                .WithMany()
                .HasForeignKey(p => p.PortfolioId);
            builder
                .HasOne<Instrument>()
                .WithMany()
                .HasForeignKey(p => p.InstrumentId);
            builder
                .Property(c => c.Version)
                .IsConcurrencyToken();
        }
    }
}
