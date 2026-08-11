using Wms.Core.Entities;
using Wms.Core.Interfaces.Repositories;
using Wms.Core.Interfaces.Services;
using Wms.Core.Interfaces.Services.Warehouse;
using Wms.Services.Base;

namespace Wms.Services.Warehouse;

public class ZoneService(IRepository<Zone> repository, IZoneRepository zoneRepository)
    : CrudService<Zone>(repository), IZoneService
{
    public async Task<IEnumerable<Zone>> GetZonesWithDetailsAsync(CancellationToken cancellationToken = default)
    {
        return await zoneRepository.GetZonesWithDetailsAsync(cancellationToken);
    }

    public async Task<Zone?> GetZoneWithRacksAsync(int zoneId, CancellationToken cancellationToken = default)
    {
        return await zoneRepository.GetZoneWithRacksAsync(zoneId, cancellationToken);
    }
}