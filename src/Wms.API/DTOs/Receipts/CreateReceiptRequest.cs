namespace Wms.API.DTOs.Receipts;

public record CreateReceiptRequest
{
    public string Supplier { get; init; } = string.Empty;

    public List<ReceiptLineRequest> Lines { get; init; } = [];
}