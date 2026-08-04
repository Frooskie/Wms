using Microsoft.EntityFrameworkCore;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Repositories;
using Wms.Infrastructure.Data;

namespace Wms.Infrastructure.Repositories;

public class RackRepository(ApplicationDbContext context) : Repository<Rack>(context), IRackRepository
{
    public async Task<IEnumerable<Rack>> GetRacksWithShelvesAndCellsAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Racks
            .Include(r => r.Shelves)
            .ThenInclude(s => s.Cells)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Rack?> GetRackWithShelvesAsync(int rackId, CancellationToken cancellationToken = default)
    {
        return await Context.Racks
            .Include(r => r.Shelves)
            .FirstOrDefaultAsync(r => r.Id == rackId, cancellationToken);
    }
}