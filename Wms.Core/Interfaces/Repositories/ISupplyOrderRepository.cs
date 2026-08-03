using Wms.Core.Entities;

namespace Wms.Core.Interfaces.Repositories;

public interface ISupplyOrderRepository : IRepository<SupplyOrder>
{
    Task<SupplyOrder?> GetSupplyOrderWithLinesAndReservationsAsync(int id,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<SupplyOrder>> GetSupplyOrdersWithLinesAsync(CancellationToken cancellationToken = default);
}