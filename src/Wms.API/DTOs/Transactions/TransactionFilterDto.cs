namespace Wms.API.DTOs.Transactions;

public record TransactionFilterDto
{
    public int? BatchId { get; init; }
    public int? ProductId { get; init; }
    public string? UserId { get; init; }
    public string? TransactionType { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
}