using Wms.Core.DTOs.Common;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Services.Base;

namespace Wms.Core.Interfaces.Services.Inventory;

public interface IBatchService : IReadOnlyService<Batch>
{
    Task<Batch> CreateBatchAsync(Batch batch, string userId, int? documentId = null,
        CancellationToken cancellationToken = default);

    Task MoveBatchAsync(int batchId, int newCellId,
        string userId, CancellationToken cancellationToken = default);

    Task DeleteBatchAsync(int id, CancellationToken cancellationToken = default);

    Task<PagedResult<Batch>> GetPagedBatchesWithFiltersAsync(
        int? productId = null,
        int? cellId = null,
        DateOnly? expiryFrom = null,
        DateOnly? expiryTo = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
}