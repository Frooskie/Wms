namespace Wms.API.DTOs.Receipts;

public record ReceiptDto
{
    public int Id { get; init; }
    public string Supplier { get; init; } = string.Empty;
    public string? Comment { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public string CreatedBy { get; init; } = string.Empty;

    public List<ReceiptLineDto> Lines { get; init; } = [];
}