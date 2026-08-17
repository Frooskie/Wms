namespace Wms.API.DTOs.Batches;

public record UpdateBatchRequest
{
    /// <summary>Закупочная цена за единицу.</summary>
    public decimal? PurchasePrice { get; init; }

    public DateTime? ProductionDate { get; init; }
    public DateTime? ExpiryDate { get; init; }
}