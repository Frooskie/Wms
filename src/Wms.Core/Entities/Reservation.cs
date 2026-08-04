namespace Wms.Core.Entities;

public class Reservation
{
    public int Id { get; set; }
    public int BatchId { get; set; }
    public int SupplyOrderId { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Batch Batch { get; set; } = null!;
    public SupplyOrder SupplyOrder { get; set; } = null!;
}