using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortEval.Domain.Models.Entities;

namespace PortEval.Infrastructure.Configurations;

internal class CurrencyExchangeRateConfiguration : IEntityTypeConfiguration<CurrencyExchangeRate>
{
    public void Configure(EntityTypeBuilder<CurrencyExchangeRate> builder)
    {
        builder
            .HasKey(e => e.Id);
        builder
            .HasOne<Currency>()
            .WithMany()
            .HasForeignKey(er => er.CurrencyFromCode)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasOne<Currency>()
            .WithMany()
            .HasForeignKey(er => er.CurrencyToCode);
        builder
            .Property(e => e.Time)
            .IsRequired();
        builder
            .Property(e => e.ExchangeRate)
            .IsRequired()
            .HasPrecision(19, 4);
        builder
            .HasIndex(e => new { e.CurrencyFromCode, e.Time });
        builder
            .HasIndex(e => new { e.CurrencyFromCode, e.CurrencyToCode, e.Time })
            .IsUnique();
    }
}