namespace Wms.API.DTOs.Receipts;

public class ReceiptLineDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int ExpectedQuantity { get; set; }
}