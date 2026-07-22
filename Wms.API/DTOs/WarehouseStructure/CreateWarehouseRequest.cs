namespace Wms.API.DTOs.WarehouseStructure;

public class CreateWarehouseRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? ContactPhone { get; set; }
}