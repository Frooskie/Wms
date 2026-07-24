namespace Wms.Core.Entities;

public class SupplyOrderLine
{
    public int Id { get; set; }
    public int SupplyOrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }

    public SupplyOrder SupplyOrder { get; set; } = null!;
    public Product Product { get; set; } = null!;
}