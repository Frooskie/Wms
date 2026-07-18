namespace Wms.API.DTOs;

public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Manufacturer { get; set; }
    public string? Supplier { get; set; }
    public string? Category { get; set; }
    public string Unit { get; set; } = "шт";
    public decimal MinStockThreshold { get; set; }
}