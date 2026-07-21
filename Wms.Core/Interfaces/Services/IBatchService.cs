using Wms.Core.Entities;

namespace Wms.Core.Interfaces.Services;

public interface IBatchService : IBaseService<Batch>
{
    Task MoveBatchAsync(int batchId, int newCellId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Batch>> GetBatchesWithFiltersAsync(
        int? productId,
        int? cellId,
        DateTime? expiryFrom,
        DateTime? expiryTo,
        CancellationToken cancellationToken = default);
}