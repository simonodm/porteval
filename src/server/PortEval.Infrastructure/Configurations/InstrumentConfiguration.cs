using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortEval.Domain.Models.Entities;

namespace PortEval.Infrastructure.Configurations
{
    internal class InstrumentConfiguration : IEntityTypeConfiguration<Instrument>
    {
        public void Configure(EntityTypeBuilder<Instrument> builder)
        {
            builder
                .HasKey(i => i.Id);
            builder
                .HasOne<Currency>()
                .WithMany()
                .HasForeignKey(i => i.CurrencyCode);
            builder
                .Property(i => i.Name)
                .IsRequired()
                .HasMaxLength(32);
            builder
                .Property(i => i.Symbol)
                .IsRequired()
                .HasMaxLength(10);
            builder
                .Property(i => i.Exchange)
                .IsRequired()
                .HasMaxLength(32);
            builder
                .Property(i => i.Type)
                .IsRequired();
            builder
                .OwnsOne(i => i.TrackingInfo);
            builder
                .Property(c => c.Version)
                .IsConcurrencyToken();
        }
    }
}
