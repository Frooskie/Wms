using Wms.Core.Entities;
using Wms.Core.Enums;

namespace Wms.Core.Interfaces.Services;

public interface IInventoryTransactionService
{
    Task<IEnumerable<InventoryTransaction>> GetFilteredAsync(
        int? batchId = null,
        int? productId = null,
        string? userId = null,
        TransactionType? transactionType = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default);
    
    Task AddTransactionAsync(
        int batchId,
        int quantityChange,
        TransactionType transactionType,
        string userId,
        int? documentId = null,
        int? oldCellId = null,
        int? newCellId = null,
        CancellationToken cancellationToken = default);
}