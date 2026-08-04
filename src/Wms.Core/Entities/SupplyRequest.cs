using Wms.Core.Enums;

namespace Wms.Core.Entities;

public class SupplyRequest
{
    public int Id { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public SupplyRequestStatus Status { get; set; } = SupplyRequestStatus.Draft;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? RejectedAt { get; set; }

    public ICollection<SupplyRequestLine> Lines { get; set; } = new List<SupplyRequestLine>();
}