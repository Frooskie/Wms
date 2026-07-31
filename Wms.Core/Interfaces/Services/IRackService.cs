using Wms.Core.Entities;

namespace Wms.Core.Interfaces.Services;

public interface IRackService : ICrudService<Rack>
{
    Task<IEnumerable<Rack>> GetRacksWithShelvesAndCellsAsync(CancellationToken cancellationToken = default);
    Task<Rack?> GetRackWithShelvesAsync(int rackId, CancellationToken cancellationToken = default);
}