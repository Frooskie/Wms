using Wms.Core.Enums;

namespace Wms.Core.Entities;

public class Receipt
{
    public int Id { get; set; }
    public string Supplier { get; set; } = string.Empty;
    public string? Comment { get; set; } // для расхождений
    public ReceiptStatus Status { get; set; } = ReceiptStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int CreatedByUserId { get; set; }
    public string CreatedBy { get; set; } = string.Empty; // UserId
    
    public ICollection<ReceiptLine> Lines { get; set; } = new List<ReceiptLine>();
}