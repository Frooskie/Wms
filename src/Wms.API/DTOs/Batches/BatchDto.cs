namespace Wms.API.DTOs.Batches;

public record BatchDto
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }

    /// <summary>Зарезервированное количество под заказы.</summary>
    public int ReservedQuantity { get; init; }

    public int AvailableQuantity => Quantity - ReservedQuantity;
    
    /// <summary>Закупочная цена за единицу.</summary>
    public decimal PurchasePrice { get; init; }
    public DateTime ProductionDate { get; init; }
    public DateTime ExpiryDate { get; init; }
    
    /// <summary>Дата поступления на склад.</summary>
    public DateTime ReceivedDate { get; init; }
    public int CellId { get; init; }
    public string CellCode { get; init; } = string.Empty;
    public string ShelfNumber { get; init; } = string.Empty;
    public string RackCode { get; init; } = string.Empty;
    public string ZoneName { get; init; } = string.Empty;
}