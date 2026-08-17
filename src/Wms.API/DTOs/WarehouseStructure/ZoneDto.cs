namespace Wms.API.DTOs.WarehouseStructure;

public record ZoneDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;

    /// <summary>Тип зоны хранения. Обычная, холодильник, морозилка ("Normal", "Fridge", "Freezer")</summary>
    public string Type { get; init; } = string.Empty;

    public int WarehouseId { get; init; }
}