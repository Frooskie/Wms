using Wms.Core.Entities;

namespace Wms.Core.Interfaces.Repositories;

public interface IZoneRepository : IRepository<Zone>
{
    Task<IEnumerable<Zone>> GetZonesWithDetailsAsync(CancellationToken cancellationToken = default);
    Task<Zone?> GetZoneWithRacksAsync(int zoneId, CancellationToken cancellationToken = default);
}