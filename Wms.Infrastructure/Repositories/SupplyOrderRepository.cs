using Microsoft.EntityFrameworkCore;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Repositories;
using Wms.Infrastructure.Data;

namespace Wms.Infrastructure.Repositories;

public class SupplyOrderRepository(ApplicationDbContext context)
    : Repository<SupplyOrder>(context), ISupplyOrderRepository
{
    public async Task<SupplyOrder?> GetSupplyOrderWithLinesAndReservationsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Context.SupplyOrders
            .Include(so => so.Lines)
            .ThenInclude(l => l.Product)
            .Include(so => so.Reservations)
            .ThenInclude(r => r.Batch)
            .FirstOrDefaultAsync(so => so.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<SupplyOrder>> GetSupplyOrdersWithLinesAsync(CancellationToken cancellationToken = default)
    {
        return await Context.SupplyOrders
            .Include(so => so.Lines)
            .ThenInclude(l => l.Product)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}