namespace Wms.API.DTOs.Products;

public record ProductDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Manufacturer { get; init; }
    public string? Supplier { get; init; }
    public string? Category { get; init; }
    public string Unit { get; init; } = string.Empty;
    public decimal MinStockThreshold { get; init; }
}