namespace Wms.API.DTOs.WarehouseStructure;

public class UpdateZoneRequest
{
    public string? Name { get; set; }
    public string? Type { get; set; }
    public int? WarehouseId { get; set; }
}