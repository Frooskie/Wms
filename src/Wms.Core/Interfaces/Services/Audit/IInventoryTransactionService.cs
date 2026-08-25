using Wms.Core.DTOs.Common;
using Wms.Core.Entities;
using Wms.Core.Enums;

namespace Wms.Core.Interfaces.Services.Audit;

public interface IInventoryTransactionService
{
    Task<PagedResult<InventoryTransaction>> GetPagedFilteredAsync(
        int? batchId = null,
        int? productId = null,
        string? userId = null,
        TransactionType? transactionType = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int pageNumber = 1,
        int pageSize = 10,
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