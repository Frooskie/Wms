using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Core.Entities;

namespace Wms.Infrastructure.Data.Configurations;

public class SupplyOrderLineConfiguration : IEntityTypeConfiguration<SupplyOrderLine>
{
    public void Configure(EntityTypeBuilder<SupplyOrderLine> builder)
    {
        builder.HasKey(sol => sol.Id);
        builder.HasOne(sol => sol.SupplyOrder)
            .WithMany(so => so.Lines)
            .HasForeignKey(sol => sol.SupplyOrderId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(sol => sol.Product)
            .WithMany()
            .HasForeignKey(sol => sol.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(sol => new { sol.SupplyOrderId, sol.ProductId }).IsUnique();
    }
}