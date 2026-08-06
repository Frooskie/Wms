namespace Wms.API.DTOs.WarehouseStructure;

public record CellDto
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public int ShelfId { get; init; }
    public bool IsOccupied { get; init; }
}