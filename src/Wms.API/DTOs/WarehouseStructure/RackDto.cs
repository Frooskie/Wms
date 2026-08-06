namespace Wms.API.DTOs.WarehouseStructure;

public record RackDto
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public int ZoneId { get; init; }
    public List<ShelfDto> Shelves { get; init; } = [];
}