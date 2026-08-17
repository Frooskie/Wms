namespace Wms.API.DTOs.WarehouseStructure;

public record CellDto
{
    public int Id { get; init; }

    /// <summary>Буквенный код ячейки (Например A-1-1).</summary>
    public string Code { get; init; } = string.Empty;

    public int ShelfId { get; init; }
    public bool IsOccupied { get; init; }
}