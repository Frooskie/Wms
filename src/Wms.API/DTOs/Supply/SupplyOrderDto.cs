namespace Wms.API.DTOs.Supply;

public class SupplyOrderDto
{
    public int Id { get; set; }
    public int? SupplyRequestId { get; set; }
    
    /// <summary>Статус (Enum SupplyOrderStatus: 0 — Draft, 1 — Confirmed, 2 — Shipped).</summary>
    public string Status { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    /// <summary>Дата подтверждения (резервирование выполнено).</summary>
    public DateTime? ConfirmedAt { get; set; }

    /// <summary>Дата отгрузки (списание выполнено).</summary>
    public DateTime? ShippedAt { get; set; }

    public List<SupplyOrderLineDto> Lines { get; set; } = new();
    public List<ReservationDto> Reservations { get; set; } = new();
}