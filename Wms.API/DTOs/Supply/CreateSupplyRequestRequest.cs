namespace Wms.API.DTOs.Supply;

public class CreateSupplyRequestRequest
{
    public string StoreName { get; set; } = string.Empty;
    public List<SupplyRequestLineRequest> Lines { get; set; } = new();
}