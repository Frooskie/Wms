namespace Wms.API.DTOs.WarehouseStructure;

public record CreateCellRequest
{
    /// <summary>Номер полки.</summary>
    public string Code { get; init; } = string.Empty;

    public int ShelfId { get; init; }
}