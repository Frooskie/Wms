using Wms.Core.Entities;

namespace Wms.Core.Interfaces.Services;

public interface IBatchService : IReadOnlyService<Batch>
{
    Task<Batch> CreateBatchAsync(Batch batch, string userId, int? documentId = null,
        CancellationToken cancellationToken = default);

    Task MoveBatchAsync(int batchId, int newCellId,
        string userId, CancellationToken cancellationToken = default);

    Task DeleteBatchAsync(int id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Batch>> GetBatchesWithFiltersAsync(int? productId, int? cellId, DateTime? expiryFrom,
        DateTime? expiryTo, CancellationToken cancellationToken = default);
}