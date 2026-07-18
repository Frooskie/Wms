using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Wms.Core.Entities;

namespace Wms.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<Zone> Zones { get; set; }
    public DbSet<Rack> Racks { get; set; }
    public DbSet<Shelf> Shelves { get; set; }
    public DbSet<Cell> Cells { get; set; }
    public DbSet<Product> Products { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Настройка связей и ограничений
        modelBuilder.Entity<Zone>()
            .HasOne(z => z.Warehouse)
            .WithMany(w => w.Zones)
            .HasForeignKey(z => z.WarehouseId)
            .OnDelete(DeleteBehavior.Cascade); // при удалении склада удаляются зоны

        modelBuilder.Entity<Rack>()
            .HasOne(r => r.Zone)
            .WithMany(z => z.Racks)
            .HasForeignKey(r => r.ZoneId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Shelf>()
            .HasOne(s => s.Rack)
            .WithMany(r => r.Shelves)
            .HasForeignKey(s => s.RackId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Cell>()
            .HasOne(c => c.Shelf)
            .WithMany(s => s.Cells)
            .HasForeignKey(c => c.ShelfId)
            .OnDelete(DeleteBehavior.Cascade);

        // Уникальные индексы для предотвращения дублирования
        modelBuilder.Entity<Rack>()
            .HasIndex(r => new { r.ZoneId, r.Code })
            .IsUnique();

        modelBuilder.Entity<Shelf>()
            .HasIndex(s => new { s.RackId, s.Number })
            .IsUnique();

        modelBuilder.Entity<Cell>()
            .HasIndex(c => new { c.ShelfId, c.Code })
            .IsUnique();
    }
}