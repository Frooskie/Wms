using Wms.Core.Entities;
using Wms.Core.Interfaces.Services.Base;

namespace Wms.Core.Interfaces.Services.WarehouseStructure;

public interface IZoneService : ICrudService<Zone>
{
    Task<IEnumerable<Zone>> GetZonesWithDetailsAsync(CancellationToken cancellationToken = default);
    Task<Zone?> GetZoneWithRacksAsync(int zoneId, CancellationToken cancellationToken = default);
}