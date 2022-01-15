using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortEval.Domain.Models.Entities;

namespace PortEval.Infrastructure.Configurations
{
    internal class PortfolioConfiguration : IEntityTypeConfiguration<Portfolio>
    {
        public void Configure(EntityTypeBuilder<Portfolio> builder)
        {
            builder
                .HasKey(p => p.Id);
            builder
                .HasOne<Currency>()
                .WithMany()
                .HasForeignKey(p => p.CurrencyCode);
            builder
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(64);
            builder
                .Property(p => p.Note)
                .HasMaxLength(255);
            builder
                .Property(c => c.Version)
                .IsConcurrencyToken();
        }
    }
}
