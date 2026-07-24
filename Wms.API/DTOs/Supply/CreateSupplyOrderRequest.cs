namespace Wms.API.DTOs.Supply;

public class CreateSupplyOrderRequest
{
    public int? SupplyRequestId { get; set; }
    public List<SupplyOrderLineRequest> Lines { get; set; } = [];
}