using Wms.Core.Entities;

namespace Wms.Core.Interfaces.Repositories;

public interface ISupplyRequestRepository : IRepository<SupplyRequest>
{
    Task<SupplyRequest?> GetSupplyRequestWithLinesAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SupplyRequest>> GetSupplyRequestsWithLinesAsync(CancellationToken cancellationToken = default);
}