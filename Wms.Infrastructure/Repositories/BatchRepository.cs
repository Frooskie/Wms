using Microsoft.EntityFrameworkCore;
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

    public async Task<IEnumerable<Batch>> GetBatchesByProductAsync(int productId, CancellationToken cancellationToken = default)
    {
        return await Context.Batches
            .Where(b => b.ProductId == productId)
            .Include(b => b.Cell)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Batch>> GetBatchesByCellAsync(int cellId, CancellationToken cancellationToken = default)
    {
        return await Context.Batches
            .Where(b => b.CellId == cellId)
            .Include(b => b.Product)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Batch>> GetBatchesByExpiryDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        return await Context.Batches
            .Where(b => b.ExpiryDate >= from && b.ExpiryDate <= to)
            .Include(b => b.Product)
            .Include(b => b.Cell)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Batch?> GetBatchWithProductAndCellAsync(int batchId, CancellationToken cancellationToken = default)
    {
        return await Context.Batches
            .Include(b => b.Product)
            .Include(b => b.Cell)
            .FirstOrDefaultAsync(b => b.Id == batchId, cancellationToken);
    }

    public async Task<bool> IsCellOccupiedByOtherBatchAsync(int cellId, int? excludeBatchId = null, CancellationToken cancellationToken = default)
    {
        var query = Context.Batches.Where(b => b.CellId == cellId);
        
        if (excludeBatchId.HasValue)
            query = query.Where(b => b.Id != excludeBatchId.Value);
        
        return await query.AnyAsync(cancellationToken);
    }
}