using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Core.Entities;

namespace Wms.Infrastructure.Data.Configurations;

public class SupplyOrderConfiguration : IEntityTypeConfiguration<SupplyOrder>
{
    public void Configure(EntityTypeBuilder<SupplyOrder> builder)
    {
        builder.HasKey(so => so.Id);
        builder.Property(so => so.CreatedBy).IsRequired().HasMaxLength(450);
        builder.HasOne(so => so.SupplyRequest)
            .WithMany()
            .HasForeignKey(so => so.SupplyRequestId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasIndex(so => so.Status);
        builder.HasIndex(so => so.CreatedAt);
    }
}