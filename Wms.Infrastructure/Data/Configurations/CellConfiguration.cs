using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Core.Entities;

namespace Wms.Infrastructure.Data.Configurations;

public class CellConfiguration : IEntityTypeConfiguration<Cell>
{
    public void Configure(EntityTypeBuilder<Cell> builder)
    {
        builder.HasOne(c => c.Shelf)
            .WithMany(s => s.Cells)
            .HasForeignKey(c => c.ShelfId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(c => new { c.ShelfId, c.Code })
            .IsUnique();
    }
}