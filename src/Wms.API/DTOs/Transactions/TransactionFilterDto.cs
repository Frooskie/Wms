namespace Wms.API.DTOs.Transactions;

public record TransactionFilterDto
{
    public int? BatchId { get; init; }
    public int? ProductId { get; init; }
    public string? UserId { get; init; }

    /// <summary>Тип операции (Enum TransactionType: 0 — In, 1 — Out, 2 — Move, 3 — WriteOff).</summary>
    public string? TransactionType { get; init; }

    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
}