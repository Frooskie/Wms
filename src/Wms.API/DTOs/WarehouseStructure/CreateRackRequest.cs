namespace Wms.API.DTOs.WarehouseStructure;

public class CreateRackRequest
{
    public string Code { get; set; } = string.Empty; // например, "A"
    public int ZoneId { get; set; }
}