namespace Wms.API.DTOs.Transactions;

public record TransactionFilterDto(
    int? BatchId,
    int? ProductId,
    string? UserId,
    string? TransactionType,
    DateTime? FromDate,
    DateTime? ToDate
);