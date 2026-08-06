namespace Wms.API.DTOs.WarehouseStructure;

public record CreateRackRequest
{
    public string Code { get; init; } = string.Empty; // например, "A"
    public int ZoneId { get; init; }
}