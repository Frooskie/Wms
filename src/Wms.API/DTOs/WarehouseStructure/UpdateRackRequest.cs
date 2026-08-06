namespace Wms.API.DTOs.WarehouseStructure;

public record UpdateRackRequest
{
    public string? Code { get; init; }
    public int? ZoneId { get; init; }
}