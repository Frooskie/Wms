using Wms.Core.Entities;
using Wms.Core.Enums;

namespace Wms.Core.Interfaces.Repositories;

public interface IInventoryTransactionRepository : IRepository<InventoryTransaction>
{
    Task<IEnumerable<InventoryTransaction>> GetFilteredAsync(
        int? batchId = null,
        int? productId = null,
        string? userId = null,
        TransactionType? transactionType = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default);
}