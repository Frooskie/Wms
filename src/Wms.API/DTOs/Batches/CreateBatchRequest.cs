namespace Wms.API.DTOs.Batches;

public class CreateBatchRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal PurchasePrice { get; set; }
    public DateTime ProductionDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int CellId { get; set; }
}