using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortEval.Domain.Models.Entities;

namespace PortEval.Infrastructure.Configurations
{
    internal class InstrumentPriceConfiguration : IEntityTypeConfiguration<InstrumentPrice>
    {
        public void Configure(EntityTypeBuilder<InstrumentPrice> builder)
        {
            builder
                .HasKey(p => p.Id);
            builder
                .HasIndex(p => new { p.InstrumentId, p.Time });
            builder
                .HasOne(p => p.Instrument)
                .WithMany(i => i.Prices)
                .HasForeignKey(p => p.InstrumentId);
            builder
                .Property(p => p.Time)
                .IsRequired();
            builder
                .Property(p => p.Price)
                .IsRequired()
                .HasPrecision(19, 4);
        }
    }
}
