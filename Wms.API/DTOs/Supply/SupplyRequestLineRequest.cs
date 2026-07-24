namespace Wms.API.DTOs.Supply;

public class SupplyRequestLineRequest
{
    public int ProductId { get; set; }
    public int RequestedQuantity { get; set; }
}