namespace Wms.API.DTOs.Batches;

public record UpdateBatchRequest
{
    /// <summary>Закупочная цена за единицу.</summary>
    public decimal? PurchasePrice { get; init; }

    public DateOnly? ProductionDate { get; init; }
    public DateOnly? ExpiryDate { get; init; }
}