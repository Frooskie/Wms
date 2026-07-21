namespace Wms.API.DTOs;

public class UpdateBatchRequest
{
    public decimal? PurchasePrice { get; set; }
    public DateTime? ProductionDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
}