namespace Wms.API.DTOs.Supply;

public class SupplyRequestDto
{
    public int Id { get; set; }
    public string StoreName { get; set; } = string.Empty;

    /// <summary>Статус (Enum SupplyOrderStatus: 0 — Draft, 1 — Confirmed, 2 — Shipped).</summary>
    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public List<SupplyRequestLineDto> Lines { get; set; } = [];
}