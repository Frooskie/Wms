namespace Wms.API.DTOs.Supply;

public class ReservationDto
{
    public int Id { get; set; }
    public int BatchId { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
}