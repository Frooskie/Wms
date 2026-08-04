using Wms.Core.Entities;

namespace Wms.Core.Interfaces.Repositories;

public interface IReceiptRepository : IRepository<Receipt>
{
    Task<Receipt?> GetReceiptWithLinesAsync(int receiptId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Receipt>> GetReceiptsWithLinesAsync(CancellationToken cancellationToken = default);
}