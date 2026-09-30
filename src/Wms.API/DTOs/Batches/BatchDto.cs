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
    public DateOnly ProductionDate { get; init; }
    public DateOnly ExpiryDate { get; init; }
    
    /// <summary>Дата поступления на склад.</summary>
    public DateOnly ReceivedDate { get; init; }
    public int CellId { get; init; }
    public string CellCode { get; init; } = string.Empty;
    public string ShelfNumber { get; init; } = string.Empty;
    public string RackCode { get; init; } = string.Empty;
    public string ZoneName { get; init; } = string.Empty;
}