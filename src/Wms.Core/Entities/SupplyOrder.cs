using Wms.Core.Enums;

namespace Wms.Core.Entities;

public class SupplyOrder
{
    public int Id { get; set; }
    public int? SupplyRequestId { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public SupplyOrderStatus Status { get; set; } = SupplyOrderStatus.Draft;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? ShippedAt { get; set; }

    public SupplyRequest? SupplyRequest { get; set; }
    public ICollection<SupplyOrderLine> Lines { get; set; } = new List<SupplyOrderLine>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}