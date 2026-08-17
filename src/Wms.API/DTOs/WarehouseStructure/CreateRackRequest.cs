namespace Wms.API.DTOs.WarehouseStructure;

public record CreateRackRequest
{
    /// <summary>Буквенный код стеллажа (Например А).</summary>
    public string Code { get; init; } = string.Empty;

    public int ZoneId { get; init; }
}