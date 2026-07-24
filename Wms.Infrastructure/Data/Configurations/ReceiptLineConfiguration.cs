using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Core.Entities;

namespace Wms.Infrastructure.Data.Configurations;

public class ReceiptLineConfiguration : IEntityTypeConfiguration<ReceiptLine>
{
    public void Configure(EntityTypeBuilder<ReceiptLine> builder)
    {
        builder.HasKey(rl => rl.Id);
        builder.HasOne(rl => rl.Receipt)
            .WithMany(r => r.Lines)
            .HasForeignKey(rl => rl.ReceiptId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(rl => rl.Product)
            .WithMany()
            .HasForeignKey(rl => rl.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(rl => new { rl.ReceiptId, rl.ProductId }).IsUnique();
    }
}