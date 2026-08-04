using Microsoft.EntityFrameworkCore;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Repositories;
using Wms.Infrastructure.Data;

namespace Wms.Infrastructure.Repositories;

public class ZoneRepository(ApplicationDbContext context) : Repository<Zone>(context), IZoneRepository
{
    public async Task<IEnumerable<Zone>> GetZonesWithDetailsAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Zones
            .Include(z => z.Racks)
            .ThenInclude(r => r.Shelves)
            .ThenInclude(s => s.Cells)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Zone?> GetZoneWithRacksAsync(int zoneId, CancellationToken cancellationToken = default)
    {
        return await Context.Zones
            .Include(z => z.Racks)
            .FirstOrDefaultAsync(z => z.Id == zoneId, cancellationToken);
    }
}