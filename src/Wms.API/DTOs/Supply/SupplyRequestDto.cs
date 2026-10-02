using Wms.Core.Enums;

namespace Wms.API.DTOs.Supply;

public class SupplyRequestDto
{
    public int Id { get; set; }
    public string StoreName { get; set; } = string.Empty;

    /// <summary>Статус заявки магазина.</summary>
    public SupplyRequestStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public List<SupplyRequestLineDto> Lines { get; set; } = [];
}