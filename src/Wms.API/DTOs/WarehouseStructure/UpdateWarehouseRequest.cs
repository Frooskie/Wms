namespace Wms.API.DTOs.WarehouseStructure;

public record UpdateWarehouseRequest
{
    public string? Name { get; init; }
    public string? Address { get; init; }
    public string? ContactPhone { get; init; }
}