using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Wms.Core.Entities;
using Wms.Infrastructure.Data.Configurations;

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
    public DbSet<Batch> Batches { get; set; }
    public DbSet<Receipt> Receipts { get; set; }
    public DbSet<ReceiptLine> ReceiptLines { get; set; }
    public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
    public DbSet<SupplyRequest> SupplyRequests { get; set; }
    public DbSet<SupplyRequestLine> SupplyRequestLines { get; set; }
    public DbSet<SupplyOrder> SupplyOrders { get; set; }
    public DbSet<SupplyOrderLine> SupplyOrderLines { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ZoneConfiguration());
        modelBuilder.ApplyConfiguration(new RackConfiguration());
        modelBuilder.ApplyConfiguration(new ShelfConfiguration());
        modelBuilder.ApplyConfiguration(new CellConfiguration());
        modelBuilder.ApplyConfiguration(new BatchConfiguration());
        modelBuilder.ApplyConfiguration(new ReceiptConfiguration());
        modelBuilder.ApplyConfiguration(new ReceiptLineConfiguration());
        modelBuilder.ApplyConfiguration(new InventoryTransactionConfiguration());
        modelBuilder.ApplyConfiguration(new SupplyRequestConfiguration());
        modelBuilder.ApplyConfiguration(new SupplyRequestLineConfiguration());
        modelBuilder.ApplyConfiguration(new SupplyOrderConfiguration());
        modelBuilder.ApplyConfiguration(new SupplyOrderLineConfiguration());
        modelBuilder.ApplyConfiguration(new ReservationConfiguration());
        modelBuilder.ApplyConfiguration(new NotificationConfiguration());
    }
}