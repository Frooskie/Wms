namespace Wms.Core.Entities;

public class ReceiptLine
{
    public int Id { get; set; }
    public int ReceiptId { get; set; }
    public int ProductId { get; set; }
    public int ExpectedQuantity { get; set; }

    public Receipt Receipt { get; set; } = null!;
    public Product Product { get; set; } = null!;
}