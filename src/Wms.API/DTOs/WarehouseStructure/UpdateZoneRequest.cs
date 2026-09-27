using Wms.Core.Enums;

namespace Wms.API.DTOs.WarehouseStructure;

public record UpdateZoneRequest
{
    public string? Name { get; init; }

    /// <summary>Тип зоны хранения.</summary>
    public ZoneType? Type { get; init; }

    public int? WarehouseId { get; init; }
}