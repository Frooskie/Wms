namespace Wms.API.DTOs.WarehouseStructure;

public record CreateZoneRequest
{
    public string Name { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty; // "Normal", "Fridge", "Freezer"
    public int WarehouseId { get; init; }
}