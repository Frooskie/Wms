using Microsoft.EntityFrameworkCore;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Repositories;
using Wms.Infrastructure.Data;

namespace Wms.Infrastructure.Repositories;

public class SupplyRequestRepository(ApplicationDbContext context)
    : Repository<SupplyRequest>(context), ISupplyRequestRepository
{
    public async Task<SupplyRequest?> GetSupplyRequestWithLinesAsync(int id,
        CancellationToken cancellationToken = default)
    {
        return await Context.SupplyRequests
            .Include(sr => sr.Lines)
            .ThenInclude(l => l.Product)
            .FirstOrDefaultAsync(sr => sr.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<SupplyRequest>> GetSupplyRequestsWithLinesAsync(
        CancellationToken cancellationToken = default)
    {
        return await Context.SupplyRequests
            .Include(sr => sr.Lines)
            .ThenInclude(l => l.Product)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}