using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Core.Entities;

namespace Wms.Infrastructure.Data.Configurations;

public class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
{
    public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
    {
        builder.HasKey(it => it.Id);
        builder.HasOne(it => it.Batch)
            .WithMany()
            .HasForeignKey(it => it.BatchId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Property(it => it.QuantityChange).IsRequired();
        builder.Property(it => it.UserId).IsRequired().HasMaxLength(450);
        builder.HasIndex(it => it.BatchId);
        builder.HasIndex(it => it.Timestamp);
        builder.HasIndex(it => it.TransactionType);
    }
}