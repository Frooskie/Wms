namespace Wms.Core.Entities;

public class Batch
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public int ReservedQuantity { get; set; }
    public decimal PurchasePrice { get; set; }
    public DateTime ProductionDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime ReceivedDate { get; set; }
    public int CellId { get; set; }
    
    public Product Product { get; set; } = null!;
    public Cell Cell { get; set; } = null!;
}