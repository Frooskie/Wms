using Wms.Core.Entities;

namespace Wms.Core.Interfaces.Repositories;

public interface IBatchRepository : IRepository<Batch>
{
    Task<IEnumerable<Batch>> GetBatchesWithDetailsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Batch>> GetBatchesByProductAsync(int productId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Batch>> GetBatchesByCellAsync(int cellId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Batch>> GetBatchesByExpiryDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<Batch?> GetBatchWithProductAndCellAsync(int batchId, CancellationToken cancellationToken = default);
    Task<bool> IsCellOccupiedByOtherBatchAsync(int cellId, int? excludeBatchId = null, CancellationToken cancellationToken = default);
}