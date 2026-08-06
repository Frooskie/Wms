namespace Wms.API.DTOs.Products;

public record UpdateProductRequest
{
    public string? Name { get; init; }
    public string? Manufacturer { get; init; }
    public string? Supplier { get; init; }
    public string? Category { get; init; }
    public string? Unit { get; init; }
    public decimal? MinStockThreshold { get; init; }
}