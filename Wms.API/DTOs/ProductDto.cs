namespace Wms.API.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Manufacturer { get; set; }
    public string? Supplier { get; set; }
    public string? Category { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal MinStockThreshold { get; set; }
}