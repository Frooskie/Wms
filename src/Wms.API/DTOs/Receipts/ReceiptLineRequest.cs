namespace Wms.API.DTOs.Receipts;

public record ReceiptLineRequest
{
    public int ProductId { get; init; }
    public int ExpectedQuantity { get; init; }
}