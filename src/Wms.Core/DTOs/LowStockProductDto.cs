namespace Wms.Core.DTOs;

public record LowStockProductDto(
    int Id,
    string Name,
    decimal MinStockThreshold,
    decimal TotalAvailable
);