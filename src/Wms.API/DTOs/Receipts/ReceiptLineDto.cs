namespace Wms.API.DTOs.Receipts;

public record ReceiptLineDto
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int ExpectedQuantity { get; init; }
}