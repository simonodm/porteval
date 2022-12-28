using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortEval.Domain.Models.Entities;

namespace PortEval.Infrastructure.Configurations
{
    internal class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder
                .HasKey(t => t.Id);
            builder
                .HasIndex(t => new { t.PositionId, t.Time });
            builder
                .HasOne(t => t.Position)
                .WithMany(p => p.Transactions)
                .HasForeignKey(t => t.PositionId);
            builder
                .Property(t => t.Time)
                .IsRequired();
            builder
                .Property(t => t.Amount)
                .IsRequired()
                .HasPrecision(19, 10);
            builder
                .Property(t => t.Price)
                .IsRequired()
                .HasPrecision(19, 4);
            builder
                .Property(t => t.Note)
                .HasMaxLength(255);
        }
    }
}
