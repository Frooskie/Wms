using Wms.Core.Enums;

namespace Wms.API.DTOs.WarehouseStructure;

public record ZoneDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;

    /// <summary>Тип зоны хранения.</summary>
    public ZoneType Type { get; init; }

    public int WarehouseId { get; init; }
}