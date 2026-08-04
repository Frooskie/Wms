namespace Wms.API.DTOs.Receipts;

public class ReceiveReceiptLineRequest
{
    public int ProductId { get; set; }
    public int ActualQuantity { get; set; }
    public int CellId { get; set; }
    public DateTime ExpiryDate { get; set; }
    public decimal PurchasePrice { get; set; }
}