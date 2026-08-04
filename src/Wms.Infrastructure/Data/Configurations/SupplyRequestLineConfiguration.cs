using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Core.Entities;

namespace Wms.Infrastructure.Data.Configurations;

public class SupplyRequestLineConfiguration : IEntityTypeConfiguration<SupplyRequestLine>
{
    public void Configure(EntityTypeBuilder<SupplyRequestLine> builder)
    {
        builder.HasKey(srl => srl.Id);
        builder.HasOne(srl => srl.SupplyRequest)
            .WithMany(sr => sr.Lines)
            .HasForeignKey(srl => srl.SupplyRequestId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(srl => srl.Product)
            .WithMany()
            .HasForeignKey(srl => srl.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(srl => new { srl.SupplyRequestId, srl.ProductId }).IsUnique();
    }
}