using Wms.Core.Enums;

namespace Wms.Core.Entities;

public class InventoryTransaction
{
    public int Id { get; set; }
    public int BatchId { get; set; }
    public int QuantityChange { get; set; } // может быть отрицательным для расхода
    public TransactionType TransactionType { get; set; }
    public int? DocumentId { get; set; } // ссылка на ReceiptId или SupplyOrderId
    public string UserId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public int? OldCellId { get; set; }
    public int? NewCellId { get; set; }

    public Batch Batch { get; set; } = null!;
}