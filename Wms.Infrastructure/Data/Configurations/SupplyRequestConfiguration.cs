using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Core.Entities;

namespace Wms.Infrastructure.Data.Configurations;

public class SupplyRequestConfiguration : IEntityTypeConfiguration<SupplyRequest>
{
    public void Configure(EntityTypeBuilder<SupplyRequest> builder)
    {
        builder.HasKey(sr => sr.Id);
        builder.Property(sr => sr.StoreName).IsRequired().HasMaxLength(200);
        builder.Property(sr => sr.CreatedBy).IsRequired().HasMaxLength(450);
        builder.HasIndex(sr => sr.Status);
        builder.HasIndex(sr => sr.CreatedAt);
    }
}