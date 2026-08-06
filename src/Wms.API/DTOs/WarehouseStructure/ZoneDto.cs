namespace Wms.API.DTOs.WarehouseStructure;

public record ZoneDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty; // "Normal", "Fridge", "Freezer"
    public int WarehouseId { get; init; }
}