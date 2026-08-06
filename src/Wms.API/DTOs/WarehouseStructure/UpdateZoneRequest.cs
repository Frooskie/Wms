namespace Wms.API.DTOs.WarehouseStructure;

public record UpdateZoneRequest
{
    public string? Name { get; init; }
    public string? Type { get; init; }
    public int? WarehouseId { get; init; }
}