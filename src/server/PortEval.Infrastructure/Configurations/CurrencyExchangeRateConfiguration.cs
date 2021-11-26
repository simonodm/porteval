using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortEval.Domain.Models.Entities;

namespace PortEval.Infrastructure.Configurations
{
    internal class CurrencyExchangeRateConfiguration : IEntityTypeConfiguration<CurrencyExchangeRate>
    {
        public void Configure(EntityTypeBuilder<CurrencyExchangeRate> builder)
        {
            builder
                .HasKey(e => e.Id);
            builder
                .HasIndex(e => new { e.CurrencyFromCode, e.Time });
            builder
                .HasOne(e => e.CurrencyFrom)
                .WithMany(c => c.ExchangeRates)
                .OnDelete(DeleteBehavior.Cascade);
            builder
                .HasOne(e => e.CurrencyTo)
                .WithMany();
            builder
                .Property(e => e.Time)
                .IsRequired();
            builder
                .Property(e => e.ExchangeRate)
                .IsRequired()
                .HasPrecision(19, 4);
        }
    }
}
