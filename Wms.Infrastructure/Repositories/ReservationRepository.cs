using Microsoft.EntityFrameworkCore;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Repositories;
using Wms.Infrastructure.Data;

namespace Wms.Infrastructure.Repositories;

public class ReservationRepository(ApplicationDbContext context)
    : Repository<Reservation>(context), IReservationRepository
{
    public async Task<IEnumerable<Reservation>> GetReservationsBySupplyOrderAsync(int supplyOrderId, CancellationToken cancellationToken = default)
    {
        return await Context.Reservations
            .Where(r => r.SupplyOrderId == supplyOrderId)
            .Include(r => r.Batch)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteReservationsBySupplyOrderAsync(int supplyOrderId, CancellationToken cancellationToken = default)
    {
        await Context.Reservations
            .Where(r => r.SupplyOrderId == supplyOrderId)
            .ExecuteDeleteAsync(cancellationToken);
    }
}