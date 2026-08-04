namespace Wms.API.DTOs.Batches;

public class BatchDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int ReservedQuantity { get; set; }
    public int AvailableQuantity => Quantity - ReservedQuantity;
    public decimal PurchasePrice { get; set; }
    public DateTime ProductionDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime ReceivedDate { get; set; }
    public int CellId { get; set; }
    public string CellCode { get; set; } = string.Empty;
    public string ShelfNumber { get; set; } = string.Empty;
    public string RackCode { get; set; } = string.Empty;
    public string ZoneName { get; set; } = string.Empty;
}