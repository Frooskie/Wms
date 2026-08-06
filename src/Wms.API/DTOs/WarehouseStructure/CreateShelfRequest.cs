namespace Wms.API.DTOs.WarehouseStructure;

public record CreateShelfRequest
{
    public int Number { get; init; }
    public int RackId { get; init; }
}