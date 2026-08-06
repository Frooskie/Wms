using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Core.Entities;

namespace Wms.Infrastructure.Data.Configurations;

public class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
{
    public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
    {
        builder.ToTable("InventoryTransactions");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.QuantityChange)
            .IsRequired();

        builder.Property(e => e.TransactionType)
            .IsRequired();

        builder.Property(e => e.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(e => e.Timestamp)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(e => e.BatchId);
        builder.HasIndex(e => e.TransactionType);
        builder.HasIndex(e => e.Timestamp);
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.DocumentId);

        builder.HasOne(e => e.Batch)
            .WithMany()
            .HasForeignKey(e => e.BatchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.OldCell)
            .WithMany()
            .HasForeignKey(e => e.OldCellId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.NewCell)
            .WithMany()
            .HasForeignKey(e => e.NewCellId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}