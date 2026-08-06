namespace Wms.API.DTOs.WarehouseStructure;

public record UpdateShelfRequest
{
    public int? Number { get; init; }
    public int? RackId { get; init; }
}