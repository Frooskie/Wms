using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Core.Entities;

namespace Wms.Infrastructure.Data.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasKey(r => r.Id);
        builder.HasOne(r => r.Batch)
            .WithMany()
            .HasForeignKey(r => r.BatchId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(r => r.SupplyOrder)
            .WithMany(so => so.Reservations)
            .HasForeignKey(r => r.SupplyOrderId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(r => new { r.BatchId, r.SupplyOrderId }).IsUnique();
        builder.HasIndex(r => r.CreatedAt);
    }
}