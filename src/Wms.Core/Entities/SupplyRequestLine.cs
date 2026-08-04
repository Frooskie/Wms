namespace Wms.Core.Entities;

public class SupplyRequestLine
{
    public int Id { get; set; }
    public int SupplyRequestId { get; set; }
    public int ProductId { get; set; }
    public int RequestedQuantity { get; set; }

    public SupplyRequest SupplyRequest { get; set; } = null!;
    public Product Product { get; set; } = null!;
}