namespace Wms.API.DTOs.Receipts;

public class ReceiveReceiptRequest
{
    public List<ReceiveReceiptLineRequest> Lines { get; set; } = [];
}