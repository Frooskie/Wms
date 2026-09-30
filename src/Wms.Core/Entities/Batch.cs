namespace Wms.Core.Entities;

public class Batch
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public int ReservedQuantity { get; set; }
    public decimal PurchasePrice { get; set; }
    public DateOnly ProductionDate { get; set; }
    public DateOnly ExpiryDate { get; set; }
    public DateOnly ReceivedDate { get; set; }
    public int CellId { get; set; }

    public Product Product { get; set; } = null!;
    public Cell Cell { get; set; } = null!;
}