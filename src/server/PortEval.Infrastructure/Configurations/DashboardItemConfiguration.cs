using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortEval.Domain.Models.Entities;

namespace PortEval.Infrastructure.Configurations
{
    internal class DashboardItemConfiguration : IEntityTypeConfiguration<DashboardItem>
    {
        public void Configure(EntityTypeBuilder<DashboardItem> builder)
        {
            builder
                .HasKey(i => i.Id);
            builder
                .HasDiscriminator<string>("Item_Type")
                .HasValue<DashboardChartItem>("Chart");
            builder
                .OwnsOne(i => i.Position,
                    p =>
                    {
                        p.Property(dp => dp.X)
                            .HasColumnName("DashboardPositionX");
                        p.Property(dp => dp.Y)
                            .HasColumnName("DashboardPositionY");
                        p.Property(dp => dp.Width)
                            .HasColumnName("DashboardWidth");
                        p.Property(dp => dp.Height)
                            .HasColumnName("DashboardHeight");
                    });
        }
    }

    internal class DashboardChartItemConfiguration : IEntityTypeConfiguration<DashboardChartItem>
    {
        public void Configure(EntityTypeBuilder<DashboardChartItem> builder)
        {
            builder
                .HasBaseType<DashboardItem>();
            builder
                .HasOne<Chart>()
                .WithMany()
                .HasForeignKey(i => i.ChartId);
        }
    }
}
