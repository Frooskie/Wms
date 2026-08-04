namespace Wms.API.DTOs.WarehouseStructure;

public class CreateZoneRequest
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "Normal", "Fridge", "Freezer"
    public int WarehouseId { get; set; }
}