namespace Wms.API.DTOs;

public class UpdateZoneRequest
{
    public string? Name { get; set; }
    public string? Type { get; set; }
    public int? WarehouseId { get; set; } // если разрешено менять привязку к складу
}