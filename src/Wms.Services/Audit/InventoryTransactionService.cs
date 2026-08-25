using Wms.Core.DTOs.Common;
using Wms.Core.Entities;
using Wms.Core.Enums;
using Wms.Core.Interfaces.Repositories;
using Wms.Core.Interfaces.Services;
using Wms.Core.Interfaces.Services.Audit;

namespace Wms.Services.Audit;

public class InventoryTransactionService(IInventoryTransactionRepository repository) : IInventoryTransactionService
{
    public async Task<PagedResult<InventoryTransaction>> GetPagedFilteredAsync(
        int? batchId = null,
        int? productId = null,
        string? userId = null,
        TransactionType? transactionType = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        return await repository.GetPagedFilteredAsync(
            batchId, productId, userId, transactionType, fromDate, toDate,
            pageNumber, pageSize, cancellationToken);
    }

    public async Task AddTransactionAsync(
        int batchId,
        int quantityChange,
        TransactionType transactionType,
        string userId,
        int? documentId = null,
        int? oldCellId = null,
        int? newCellId = null,
        CancellationToken cancellationToken = default)
    {
        var transaction = new InventoryTransaction
        {
            BatchId = batchId,
            QuantityChange = quantityChange,
            TransactionType = transactionType,
            UserId = userId,
            DocumentId = documentId,
            OldCellId = oldCellId,
            NewCellId = newCellId,
            Timestamp = DateTime.UtcNow
        };

        await repository.AddAsync(transaction, cancellationToken);
        // не вызываем SaveChanges – сохранение произойдёт в вызывающем сервисе
    }
}