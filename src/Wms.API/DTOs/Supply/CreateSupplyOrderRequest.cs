namespace Wms.API.DTOs.Supply;

public class CreateSupplyOrderRequest
{
    /// <summary>Ссылка на заявку (SupplyRequests.Id), если заказ создан на основе заявки.</summary>
    public int? SupplyRequestId { get; set; }

    public List<SupplyOrderLineRequest> Lines { get; set; } = [];
}