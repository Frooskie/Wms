using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Core.Entities;

namespace Wms.Infrastructure.Data.Configurations;

public class BatchConfiguration : IEntityTypeConfiguration<Batch>
{
    public void Configure(EntityTypeBuilder<Batch> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Quantity).IsRequired();
        builder.Property(b => b.ReservedQuantity).HasDefaultValue(0);
        builder.Property(b => b.PurchasePrice).HasPrecision(18, 2);

        builder.HasOne(b => b.Product)
            .WithMany()
            .HasForeignKey(b => b.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Cell)
            .WithMany()
            .HasForeignKey(b => b.CellId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(b => b.ExpiryDate);
        builder.HasIndex(b => b.ProductId);
        builder.HasIndex(b => b.CellId);
        
        builder.ToTable(t =>
        {
            t.HasCheckConstraint(
                "CK_Batches_ExpiryAfterProduction",
                "\"ExpiryDate\" > \"ProductionDate\"");

            t.HasCheckConstraint(
                "CK_Batches_ReservedNotExceedQuantity",
                "\"ReservedQuantity\" <= \"Quantity\"");
        });
    }
}