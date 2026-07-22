namespace Wms.API.DTOs.Receipts;

public class ReceiptDto
{
    public int Id { get; set; }
    public string Supplier { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    
    public List<ReceiptLineDto> Lines { get; set; } = [];
}