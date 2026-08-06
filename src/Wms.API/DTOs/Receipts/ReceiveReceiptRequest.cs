namespace Wms.API.DTOs.Receipts;

public record ReceiveReceiptRequest
{
    public List<ReceiveReceiptLineRequest> Lines { get; init; } = [];
}