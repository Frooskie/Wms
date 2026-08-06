namespace Wms.API.DTOs.WarehouseStructure;

public record WarehouseDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Address { get; init; }
    public string? ContactPhone { get; init; }
}