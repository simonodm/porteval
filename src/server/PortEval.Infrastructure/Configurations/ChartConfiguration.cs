using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortEval.Domain.Models.Entities;

namespace PortEval.Infrastructure.Configurations
{
    internal class ChartConfiguration : IEntityTypeConfiguration<Chart>
    {
        public void Configure(EntityTypeBuilder<Chart> builder)
        {
            builder
                .HasKey(c => c.Id);
            builder
                .Property(c => c.Name)
                .HasMaxLength(64);
            builder
                .OwnsOne(
                    c => c.DateRange,
                    d =>
                    {
                        d.OwnsOne(dr => dr.ToDateRange,
                            tdr =>
                            {
                                tdr.Property(tdr2 => tdr2.Unit)
                                    .HasColumnName("ToDateRangeUnit");
                                tdr.Property(tdr2 => tdr2.Value)
                                    .HasColumnName("ToDateRangeValue");
                            });
                        d.Property(dr => dr.Start)
                            .HasColumnName("DateRangeStart");
                        d.Property(dr => dr.End)
                            .HasColumnName("DateRangeEnd");
                        d.Property(dr => dr.IsToDate)
                            .HasColumnName("IsToDate");
                    });
            builder
                .OwnsOne(
                    c => c.TypeConfiguration,
                    t =>
                    {
                        t.Property(typeConfig => typeConfig.Frequency)
                            .HasColumnName("Frequency");
                        t.Property(typeConfig => typeConfig.Type)
                            .HasColumnName("Type");
                        t.Property(typeConfig => typeConfig.CurrencyCode)
                            .HasColumnName("CurrencyCode");
                        t.HasOne<Currency>()
                            .WithMany()
                            .HasForeignKey(typeConfig => typeConfig.CurrencyCode);
                    });
            builder
                .Property(c => c.Version)
                .IsConcurrencyToken();
        }
    }
}
