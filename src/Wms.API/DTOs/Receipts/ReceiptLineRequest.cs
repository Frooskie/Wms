namespace Wms.API.DTOs.Receipts;

public class ReceiptLineRequest
{
    public int ProductId { get; set; }
    public int ExpectedQuantity { get; set; }
}