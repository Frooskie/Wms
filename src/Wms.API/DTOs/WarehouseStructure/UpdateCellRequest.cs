namespace Wms.API.DTOs.WarehouseStructure;

public record UpdateCellRequest
{
    public string? Code { get; init; }
    public int? ShelfId { get; init; }
}