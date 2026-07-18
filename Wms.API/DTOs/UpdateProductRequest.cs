namespace Wms.API.DTOs;

public class UpdateProductRequest
{
    public string? Name { get; set; }
    public string? Manufacturer { get; set; }
    public string? Supplier { get; set; }
    public string? Category { get; set; }
    public string? Unit { get; set; }
    public decimal? MinStockThreshold { get; set; }
}