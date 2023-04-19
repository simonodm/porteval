using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortEval.Domain.Models.Entities;

namespace PortEval.Infrastructure.Configurations;

internal class InstrumentSplitConfiguration : IEntityTypeConfiguration<InstrumentSplit>
{
    public void Configure(EntityTypeBuilder<InstrumentSplit> builder)
    {
        builder
            .HasKey(s => s.Id);
        builder
            .Property(s => s.Time)
            .IsRequired();
        builder
            .Property(s => s.ProcessingStatus)
            .IsRequired();
        builder
            .HasOne<Instrument>()
            .WithMany()
            .HasForeignKey(s => s.InstrumentId);
        builder
            .OwnsOne(s => s.SplitRatio,
                r =>
                {
                    r.Property(ratio => ratio.Denominator).HasColumnName("SplitRatioDenominator").IsRequired();
                    r.Property(ratio => ratio.Numerator).HasColumnName("SplitRatioNumerator").IsRequired();
                })
            .Navigation(s => s.SplitRatio).IsRequired();
        builder
            .Property(s => s.Version)
            .IsConcurrencyToken();
        builder
            .HasIndex(s => new { s.InstrumentId, s.Time })
            .IsUnique();
    }
}