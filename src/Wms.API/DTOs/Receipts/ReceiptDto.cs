namespace Wms.API.DTOs.Receipts;

public record ReceiptDto
{
    public int Id { get; init; }
    public string Supplier { get; init; } = string.Empty;

    /// <summary>Примечание (например, расхождения).</summary>
    public string? Comment { get; init; }

    /// <summary>Статус (Enum ReceiptStatus: 0 — Pending, 1 — Received, 2 — Rejected).</summary>
    public string Status { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }
    public string CreatedBy { get; init; } = string.Empty;

    public List<ReceiptLineDto> Lines { get; init; } = [];
}