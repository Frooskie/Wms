namespace Wms.API.DTOs.Transactions;

public record InventoryTransactionResponseDto(
    int Id,
    int BatchId,
    string ProductName,
    int QuantityChange,
    string TransactionType,
    int? DocumentId,
    string UserFullName,
    DateTime Timestamp,
    string? OldCellCode,
    string? NewCellCode
);