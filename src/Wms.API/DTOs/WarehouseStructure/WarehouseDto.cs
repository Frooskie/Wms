namespace Wms.API.DTOs.WarehouseStructure;

public class WarehouseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? ContactPhone { get; set; }
}