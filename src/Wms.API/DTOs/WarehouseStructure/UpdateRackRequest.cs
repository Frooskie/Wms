namespace Wms.API.DTOs.WarehouseStructure;

public record UpdateRackRequest
{
    /// <summary>Буквенный код стеллажа (Например А).</summary>
    public string? Code { get; init; }
    public int? ZoneId { get; init; }
}