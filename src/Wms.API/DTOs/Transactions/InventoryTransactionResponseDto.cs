using Wms.Core.Enums;

namespace Wms.API.DTOs.Transactions;

public record InventoryTransactionResponseDto
{
    public int Id { get; init; }
    public int BatchId { get; init; }
    public string ProductName { get; init; } = string.Empty;

    /// <summary>Изменение количества (положительное — приход, отрицательное — расход).</summary>
    public int QuantityChange { get; init; }

    /// <summary>Тип операции.</summary>
    public TransactionType TransactionType { get; init; }

    /// <summary>Идентификатор документа-инициатора (Receipts.Id для прихода, SupplyOrders.Id для отгрузки).</summary>
    public int? DocumentId { get; init; }

    public string UserFullName { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public string? OldCellCode { get; init; }
    public string? NewCellCode { get; init; }
}