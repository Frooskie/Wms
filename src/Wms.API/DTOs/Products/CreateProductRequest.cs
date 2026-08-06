namespace Wms.API.DTOs.Products;

public record CreateProductRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Manufacturer { get; init; }
    public string? Supplier { get; init; }
    public string? Category { get; init; }
    public string Unit { get; init; } = "шт";
    public decimal MinStockThreshold { get; init; }
}