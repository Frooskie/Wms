using Microsoft.EntityFrameworkCore;
using Wms.Core.DTOs.Common;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Repositories;
using Wms.Infrastructure.Data;

namespace Wms.Infrastructure.Repositories;

public class BatchRepository(ApplicationDbContext context) : Repository<Batch>(context), IBatchRepository
{
    public async Task<IEnumerable<Batch>> GetBatchesWithDetailsAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Batches
            .Include(b => b.Product)
            .Include(b => b.Cell)
            .ThenInclude(c => c.Shelf)
            .ThenInclude(s => s.Rack)
            .ThenInclude(r => r.Zone)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Batch>> GetBatchesByProductAsync(int productId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Batches
            .Where(b => b.ProductId == productId)
            .Include(b => b.Cell)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Batch>> GetBatchesByCellAsync(int cellId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Batches
            .Where(b => b.CellId == cellId)
            .Include(b => b.Product)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<Batch>> GetBatchesByExpiryDateRangeAsync(
        DateOnly from, DateOnly to,
        CancellationToken cancellationToken = default)
    {
        return await Context.Batches
            .Where(b => b.ExpiryDate >= from && b.ExpiryDate <= to)
            .Include(b => b.Product)
            .Include(b => b.Cell)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Batch?> GetBatchWithProductAndCellAsync(int batchId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Batches
            .Include(b => b.Product)
            .Include(b => b.Cell)
            .FirstOrDefaultAsync(b => b.Id == batchId, cancellationToken);
    }
    
    public async Task<PagedResult<Batch>> GetBatchesPagedFilteredAsync(
        int? productId = null,
        int? cellId = null,
        DateOnly? expiryFrom = null,
        DateOnly? expiryTo = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Batches
            .Include(b => b.Product)
            .Include(b => b.Cell)
            .ThenInclude(c => c.Shelf)
            .ThenInclude(s => s.Rack)
            .ThenInclude(r => r.Zone)
            .AsNoTracking();
        
        if (productId.HasValue)
            query = query.Where(b => b.ProductId == productId.Value);
        if (cellId.HasValue)
            query = query.Where(b => b.CellId == cellId.Value);
        if (expiryFrom.HasValue)
            query = query.Where(b => b.ExpiryDate >= expiryFrom.Value);
        if (expiryTo.HasValue)
            query = query.Where(b => b.ExpiryDate <= expiryTo.Value);
        
        query = query.OrderBy(b => b.ExpiryDate).ThenBy(b => b.Id);
        
        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Batch>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<bool> IsCellOccupiedByOtherBatchAsync(int cellId, int? excludeBatchId = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Batches.Where(b => b.CellId == cellId);

        if (excludeBatchId.HasValue)
            query = query.Where(b => b.Id != excludeBatchId.Value);

        return await query.AnyAsync(cancellationToken);
    }
}