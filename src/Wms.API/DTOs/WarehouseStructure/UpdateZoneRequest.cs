namespace Wms.API.DTOs.WarehouseStructure;

public record UpdateZoneRequest
{
    public string? Name { get; init; }

    /// <summary>Тип зоны хранения. Обычная, холодильник, морозилка ("Normal", "Fridge", "Freezer")</summary>
    public string? Type { get; init; }

    public int? WarehouseId { get; init; }
}