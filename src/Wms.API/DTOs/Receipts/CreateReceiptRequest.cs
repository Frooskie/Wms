namespace Wms.API.DTOs.Receipts;

public class CreateReceiptRequest
{
    public string Supplier { get; set; } = string.Empty;

    public List<ReceiptLineRequest> Lines { get; set; } = [];
}