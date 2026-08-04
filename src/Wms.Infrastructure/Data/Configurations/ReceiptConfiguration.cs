using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Core.Entities;

namespace Wms.Infrastructure.Data.Configurations;

public class ReceiptConfiguration : IEntityTypeConfiguration<Receipt>
{
    public void Configure(EntityTypeBuilder<Receipt> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Supplier).IsRequired().HasMaxLength(200);
        builder.Property(r => r.Comment).HasMaxLength(500);
        builder.Property(r => r.CreatedBy).IsRequired().HasMaxLength(450);
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => r.CreatedAt);
    }
}