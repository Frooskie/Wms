using Wms.Core.Entities;
using Wms.Core.Interfaces.Services.Base;

namespace Wms.Core.Interfaces.Services.WarehouseStructure;

public interface IRackService : ICrudService<Rack>
{
    Task<IEnumerable<Rack>> GetRacksWithShelvesAndCellsAsync(CancellationToken cancellationToken = default);
    Task<Rack?> GetRackWithShelvesAsync(int rackId, CancellationToken cancellationToken = default);
}