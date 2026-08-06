namespace Wms.API.DTOs.Transactions;

public record InventoryTransactionResponseDto
{
    public int Id { get; init; }
    public int BatchId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int QuantityChange { get; init; }
    public string TransactionType { get; init; } = string.Empty;
    public int? DocumentId { get; init; }
    public string UserFullName { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public string? OldCellCode { get; init; }
    public string? NewCellCode { get; init; }
}