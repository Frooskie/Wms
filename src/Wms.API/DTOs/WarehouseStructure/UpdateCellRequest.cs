namespace Wms.API.DTOs.WarehouseStructure;

public record UpdateCellRequest
{
    /// <summary>Буквенный код ячейки (Например A-1-1).</summary>
    public string? Code { get; init; }

    public int? ShelfId { get; init; }
}