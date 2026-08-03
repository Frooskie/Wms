using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Core.Entities;

namespace Wms.Infrastructure.Data.Configurations;

public class RackConfiguration : IEntityTypeConfiguration<Rack>
{
    public void Configure(EntityTypeBuilder<Rack> builder)
    {
        builder.HasOne(r => r.Zone)
            .WithMany(z => z.Racks)
            .HasForeignKey(r => r.ZoneId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => new { r.ZoneId, r.Code })
            .IsUnique();
    }
}