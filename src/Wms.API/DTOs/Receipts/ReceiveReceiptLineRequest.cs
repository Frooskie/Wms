namespace Wms.API.DTOs.Receipts;

public record ReceiveReceiptLineRequest
{
    public int ProductId { get; init; }
    public int ActualQuantity { get; init; }
    public int CellId { get; init; }
    public DateTime ExpiryDate { get; init; }
    public decimal PurchasePrice { get; init; }
}