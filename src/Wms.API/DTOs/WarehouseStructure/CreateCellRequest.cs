namespace Wms.API.DTOs.WarehouseStructure;

public record CreateCellRequest
{
    public string Code { get; init; } = string.Empty; // например, "A-1-1"
    public int ShelfId { get; init; }
}