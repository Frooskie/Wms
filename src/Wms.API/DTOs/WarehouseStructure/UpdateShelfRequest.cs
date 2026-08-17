namespace Wms.API.DTOs.WarehouseStructure;

public record UpdateShelfRequest
{
    /// <summary>Буквенный код ячейки (Например A-1-1).</summary>
    public int? Number { get; init; }
    public int? RackId { get; init; }
}