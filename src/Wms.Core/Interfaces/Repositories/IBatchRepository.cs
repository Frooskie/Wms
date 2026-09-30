using Wms.Core.DTOs.Common;
using Wms.Core.Entities;

namespace Wms.Core.Interfaces.Repositories;

public interface IBatchRepository : IRepository<Batch>
{
    Task<IEnumerable<Batch>> GetBatchesWithDetailsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Batch>> GetBatchesByProductAsync(int productId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Batch>> GetBatchesByCellAsync(int cellId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Batch>> GetBatchesByExpiryDateRangeAsync(
        DateOnly from, DateOnly to,
        CancellationToken cancellationToken = default);

    Task<Batch?> GetBatchWithProductAndCellAsync(int batchId, CancellationToken cancellationToken = default);
    
    Task<PagedResult<Batch>> GetBatchesPagedFilteredAsync(
        int? productId = null,
        int? cellId = null,
        DateOnly? expiryFrom = null,
        DateOnly? expiryTo = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    Task<bool> IsCellOccupiedByOtherBatchAsync(int cellId, int? excludeBatchId = null,
        CancellationToken cancellationToken = default);
}