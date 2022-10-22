using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortEval.Domain.Models.Entities;

namespace PortEval.Infrastructure.Configurations
{
    class DataImportConfiguration : IEntityTypeConfiguration<DataImport>
    {
        public void Configure(EntityTypeBuilder<DataImport> builder)
        {
            builder
                .HasKey(i => i.Id);
            builder
                .Property(i => i.TemplateType)
                .IsRequired();
            builder
                .Property(i => i.Status)
                .IsRequired();
            builder
                .Property(i => i.StatusDetails)
                .HasMaxLength(256);
            builder
                .Property(i => i.Time)
                .IsRequired();
            builder
                .Property(i => i.ErrorLogAvailable)
                .IsRequired();
            builder
                .Property(i => i.ErrorLogPath)
                .HasMaxLength(256);
        }
    }
}
