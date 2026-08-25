using Microsoft.EntityFrameworkCore;
using Wms.Core.Entities;
using Wms.Core.DTOs.Common;
using Wms.Core.Enums;
using Wms.Core.Interfaces.Repositories;
using Wms.Infrastructure.Data;

namespace Wms.Infrastructure.Repositories;

public class InventoryTransactionRepository(ApplicationDbContext context)
    : Repository<InventoryTransaction>(context), IInventoryTransactionRepository
{
    private IQueryable<InventoryTransaction> BuildFilteredQuery(
        int? batchId,
        int? productId,
        string? userId,
        TransactionType? transactionType,
        DateTime? fromDate,
        DateTime? toDate)
    {
        var query = DbSet.AsNoTracking();

        if (batchId.HasValue)
            query = query.Where(t => t.BatchId == batchId.Value);

        if (productId.HasValue)
            query = query.Where(t => t.Batch.ProductId == productId.Value);

        if (!string.IsNullOrEmpty(userId))
            query = query.Where(t => t.UserId == userId);

        if (transactionType.HasValue)
            query = query.Where(t => t.TransactionType == transactionType.Value);

        if (fromDate.HasValue)
            query = query.Where(t => t.Timestamp >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(t => t.Timestamp <= toDate.Value);
        
        query = query.OrderByDescending(t => t.Timestamp);
        
        return query
            .Include(t => t.Batch)
                .ThenInclude(b => b.Product)
            .Include(t => t.Batch)
                .ThenInclude(b => b.Cell)
            .Include(t => t.OldCell)
            .Include(t => t.NewCell)
            .Include(t => t.User);
    }
    
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
        var query = BuildFilteredQuery(batchId, productId, userId, transactionType, fromDate, toDate);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return new PagedResult<InventoryTransaction>(items, totalCount, pageNumber, pageSize);
    }
}