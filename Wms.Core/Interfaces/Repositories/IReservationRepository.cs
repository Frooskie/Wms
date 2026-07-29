using Wms.Core.Entities;

namespace Wms.Core.Interfaces.Repositories;

public interface IReservationRepository : IRepository<Reservation>
{
    Task<IEnumerable<Reservation>> GetReservationsBySupplyOrderAsync(int supplyOrderId, CancellationToken cancellationToken = default);
    Task DeleteReservationsBySupplyOrderAsync(int supplyOrderId, CancellationToken cancellationToken = default);
}