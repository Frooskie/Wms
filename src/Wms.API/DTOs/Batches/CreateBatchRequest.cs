namespace Wms.API.DTOs.Batches;

public record CreateBatchRequest
{
    public int ProductId { get; init; }
    public int Quantity { get; init; }

    /// <summary>Закупочная цена за единицу.</summary>
    public decimal PurchasePrice { get; init; }

    public DateTime ProductionDate { get; init; }
    public DateTime ExpiryDate { get; init; }
    public int CellId { get; init; }
}