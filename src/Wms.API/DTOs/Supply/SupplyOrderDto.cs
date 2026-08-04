namespace Wms.API.DTOs.Supply;

public class SupplyOrderDto
{
    public int Id { get; set; }
    public int? SupplyRequestId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? ShippedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public List<SupplyOrderLineDto> Lines { get; set; } = new();
    public List<ReservationDto> Reservations { get; set; } = new();
}